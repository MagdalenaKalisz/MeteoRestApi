namespace Library.Application.Services
{
    using Library.Application.Interfaces;
    using Library.Buses;
    using Library.Domain;

    /// <summary>
    /// Default implementation of the <see cref="IDomainEventDispatcher"/> interface.
    /// </summary>
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IOutbox _outbox;
        private readonly IBus _bus;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
        /// </summary>
        /// <param name="outbox"></param>
        /// <param name="bus"></param>
        public DomainEventDispatcher(IOutbox outbox,
                                     IBus bus)
        {
            _outbox = outbox;
            _bus = bus;
        }

        /// <inheritdoc/>
        public async Task DispatchAndSaveChangesAsync(Func<CancellationToken, Task> continuation,
                                                      CancellationToken cancellationToken = default,
                                                      params IEnumerable<IAggregateRoot> aggregateRoots)
        {
            ArgumentNullException.ThrowIfNull(continuation);
            ArgumentNullException.ThrowIfNull(aggregateRoots);

            IDomainEvent[] domainEvents = [.. aggregateRoots.SelectMany(x => x.GetAndClearDomainEvents())];

            if (domainEvents.Length == 0)
            {
                await continuation(cancellationToken);

                return;
            }

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                await _bus.PublishAsync(domainEvent, EventPublishingTarget.DomainEvent, cancellationToken);
            }

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                await _outbox.SaveEventAsync(domainEvent, cancellationToken);
            }

            await continuation(cancellationToken);

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                await _bus.PublishAsync(domainEvent, EventPublishingTarget.Event, cancellationToken);
            }
        }
    }
}
