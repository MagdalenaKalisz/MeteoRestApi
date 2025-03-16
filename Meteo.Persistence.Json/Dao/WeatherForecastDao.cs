namespace Meteo.Persistence.Json.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Weather forecast for JSON storage.
    /// </summary>
    public sealed class WeatherForecastDao
    {
        /// <summary>
        /// Forecast ID.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Forecast creation timestamp.
        /// </summary>
        public DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Related forecast definition ID.
        /// </summary>
        public Guid DefinitionId { get; }

        /// <summary>
        /// List of daily forecasts.
        /// </summary>
        public IEnumerable<WeatherForecastForDayDao> Forecasts { get; } = [];

        /// <summary>
        /// JSON Constructor for Deserialization
        /// </summary>
        [JsonConstructor]
        public WeatherForecastDao(Guid id, DateTimeOffset createdAt, Guid definitionId, IEnumerable<WeatherForecastForDayDao> forecasts)
        {
            Id = id;
            CreatedAt = createdAt;
            DefinitionId = definitionId;
            Forecasts = forecasts ?? [];
        }
    }
}
