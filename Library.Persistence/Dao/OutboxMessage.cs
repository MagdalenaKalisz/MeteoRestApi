namespace Library.Persistence.Dao
{
    using System;

    /// <summary>
    /// Represents an outbox message.
    /// </summary>
    public sealed class OutboxMessage
    {
        /// <summary>
        /// The unique identifier of the message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The timestamp when the event occurred.
        /// </summary>
        public DateTimeOffset OccurredAt { get; set; }

        /// <summary>
        /// The type of the event.
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// The serialized payload of the event.
        /// </summary>
        public string Payload { get; set; } = null!;

        /// <summary>
        /// The timestamp when the message was processed.
        /// </summary>
        public DateTimeOffset? ProcessedAt { get; set; }
    }
}
