namespace Meteo.Application.Handlers.Commands
{
    using Library.Application;
    using Library.Application.Interfaces;
    using Library.Domain;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Database;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the command to delete all weather forecast definitions.
    /// </summary>
    public sealed record DeleteAllWeatherForecastDefinitionsCommand() : ICommand<bool>
    {
        /// <summary>
        /// A handler for deleting all weather forecast definitions.
        /// </summary>
        /// <param name="_db"></param>
        /// <param name="_domainEventDispatcher"></param>
        private sealed class Handler(
            IWeatherDatabaseSession _db,
            IDomainEventDispatcher _domainEventDispatcher) : ICommandHandler<DeleteAllWeatherForecastDefinitionsCommand, bool>
        {
            /// <inheritdoc/>
            public async Task<bool> HandleAsync(DeleteAllWeatherForecastDefinitionsCommand command, CancellationToken cancellationToken)
            {
                List<WeatherForecastDefinition> definitions = await _db.WeatherForecastDefinitions.GetAllAsync(default, cancellationToken);

                if (definitions is null || definitions.Count == 0)
                {
                    return false;
                }

                List<IAggregateRoot> deletedAggregates = [];

                List<WeatherForecast> forecasts = await _db.WeatherForecasts.GetAllAsync(default, cancellationToken);

                foreach (WeatherForecast forecast in forecasts.Where(f => definitions.Any(d => d.Id == f.DefinitionId)))
                {
                    await _db.WeatherForecasts.DeleteAsync(forecast, cancellationToken);
                    deletedAggregates.Add(forecast);
                }

                foreach (WeatherForecastDefinition definition in definitions)
                {
                    await _db.WeatherForecastDefinitions.DeleteAsync(definition, cancellationToken);
                    deletedAggregates.Add(definition);
                }

                await _db.DispatchAndSaveChangesAsync(_domainEventDispatcher, cancellationToken, deletedAggregates);

                return true;
            }
        }
    }
}
