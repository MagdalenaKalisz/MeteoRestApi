namespace Meteo.Domain.Events
{
    using Library.Domain;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Represents a domain event that occurs when a weather forecast definition is created.
    /// </summary>
    public sealed class WeatherForecastDefinitionCreated : IDomainEvent
    {
        /// <summary>
        /// Gets the identifier of the created weather forecast definition.
        /// </summary>
        public WeatherForecastDefinitionId DefinitionId { get; }

        /// <summary>
        /// Gets the geographical coordinates associated with the weather forecast definition.
        /// </summary>
        public Coordinates Coordinates { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastDefinitionCreated"/> class.
        /// </summary>
        /// <param name="definitionId">The unique identifier of the weather forecast definition.</param>
        /// <param name="coordinates">The geographical coordinates of the forecast definition.</param>
        public WeatherForecastDefinitionCreated(WeatherForecastDefinitionId definitionId, Coordinates coordinates)
        {
            DefinitionId = definitionId;
            Coordinates = coordinates;
        }
    }
}
