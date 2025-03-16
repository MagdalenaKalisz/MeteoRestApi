namespace Library.Buses
{
    /// <summary>
    /// Event publishing target.
    /// </summary>
    public enum EventPublishingTarget
    {
        /// <summary>
        /// Publishes the notification as domain event (i.e. event handled before the transaction is committed).
        /// The event is not guaranteed to be delivered.
        /// </summary>
        DomainEvent,

        /// <summary>
        /// Publishes the notification as event (i.e. event handled after the transaction is committed).
        /// The event is not guaranteed to be delivered.
        /// </summary>
        Event,

        /// <summary>
        /// Publishes the notification as persisted event (i.e. event dispatched from outbox after the transaction is committed).
        /// The events from the outbox are dispatched here.
        /// The event is guaranteed to be delivered at least once.
        /// </summary>
        PersistedEvent,
    }
}