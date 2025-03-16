namespace Meteo.Persistence.PostgreSql.Dao
{
    using System;
    using System.Text.Json.Serialization;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Represents a data transfer object for <see cref="WeatherForecastId"/>.
    /// </summary>
    public class WeatherForecastIdDao
    {
        /// <summary>
        /// Gets or sets the unique identifier of the weather forecast.
        /// </summary>
        public Guid Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastIdDao"/> class.
        /// </summary>
        [JsonConstructor]
        public WeatherForecastIdDao(Guid value)
        {
            Value = value;
        }

        /// <summary>
        /// Implicit conversion from <see cref="WeatherForecastIdDao"/> to <see cref="WeatherForecastId"/>.
        /// </summary>
        public static implicit operator WeatherForecastId(WeatherForecastIdDao dao) => WeatherForecastId.Create(dao.Value);

        /// <summary>
        /// Implicit conversion from <see cref="WeatherForecastId"/> to <see cref="WeatherForecastIdDao"/>.
        /// </summary>
        public static implicit operator WeatherForecastIdDao(WeatherForecastId id) => new WeatherForecastIdDao(id.Value);
    }
}
