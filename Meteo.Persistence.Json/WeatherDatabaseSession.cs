namespace Meteo.Persistence.Json
{
    using Library.Persistence;
    using Meteo.Domain.Database;
    using Meteo.Persistence.Json.Collections;

    /// <summary>
    /// JSON-based implementation of <see cref="IWeatherDatabaseSession"/>.
    /// </summary>
    public sealed class WeatherDatabaseSession : IWeatherDatabaseSession
    {
        private readonly IOutboxRepository _outboxRepository;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="outboxRepository"></param>
        public WeatherDatabaseSession(string basePath, IOutboxRepository outboxRepository)
        {
            WeatherForecasts = new WeatherForecastCollection(Path.Combine(basePath, "weather_forecasts.json"));
            WeatherForecastDefinitions = new WeatherForecastDefinitionCollection(Path.Combine(basePath, "weather_forecast_definitions.json"));
            _outboxRepository = outboxRepository;
        }

        /// <summary>
        /// Weather forecasts collection
        /// </summary>
        public IWeatherForecastCollection WeatherForecasts { get; }

        /// <summary>
        /// Weather forecast definitions collection
        /// </summary>
        public IWeatherForecastDefinitionCollection WeatherForecastDefinitions { get; }

        /// <summary>
        /// Saves changes (not required for JSON persistence)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Begins a transaction (not required for JSON persistence)
        /// </summary>
        /// <returns></returns>
        public Task BeginTransactionAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Commits a transaction (not required for JSON persistence)
        /// </summary>
        /// <returns></returns>
        public Task CommitTransactionAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Rolls back a transaction (not required for JSON persistence)
        /// </summary>
        /// <returns></returns>
        public Task RollbackTransactionAsync()
        {
            return Task.CompletedTask;
        }
    }
}
