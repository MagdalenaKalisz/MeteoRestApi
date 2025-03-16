namespace Meteo.Persistence.Json.Dao
{
    using System.Text.Json.Serialization;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// DAO representation of the WeatherForecastCreated event.
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
        /// Constructor.
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
