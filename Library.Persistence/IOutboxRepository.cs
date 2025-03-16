namespace Library.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Persistence.Dao;

    /// <summary>
    /// Represents a repository for outbox messages.
    /// </summary>
    public interface IOutboxRepository
    {
        /// <summary>
        /// Gets unprocessed outbox messages.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks an outbox message as processed.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        Task MarkMessageAsProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves an outbox message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        Task SaveOutboxMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    }
}
