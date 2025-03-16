namespace Meteo.Persistence.PostgreSql.Dao
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Library.Persistence;

    /// <summary>
    /// Data Access Object (DAO) for daily weather forecasts, representing forecast details for a specific day.
    /// </summary>
    [Table("WeatherForecastForDays")]
    public sealed class WeatherForecastForDayDao : IDaoWithId
    {
        /// <summary>
        /// Unique identifier for the daily forecast record.
        /// </summary>
        [Key] // Primary key annotation
        public Guid Id { get; set; }

        /// <summary>
        /// Foreign key linking to the associated weather forecast.
        /// </summary>
        public Guid WeatherForecastId { get; set; }

        /// <summary>
        /// Date and time of the forecast.
        /// </summary>
        public DateTimeOffset ForecastDateTime { get; set; }

        /// <summary>
        /// Temperature recorded in the forecast.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Humidity percentage recorded in the forecast.
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// Navigation property linking to the associated weather forecast.
        /// </summary>
        [ForeignKey(nameof(WeatherForecastId))]
        public WeatherForecastDao? WeatherForecast { get; set; }

        /// <summary>
        /// Parameterless constructor required by Entity Framework.
        /// </summary>
        public WeatherForecastForDayDao() { }

        /// <summary>
        /// Constructor for initializing WeatherForecastForDayDao with the required properties.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="forecastDateTime"></param>
        /// <param name="temperature"></param>
        /// <param name="humidity"></param>
        /// <param name="weatherForecastId"></param>
        public WeatherForecastForDayDao(Guid id, Guid weatherForecastId, DateTimeOffset forecastDateTime, double temperature, double humidity)
        {
            Id = id;
            WeatherForecastId = weatherForecastId;
            ForecastDateTime = forecastDateTime;
            Temperature = temperature;
            Humidity = humidity;
        }
    }
}
