namespace Library.Application.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service for managing outbox events.
    /// </summary>
    public interface IOutbox
    {
        /// <summary>
        /// Saves an event to the outbox.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        Task SaveEventAsync(object @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes outbox messages.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default);
    }
}
