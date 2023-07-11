namespace MyWeather.App.Models.Geocoding
{
    /// <summary>
    /// Represents the result of a Bing Maps geocoding request.
    /// </summary>
    public class BingMapsGeocodingResult
    {
        /// <summary>
        /// Gets or sets the status code of the geocoding request.
        /// </summary>
        public string? StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status description of the geocoding request.
        /// </summary>
        public string? StatusDescription { get; set; }

        /// <summary>
        /// Gets or sets the resource sets returned by the geocoding request.
        /// </summary>
        public ResourceSet[]? ResourceSets { get; set; }
    }

    /// <summary>
    /// Represents a set of resources returned by a Bing Maps geocoding request.
    /// </summary>
    public class ResourceSet
    {
        /// <summary>
        /// Gets or sets the estimated total number of resources in the set.
        /// </summary>
        public int EstimatedTotal { get; set; }

        /// <summary>
        /// Gets or sets the resources in the set.
        /// </summary>
        public Resource[]? Resources { get; set; }
    }

    /// <summary>
    /// Represents a resource returned by a Bing Maps geocoding request.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the point associated with the resource.
        /// </summary>
        public Point? Point { get; set; }
    }

    /// <summary>
    /// Represents a point on the Earth's surface.
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Gets or sets the coordinates of the point.
        /// </summary>
        public double[]? Coordinates { get; set; }
    }
}