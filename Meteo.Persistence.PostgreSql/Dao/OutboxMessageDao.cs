namespace Meteo.Persistence.PostgreSql.Dao
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents an Outbox message stored in PostgreSQL.
    /// </summary>
    [Table("OutboxMessages")]
    public class OutboxMessageDao
    {
        /// <summary>
        /// Gets or sets the unique identifier of the outbox message.
        /// </summary>
        [Key]
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
        /// Parameterless constructor required by Entity Framework.
        /// </summary>
        public OutboxMessageDao() { }

        /// <summary>
        /// Constructor for initializing the OutboxMessageDao with required properties.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="payload"></param>
        /// <param name="occurredAt"></param>
        /// <param name="processedAt"></param>
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
