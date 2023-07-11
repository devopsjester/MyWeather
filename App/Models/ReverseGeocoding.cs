namespace MyWeather.App.Models.ReverseGeocoding
{
    /// <summary>
    /// Represents the result of a reverse geocoding request to the Bing Maps REST Services API.
    /// </summary>
    public class BingMapsReverseGeocodingResult
    {
        /// <summary>
        /// Gets or sets the status code of the reverse geocoding request.
        /// </summary>
        public string? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status description of the reverse geocoding request.
        /// </summary>
        public string? StatusDescription { get; set; }

        /// <summary>
        /// Gets or sets the array of resource sets returned by the reverse geocoding request.
        /// </summary>
        public ResourceSet[]? ResourceSets { get; set; }
    }

    /// <summary>
    /// Represents a set of resources returned by a reverse geocoding request to the Bing Maps REST Services API.
    /// </summary>
    public class ResourceSet
    {
        /// <summary>
        /// Gets or sets the estimated total number of resources in the resource set.
        /// </summary>
        public int EstimatedTotal { get; set; }

        /// <summary>
        /// Gets or sets the array of resources in the resource set.
        /// </summary>
        public Resource[]? Resources { get; set; }
    }

    /// <summary>
    /// Represents a resource returned by a reverse geocoding request to the Bing Maps REST Services API.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the address of the resource.
        /// </summary>
        public Address? Address { get; set; }
    }

    /// <summary>
    /// Represents an address returned by a reverse geocoding request to the Bing Maps REST Services API.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the country or region of the address.
        /// </summary>
        public string? CountryRegion { get; set; }

        /// <summary>
        /// Gets or sets the first-level administrative division of the address, such as a state or province.
        /// </summary>
        public string? AdminDistrict { get; set; }

        /// <summary>
        /// Gets or sets the second-level administrative division of the address, such as a county or district.
        /// </summary>
        public string? AdminDistrict2 { get; set; }
    }
}