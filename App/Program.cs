using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Serilog;
using Serilog.Events;
using MyWeather.App.Models.Weather;
using MyWeather.App.Models.Geocoding;
using MyWeather.App.Models.ReverseGeocoding;

namespace MyWeather.App
{
    class Program
    {
        const string API_KEY = "1b123c9a91468b0da3e0a39c238b2a01";
        const string BING_API_KEY = "ApY9o9zqdAMNwwzcfi8JfqoBDoFs8GzdPHtRSMVykzwo7FJRrUwP2GXtY2FwLGst";

        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var cityOption = new Option<string>("--city", "The name of the city");
            var stateOption = new Option<string>("--state", "The name of the state");
            var zipcodeOption = new Option<string>("--zipcode", "The zipcode of the location");
            var metricOption = new Option<bool>("--metric", "Use metric units");
            var imperialOption = new Option<bool>("--imperial", "Use imperial units");

            zipcodeOption.AddValidator(option =>
            {
                // if zipcode is specified, city and state cannot be specified
                if (option.GetValueForOption(zipcodeOption) != null && (option.GetValueForOption(cityOption) != null || option.GetValueForOption(stateOption) != null))
                {
                    option.ErrorMessage = "Cannot specify both --zipcode and --city/--state";
                }
            });

            metricOption.AddValidator(option =>
            {
                // --metric and --imperial are mutually exclusive
                if (option.GetValueForOption(imperialOption))
                {
                    option.ErrorMessage = "Cannot specify both --metric and --imperial";
                }
            });

            imperialOption.AddValidator(option =>
            {
                // --metric and --imperial are mutually exclusive
                if (option.GetValueForOption(metricOption))
                {
                    option.ErrorMessage = "Cannot specify both --metric and --imperial";
                }
            });

            //rootCommand.Description = "Retrieves the current weather conditions for a location";
            var rootCommand = new RootCommand
            {
                cityOption,
                stateOption,
                zipcodeOption,
                metricOption,
                imperialOption
            };

            rootCommand.Name = "weather";
            rootCommand.Description = "Retrieves the current weather conditions for a location";

            rootCommand.Handler = CommandHandler.Create<string, string, string, bool, bool>((city, state, zipcode, metric, imperial) =>
            {
                string units = string.Empty;
                if (metric)
                {
                    units = "metric";
                }
                else if (imperial)
                {
                    units = "imperial";
                }

                Log.Information("Retrieving weather data in {units} units...", units);

                if (zipcode != null)
                {
                    Log.Information("Zipcode is {zipcode}", zipcode);
                    return GetWeatherForZipcode(zipcode, units);
                }
                else
                {
                    Log.Information("Retrieving weather data for city {city} and state {state}", city, state);
                    return GetWeatherForCityAndState(city, state, units);
                }
            });

            await rootCommand.InvokeAsync(args);

            Log.CloseAndFlush();
        }

        private static async Task<string> GetCountryAsync(double latitude, double longitude)
        {
            var httpClient = new HttpClient();
            var reverseGeocodingResponse = await httpClient.GetAsync($"https://dev.virtualearth.net/REST/v1/Locations/{latitude},{longitude}?key={BING_API_KEY}&includeEntityTypes=CountryRegion");
            var reverseGeocodingContent = await reverseGeocodingResponse.Content.ReadAsStringAsync();
            var reverseGeocodingResult = JsonConvert.DeserializeObject<BingMapsReverseGeocodingResult>(reverseGeocodingContent);

            if (reverseGeocodingResult.ResourceSets[0].Resources.Length == 0)
            {
                Log.Error("Unable to reverse geocode location for latitude {latitude} and longitude {longitude}", latitude, longitude);

                throw new Exception($"Unable to reverse geocode location for latitude {latitude} and longitude {longitude}");
            }

            var country = reverseGeocodingResult.ResourceSets[0].Resources[0].Address.CountryRegion;

            return country;
        }

        private static async Task<WeatherResult> GetWeatherDataAsync(double latitude, double longitude, string units)
        {
            var httpClient = new HttpClient();
            var weatherResponse = await httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={API_KEY}&units={units}");
            var weatherContent = await weatherResponse.Content.ReadAsStringAsync();
            var weatherResult = JsonConvert.DeserializeObject<WeatherResult>(weatherContent);

            return weatherResult;
        }

        static async Task<int> GetWeatherForCityAndState(string city, string state, string units)
        {
            var location = $"{city}, {state}";

            var httpClient = new HttpClient();
            var geocodingResponse = await httpClient.GetAsync($"https://dev.virtualearth.net/REST/v1/Locations?q={location}&key={BING_API_KEY}");
            var geocodingContent = await geocodingResponse.Content.ReadAsStringAsync();
            var geocodingResult = JsonConvert.DeserializeObject<BingMapsGeocodingResult>(geocodingContent);

            if (geocodingResult.ResourceSets[0].Resources.Length == 0)
            {
                Console.WriteLine("Unable to geocode location");
                return 1;
            }

            var latitude = geocodingResult.ResourceSets[0].Resources[0].Point.Coordinates[0];
            var longitude = geocodingResult.ResourceSets[0].Resources[0].Point.Coordinates[1];

            if (string.IsNullOrEmpty(units))
            {
                var country = await GetCountryAsync(latitude, longitude);
                units = country == "United States" ? "imperial" : "metric";
            }

            try
            {
                var weatherResult = await GetWeatherDataAsync(latitude, longitude, units);
                var logUnits = units == "metric" ? "celsius" : "fahrenheit";

                Log.Information("Retrieved weather data for {location}: {description}, {temperature} degrees {logUnits}", location, weatherResult.Weather[0].Description, weatherResult.Main.Temp, logUnits);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving weather data for location {location}", location);
                return 1;
            }
        }

        static async Task<int> GetWeatherForZipcode(string zipcode, string units)
        {
            Log.Information("Retrieving weather data for zipcode {zipcode}", zipcode);

            var httpClient = new HttpClient();
            var geocodingResponse = await httpClient.GetAsync($"https://dev.virtualearth.net/REST/v1/Locations/{zipcode}?key={BING_API_KEY}");
            var geocodingContent = await geocodingResponse.Content.ReadAsStringAsync();
            var geocodingResult = JsonConvert.DeserializeObject<BingMapsGeocodingResult>(geocodingContent);

            if (geocodingResult.ResourceSets[0].Resources.Length == 0)
            {
                Log.Error("Unable to geocode location for zipcode {zipcode}", zipcode);
                Log.Error("Error message: {errorMessage}", geocodingResult.StatusCode);

                return 1;
            }

            var latitude = geocodingResult.ResourceSets[0].Resources[0].Point.Coordinates[0];
            var longitude = geocodingResult.ResourceSets[0].Resources[0].Point.Coordinates[1];

            if (string.IsNullOrEmpty(units))
            {
                var country = await GetCountryAsync(latitude, longitude);
                units = country == "United States" ? "imperial" : "metric";
            }

            try
            {
                var weatherResult = await GetWeatherDataAsync(latitude, longitude, units);

                Log.Information("Retrieved weather data for zipcode {zipcode}: {description}, {temperature} degrees Fahrenheit", zipcode, weatherResult.Weather[0].Description, weatherResult.Main.Temp);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving weather data for zipcode {zipcode}", zipcode);
                return 1;
            }
        }
    }
}
