namespace Meteo.Persistence.PostgreSql
{
    using Domain.Database;
    using Library.Persistence;

    /// <summary>
    /// Default implementation of <see cref="IWeatherDatabaseSession"/>.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="weatherForecastRepository"></param>
    /// <param name="weatherForecastDefinitionRepository"></param>
    /// <param name="outboxRepository"></param>
    public sealed class PostgreSqlWeatherDatabaseSession(
        ApplicationDbContext context,
        IWeatherForecastCollection weatherForecastRepository,
        IWeatherForecastDefinitionCollection weatherForecastDefinitionRepository,
        IOutboxRepository outboxRepository) : DatabaseSession(context), IWeatherDatabaseSession
    {
        /// <inheritdoc/>
        public IWeatherForecastCollection WeatherForecasts { get; } = weatherForecastRepository;

        /// <inheritdoc/>
        public IWeatherForecastDefinitionCollection WeatherForecastDefinitions { get; } = weatherForecastDefinitionRepository;

        /// <summary>
        /// Gets the outbox repository.
        /// </summary>
        public IOutboxRepository OutboxRepository { get; } = outboxRepository;

    }
}
