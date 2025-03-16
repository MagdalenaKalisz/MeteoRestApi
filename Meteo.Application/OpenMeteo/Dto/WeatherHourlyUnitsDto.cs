namespace Meteo.Application.OpenMeteo.Dto
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents the units of measurement for hourly weather data.
    /// </summary>
    public sealed class WeatherHourlyUnitsDto
    {
        /// <summary>
        /// Unit for time representation.
        /// </summary>
        public string Time { get; set; } = null!;

        /// <summary>
        /// Unit for temperature at 2 meters above ground level.
        /// </summary>
        [JsonPropertyName("temperature_2m")]
        public string Temperature2m { get; set; } = null!;

        /// <summary>
        /// Unit for relative humidity at 2 meters above ground level.
        /// </summary>
        [JsonPropertyName("relative_humidity_2m")]
        public string RelativeHumidity2m { get; set; } = null!;
    }
}
