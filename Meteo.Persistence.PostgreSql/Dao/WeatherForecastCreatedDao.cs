namespace Meteo.Persistence.PostgreSql.Dao
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// DAO representation of the WeatherForecastCreated event (for deserialization only).
    /// </summary>
    public class WeatherForecastCreatedDao
    {
        /// <summary>
        /// Forecast identifier.
        /// </summary>
        public Guid ForecastId { get; set; }

        /// <summary>
        /// Weather forecast definition identifier.
        /// </summary>
        public Guid DefinitionId { get; set; }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="forecastId">Forecast identifier.</param>
        /// <param name="definitionId">Weather forecast definition identifier.</param>
        [JsonConstructor]
        public WeatherForecastCreatedDao(Guid forecastId, Guid definitionId)
        {
            ForecastId = forecastId;
            DefinitionId = definitionId;
        }
    }
}
