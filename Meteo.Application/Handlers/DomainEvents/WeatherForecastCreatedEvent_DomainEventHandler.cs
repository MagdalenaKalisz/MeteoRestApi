namespace Meteo.Application.Handlers.DomainEvents
{
    using Meteo.Domain.Events;
    using Microsoft.Extensions.Logging;


    /// <summary>
    /// A handler for the <see cref="WeatherForecastCreated"/>.
    /// </summary>
    public sealed class WeatherForecastCreatedEvent_DomainEventHandler : IDomainEventHandler<WeatherForecastCreated>
    {
        private readonly ILogger<WeatherForecastCreatedEvent_DomainEventHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastDefinitionCreatedEvent_DomainEventHandler"/> class.
        /// </summary>
        /// <param name="logger"></param>
        public WeatherForecastCreatedEvent_DomainEventHandler(ILogger<WeatherForecastCreatedEvent_DomainEventHandler> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(WeatherForecastCreated domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Weather forecast created: {Id} with Weather forecast definition id {DefinitionId}", domainEvent.DefinitionId, domainEvent.DefinitionId);
            await Task.CompletedTask;
        }
    }
}
