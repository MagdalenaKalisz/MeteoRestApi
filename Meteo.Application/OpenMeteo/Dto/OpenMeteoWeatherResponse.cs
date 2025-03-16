namespace Meteo.Application.OpenMeteo.Dto
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents the response from the OpenMeteo API containing weather data.
    /// </summary>
    public sealed class OpenMeteoWeatherResponse
    {
        /// <summary>
        /// Latitude of the location for which the weather data is provided.
        /// </summary>
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of the location for which the weather data is provided.
        /// </summary>
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Time taken to generate the response in milliseconds.
        /// </summary>
        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }

        /// <summary>
        /// UTC offset in seconds for the given location.
        /// </summary>
        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        /// <summary>
        /// Timezone name of the location.
        /// </summary>
        public string Timezone { get; set; } = null!;

        /// <summary>
        /// Abbreviation of the timezone.
        /// </summary>
        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; } = null!;

        /// <summary>
        /// Elevation of the location in meters.
        /// </summary>
        public double Elevation { get; set; }

        /// <summary>
        /// Units of measurement for the hourly weather data.
        /// </summary>
        [JsonPropertyName("hourly_units")]
        public WeatherHourlyUnitsDto HourlyUnits { get; set; } = null!;

        /// <summary>
        /// Hourly weather data.
        /// </summary>
        [JsonPropertyName("hourly")]
        public WeatherHourlyDto Hourly { get; set; } = null!;
    }
}
