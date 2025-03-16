namespace Meteo.Application.Handlers.Commands
{
    using Library.Application;
    using Library.Application.Interfaces;
    using Library.Domain;
    using Meteo.Domain.Database;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Represents the command to delete a weather forecast definition by its ID.
    /// </summary>
    public sealed record DeleteWeatherForecastDefinitionCommand(WeatherForecastDefinitionId Id) : ICommand<bool>
    {
        /// <summary>
        /// A handler.
        /// </summary>
        /// <param name="_db"></param>
        /// <param name="_domainEventDispatcher"></param>
        private sealed class Handler(
            IWeatherDatabaseSession _db,
            IDomainEventDispatcher _domainEventDispatcher) : ICommandHandler<DeleteWeatherForecastDefinitionCommand, bool>
        {
            /// <inheritdoc/>
            public async Task<bool> HandleAsync(DeleteWeatherForecastDefinitionCommand command, CancellationToken cancellationToken)
            {
                Domain.Aggregates.WeatherForecastDefinition? instance = await _db.WeatherForecastDefinitions.GetByIdAsync(command.Id, default, cancellationToken);

                if (instance is null)
                {
                    return false;
                }

                List<IAggregateRoot> deletedAggregates = [];

                List<Domain.Aggregates.WeatherForecast> relatedForecasts = await _db.WeatherForecasts.GetAllAsync(default, cancellationToken);
                foreach (Domain.Aggregates.WeatherForecast? forecast in relatedForecasts.Where(f => f.DefinitionId == instance.Id).ToList())
                {
                    await _db.WeatherForecasts.DeleteAsync(forecast, cancellationToken);
                    deletedAggregates.Add(forecast);
                }

                await _db.WeatherForecastDefinitions.DeleteAsync(instance, cancellationToken);
                deletedAggregates.Add(instance);

                await _db.DispatchAndSaveChangesAsync(_domainEventDispatcher, cancellationToken, deletedAggregates);

                return true;
            }
        }
    }
}
