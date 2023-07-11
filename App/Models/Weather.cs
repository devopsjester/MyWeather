namespace MyWeather.App.Models.Weather
{
    /// <summary>
    /// Represents the result of a weather request from the OpenWeatherMap API.
    /// </summary>
    public class WeatherResult
    {
        /// <summary>
        /// Gets or sets the weather conditions for the location.
        /// </summary>
        public Weather[] Weather { get; set; } = null!;

        /// <summary>
        /// Gets or sets the main weather parameters for the location.
        /// </summary>
        public Main Main { get; set; } = null!;
    }

    /// <summary>
    /// Represents the weather conditions for a location.
    /// </summary>
    public class Weather
    {
        /// <summary>
        /// Gets or sets the description of the weather conditions.
        /// </summary>
        public string Description { get; set; } = null!;
    }

    /// <summary>
    /// Represents the main weather parameters for a location.
    /// </summary>
    public class Main
    {
        /// <summary>
        /// Gets or sets the temperature in Kelvin.
        /// </summary>
        public double Temp { get; set; }
    }
}