namespace Meteo.Application.OpenMeteo.Dto
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents hourly weather data.
    /// </summary>
    public sealed class WeatherHourlyDto
    {
        /// <summary>
        /// List of time entries corresponding to weather data.
        /// </summary>
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = [];

        /// <summary>
        /// List of temperature values at 2 meters above ground level.
        /// </summary>
        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature2m { get; set; } = [];

        /// <summary>
        /// List of relative humidity values at 2 meters above ground level.
        /// </summary>
        [JsonPropertyName("relative_humidity_2m")]
        public List<int> RelativeHumidity2m { get; set; } = [];
    }
}
