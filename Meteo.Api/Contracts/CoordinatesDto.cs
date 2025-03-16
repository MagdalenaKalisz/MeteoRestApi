namespace Meteo.Api.Contracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Coordinates.
    /// </summary>
    public sealed class CoordinatesDto
    {
        /// <summary>
        /// Latitude.
        /// </summary>
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude.
        /// </summary>
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}