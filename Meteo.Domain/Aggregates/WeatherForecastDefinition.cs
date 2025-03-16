namespace Meteo.Domain.Aggregates
{
    using Meteo.Domain.Events;

    /// <summary>
    /// Weather forecast definition aggregate root.
    /// </summary>
    public sealed class WeatherForecastDefinition : AggregateRoot<WeatherForecastDefinitionId>
    {
        /// <summary>
        /// Coordinates of the weather forecast definition.
        /// </summary>
        public Coordinates Coordinates { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastDefinition"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="coordinates"></param>
        private WeatherForecastDefinition(WeatherForecastDefinitionId id,
                                          CreationTimestamp createdAt,
                                          Coordinates coordinates) :
            base(id, createdAt)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            Coordinates = coordinates;
        }

        /// <summary>
        /// Factory method to create a new WeatherForecast instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="coordinates"></param>
        public static WeatherForecastDefinition Create(WeatherForecastDefinitionId id,
                                                       CreationTimestamp createdAt,
                                                       Coordinates coordinates)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(createdAt);
            ArgumentNullException.ThrowIfNull(coordinates);

            WeatherForecastDefinition definition = new(id, createdAt, coordinates);

            definition.AddDomainEvent(new WeatherForecastDefinitionCreated(id, coordinates));

            return definition;
        }
    }
}
