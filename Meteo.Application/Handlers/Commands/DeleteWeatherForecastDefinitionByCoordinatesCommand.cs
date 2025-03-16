namespace Meteo.Application.Handlers.Commands
{
    using Library.Application;
    using Library.Application.Interfaces;
    using Library.Domain;
    using Library.Domain.Database;
    using Meteo.Domain.Database;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Represents the command to delete a weather forecast definition by coordinates.
    /// </summary>
    public sealed record DeleteWeatherForecastDefinitionByCoordinatesCommand(
        Coordinates Coordinates) : ICommand<bool>
    {
        /// <summary>
        /// A handler.
        /// </summary>
        /// <param name="_db"></param>
        /// <param name="_domainEventDispatcher"></param>
        private sealed class Handler(
            IWeatherDatabaseSession _db,
            IDomainEventDispatcher _domainEventDispatcher) : ICommandHandler<DeleteWeatherForecastDefinitionByCoordinatesCommand, bool>
        {
            /// <inheritdoc/>
            public async Task<bool> HandleAsync(DeleteWeatherForecastDefinitionByCoordinatesCommand command, CancellationToken cancellationToken)
            {
                Domain.Aggregates.WeatherForecastDefinition? instance = await _db.WeatherForecastDefinitions.GetByCoordinatesAsync(command.Coordinates, QueryBehavior.Default, cancellationToken);

                if (instance is null)
                {
                    return false;
                }

                List<IAggregateRoot> deletedAggregates = [];

                List<Domain.Aggregates.WeatherForecast> relatedForecasts = await _db.WeatherForecasts.GetAllAsync(QueryBehavior.Default, cancellationToken);

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
