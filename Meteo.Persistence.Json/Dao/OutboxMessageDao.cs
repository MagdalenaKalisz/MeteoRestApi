namespace Meteo.Persistence.Json.Dao
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents an Outbox message stored in JSON.
    /// </summary>
    public class OutboxMessageDao
    {
        /// <summary>
        /// Gets or sets the unique identifier of the outbox message.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the event type (fully qualified class name).
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the serialized JSON payload of the event.
        /// </summary>
        public string Payload { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp when the event occurred.
        /// </summary>
        public DateTimeOffset OccurredAt { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the event was processed. (Can be null if unprocessed.)
        /// </summary>
        public DateTimeOffset? ProcessedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxMessageDao"/> class.
        /// Required for JSON deserialization.
        /// </summary>
        [JsonConstructor]
        public OutboxMessageDao(Guid id, string type, string payload, DateTimeOffset occurredAt, DateTimeOffset? processedAt)
        {
            Id = id;
            Type = type;
            Payload = payload;
            OccurredAt = occurredAt;
            ProcessedAt = processedAt;
        }
    }
}
