namespace Meteo.Domain.Events
{
    using Library.Domain;

    /// <summary>
    /// Represents a domain event that occurs when a weather forecast is created.
    /// </summary>
    public sealed class WeatherForecastCreated : IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier of the created weather forecast.
        /// </summary>
        public WeatherForecastId ForecastId { get; }

        /// <summary>
        /// Gets the identifier of the weather forecast definition associated with this event.
        /// </summary>
        public WeatherForecastDefinitionId DefinitionId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastCreated"/> class.
        /// </summary>
        /// <param name="forecastId"></param>
        /// <param name="definitionId"></param>
        public WeatherForecastCreated(WeatherForecastId forecastId, WeatherForecastDefinitionId definitionId)
        {
            ForecastId = forecastId;
            DefinitionId = definitionId;
        }
    }
}
