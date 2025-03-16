namespace Library.Application.Events
{
    /// <summary>
    /// Represents an email event stored in the outbox.
    /// </summary>
    public class EmailOutboxEvent
    {
        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string To { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the body of the email.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailOutboxEvent"/> class.
        /// </summary>
        public EmailOutboxEvent(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
