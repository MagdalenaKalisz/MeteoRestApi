namespace Meteo.Api.Contracts.WeatherForecasts
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents the response for a daily weather forecast.
    /// </summary>
    public sealed class WeatherForecastForDayDto
    {
        /// <summary>
        /// Date and time of the forecast.
        /// </summary>
        [JsonPropertyName("forecastDateTime")]
        public DateTimeOffset ForecastDateTime { get; set; }

        /// <summary>
        /// Temperature recorded in the forecast.
        /// </summary>
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        /// <summary>
        /// The unit of the temperature (Celsius, Fahrenheit, Kelvin).
        /// </summary>
        [JsonPropertyName("temperatureUnit")]
        public string TemperatureUnit { get; set; } = "Celsius"; // Default to Celsius

        /// <summary>
        /// Humidity percentage recorded in the forecast.
        /// </summary>
        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastForDayDto"/> class.
        /// </summary>
        /// <param name="forecastDateTime"></param>
        /// <param name="temperature"></param>
        /// <param name="temperatureUnit"></param>
        /// <param name="humidity"></param>
        public WeatherForecastForDayDto(DateTimeOffset forecastDateTime, double temperature, string temperatureUnit, double humidity)
        {
            ForecastDateTime = forecastDateTime;
            Temperature = temperature;
            TemperatureUnit = temperatureUnit;
            Humidity = humidity;
        }
    }
}
