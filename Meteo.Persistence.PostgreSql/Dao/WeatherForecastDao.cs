namespace Meteo.Persistence.PostgreSql.Dao
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Library.Persistence;

    /// <summary>
    /// Data Access Object (DAO) for weather forecasts, representing database records.
    /// </summary>
    [Table("WeatherForecasts")]
    public sealed class WeatherForecastDao : IDaoWithId
    {
        /// <summary>
        /// Unique identifier for the weather forecast record.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Timestamp indicating when the forecast record was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Related forecast definition ID.
        /// </summary>
        public Guid DefinitionId { get; set; }

        /// <summary>
        /// List of daily weather forecasts associated with this record.
        /// </summary>
        public List<WeatherForecastForDayDao>? Forecasts { get; set; } = [];

        /// <summary>
        /// Parameterless constructor required by Entity Framework.
        /// </summary>
        public WeatherForecastDao() { }

        /// <summary>
        /// Custom constructor for initialization
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="definitionId"></param>
        /// <param name="forecasts"></param>
        public WeatherForecastDao(Guid id, DateTimeOffset createdAt, Guid definitionId, List<WeatherForecastForDayDao> forecasts)
        {
            Id = id;
            CreatedAt = createdAt;
            DefinitionId = definitionId;
            Forecasts = forecasts ?? [];
        }
    }
}
