namespace Meteo.Persistence.Json.Dao
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Single day's weather forecast for JSON storage.
    /// </summary>
    public sealed class WeatherForecastForDayDao
    {
        /// <summary>
        /// Forecast date and time.
        /// </summary>
        public DateTimeOffset ForecastDateTime { get; }

        /// <summary>
        /// Temperature value.
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        /// Humidity percentage.
        /// </summary>
        public double Humidity { get; }

        /// <summary>
        /// JSON Constructor for Deserialization
        /// </summary>
        [JsonConstructor]
        public WeatherForecastForDayDao(DateTimeOffset forecastDateTime, double temperature, double humidity)
        {
            ForecastDateTime = forecastDateTime;
            Temperature = temperature;
            Humidity = humidity;
        }
    }
}
