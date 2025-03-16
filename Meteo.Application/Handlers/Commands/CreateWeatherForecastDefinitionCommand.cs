namespace Meteo.Application.Handlers.Commands
{
    using Library.Application;
    using Library.Application.Events;

    using Library.Application.Interfaces;
    using Library.Domain.Database;

    using Library.Identifiers;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Database;

    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Represents the command to create a new weather forecast definition.
    /// </summary>
    /// <param name="Coordinates"></param>
    /// <param name="Email"></param>
    public sealed record CreateWeatherForecastDefinitionCommand(
        Coordinates Coordinates, string? Email) : ICommand<WeatherForecastDefinitionId>
    {
        /// <summary>
        /// A handler.
        /// </summary>
        /// <param name="_time"></param>
        /// <param name="_identifierProvider"></param>
        /// <param name="_db"></param>
        /// <param name="_outbox"></param>
        /// <param name="_domainEventDispatcher"></param>
        private sealed class Handler(
            TimeProvider _time,
            IdentifierProvider _identifierProvider,
            IWeatherDatabaseSession _db,
            IOutbox _outbox,
            IDomainEventDispatcher _domainEventDispatcher) : ICommandHandler<CreateWeatherForecastDefinitionCommand, WeatherForecastDefinitionId?>
        {
            /// <inheritdoc/>
            public async Task<WeatherForecastDefinitionId?> HandleAsync(CreateWeatherForecastDefinitionCommand command, CancellationToken cancellationToken)
            {
                WeatherForecastDefinition? forecast = await _db.WeatherForecastDefinitions.GetByCoordinatesAsync(command.Coordinates, QueryBehavior.NoTracking, cancellationToken);

                if (forecast is not null)
                {
                    return null;
                }
                WeatherForecastDefinition instance = WeatherForecastDefinition.Create(
                    _identifierProvider.CreateSequentialUuid(),
                    _time.GetUtcNow(),
                    command.Coordinates);

                await _db.WeatherForecastDefinitions.AddAsync(instance, cancellationToken);

                await _db.DispatchAndSaveChangesAsync(_domainEventDispatcher, cancellationToken, instance);

                if (command.Email?.Length > 0)
                {
                    var emailEvent = new EmailOutboxEvent(
                        "vaporbae@gmail.com",
                        "New Weather Forecast Created",
                        $"A new weather forecast has been created for coordinates: {command.Coordinates.Latitude}, {command.Coordinates.Longitude}."
                    );

                    await _outbox.SaveEventAsync(emailEvent, cancellationToken);
                }

                return instance.Id;
            }
        }
    }
}
