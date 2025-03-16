namespace Meteo.Domain.Aggregates
{
    using Library.Domain;
    using Meteo.Domain.Events;

    /// <summary>
    /// Weather forecast aggregate root for a specific day.
    /// </summary>
    public sealed class WeatherForecast : AggregateRoot<WeatherForecastId>
    {
        /// <summary>
        /// The definition id of the weather forecast.
        /// </summary>
        public WeatherForecastDefinitionId DefinitionId { get; }

        /// <summary>
        /// The forecast for the day.
        /// </summary>
        public IReadOnlyList<WeatherForecastForDay> Forecasts { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecast"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="definitionId"></param>
        /// <param name="forecasts"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private WeatherForecast(WeatherForecastId id,
                                CreationTimestamp createdAt,
                                WeatherForecastDefinitionId definitionId,
                                IEnumerable<WeatherForecastForDay> forecasts)
            : base(id, createdAt)
        {
            ArgumentNullException.ThrowIfNull(definitionId);
            ArgumentNullException.ThrowIfNull(forecasts);

            if (!forecasts.Any())
            {
                Forecasts = [];
                DefinitionId = definitionId;
                return;
            }

            Forecasts = [.. forecasts];

            if (Forecasts.Count > 24)
            {
                throw new ArgumentOutOfRangeException(nameof(forecasts), "Each day cannot have more than 24 forecast entries.");
            }

            if (Forecasts.Select(f => f.ForecastDateTime).Distinct().Take(Forecasts.Count + 1).Count() != Forecasts.Count)
            {
                throw new ArgumentException("Forecast contains duplicate hours.", nameof(forecasts));
            }

            DefinitionId = definitionId;
        }

        /// <summary>
        /// Factory method to create a new WeatherForecast instance.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="definitionId"></param>
        /// <param name="forecasts"></param>
        /// <returns></returns>
        public static WeatherForecast Create(WeatherForecastId id,
                                             CreationTimestamp createdAt,
                                             WeatherForecastDefinitionId definitionId,
                                             IEnumerable<WeatherForecastForDay> forecasts)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(definitionId);
            ArgumentNullException.ThrowIfNull(forecasts);

            WeatherForecast weatherForecast = new(id, createdAt, definitionId, forecasts);

            weatherForecast.AddDomainEvent(new WeatherForecastCreated(id, definitionId));

            return weatherForecast;
        }
    }
}
