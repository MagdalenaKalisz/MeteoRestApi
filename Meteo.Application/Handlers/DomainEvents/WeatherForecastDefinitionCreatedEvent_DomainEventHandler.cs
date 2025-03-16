namespace Meteo.Application.Handlers.DomainEvents
{
    using System.Threading;
    using System.Threading.Tasks;
    using Meteo.Domain.Events;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Handles the <see cref="WeatherForecastDefinitionCreated"/> domain event.
    /// </summary>
    public sealed class WeatherForecastDefinitionCreatedEvent_DomainEventHandler : IDomainEventHandler<WeatherForecastDefinitionCreated>
    {
        private readonly ILogger<WeatherForecastDefinitionCreatedEvent_DomainEventHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastDefinitionCreatedEvent_DomainEventHandler"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastDefinitionCreatedEvent_DomainEventHandler(ILogger<WeatherForecastDefinitionCreatedEvent_DomainEventHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Handles the <see cref="WeatherForecastDefinitionCreated"/> event.
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <param name="cancellationToken"></param>
        public async Task HandleAsync(WeatherForecastDefinitionCreated domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Weather forecast created: {DefinitionId} at {Coordinates}", domainEvent.DefinitionId, domainEvent.Coordinates);

            // Add business logic here, e.g., notify an external system, enqueue a message, etc.

            await Task.CompletedTask;
        }
    }
}
