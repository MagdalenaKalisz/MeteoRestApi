namespace Meteo.Infrastructure.EventHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Application.Events;
    using Library.Application.Interfaces;
    using Library.Buses.Handlers;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Subscriber for handling email events.
    /// </summary>
    public class EmailEventHandler : IEventHandler<EmailOutboxEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailEventHandler> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmailEventHandler(IEmailService emailService, ILogger<EmailEventHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task HandleAsync(EmailOutboxEvent emailEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing email event for: {To}", emailEvent.To);

            try
            {
                await _emailService.SendEmailAsync(emailEvent.To, emailEvent.Subject, emailEvent.Body, cancellationToken);
                _logger.LogInformation("Email sent to: {To}", emailEvent.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to: {To}", emailEvent.To);
            }
        }
    }
}
