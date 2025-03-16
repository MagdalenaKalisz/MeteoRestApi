namespace Meteo.Persistence.Json.Services
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Application.Events;
    using Library.Application.Interfaces;
    using Library.Buses;
    using Library.Identifiers;
    using Library.Persistence;
    using Library.Persistence.Dao;

    using Meteo.Domain.Events;
    using Meteo.Persistence.Json.Repositories;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a service for managing the outbox.
    /// </summary>
    public sealed class JsonOutbox : IOutbox
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly IBus _bus;
        private readonly TimeProvider _timeProvider;
        private readonly IdentifierProvider _identifierProvider;
        private readonly ILogger<JsonOutbox> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonOutbox"/> class.
        /// </summary>
        /// <param name="outboxRepository"></param>
        /// <param name="bus"></param>
        /// <param name="timeProvider"></param>
        /// <param name="identifierProvider"></param>
        /// <param name="logger"></param>
        public JsonOutbox(
            IOutboxRepository outboxRepository,
            IBus bus,
            TimeProvider timeProvider,
            IdentifierProvider identifierProvider,
            ILogger<JsonOutbox> logger)
        {
            _outboxRepository = outboxRepository;
            _bus = bus;
            _timeProvider = timeProvider;
            _identifierProvider = identifierProvider;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task SaveEventAsync(object @event, CancellationToken cancellationToken = default)
        {
            string eventType = @event.GetType().FullName!;

            OutboxMessage message = new()
            {
                Id = _identifierProvider.CreateSequentialUuid(),
                OccurredAt = _timeProvider.GetUtcNow(),
                Type = eventType,
                Payload = OutboxRepository.SerializeEvent(@event, cancellationToken), // Delegate serialization to repository
            };

            await _outboxRepository.SaveOutboxMessageAsync(message, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default)
        {
            List<OutboxMessage> messages = await _outboxRepository.GetUnprocessedMessagesAsync(cancellationToken);

            foreach (OutboxMessage message in messages)
            {
                try
                {
                    Type? eventType = AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.FullName == message.Type);
                    

                    if (eventType == typeof(EmailOutboxEvent))
                    {
                        var emailEvent = JsonSerializer.Deserialize<EmailOutboxEvent>(message.Payload);
                        if (emailEvent != null)
                        {
                            await _bus.PublishAsync(emailEvent, target: EventPublishingTarget.PersistedEvent, cancellationToken);
                        }
                    }

                    else if (eventType != null)
                    {
                        object? @event = OutboxRepository.DeserializeEvent(message.Payload, eventType, cancellationToken); // Delegate deserialization to repository

                        if (@event != null)
                        {
                            await _bus.PublishAsync(@event, target: EventPublishingTarget.PersistedEvent, cancellationToken);
                        }
                    }

                    await _outboxRepository.MarkMessageAsProcessedAsync(message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox event {EventType}", message.Type);
                }
            }
        }
    }
}
