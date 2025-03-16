namespace Meteo.Infrastructure.Services
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Application.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a service for sending emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="cancellationToken"></param>
        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                if (!int.TryParse(_configuration["EmailSettings:Port"], out int port))
                {
                    port = 587; // Default SMTP port (change based on your provider)
                }

                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                if (string.IsNullOrWhiteSpace(senderEmail))
                {
                    throw new InvalidOperationException("Sender email is missing in configuration.");
                }

                var senderName = _configuration["EmailSettings:SenderName"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                if (!bool.TryParse(_configuration["EmailSettings:UseSsl"], out bool useSsl))
                {
                    useSsl = true;
                }

                if (string.IsNullOrWhiteSpace(smtpServer) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException("SMTP settings are missing in configuration.");
                }

                using var client = new SmtpClient(smtpServer, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = useSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage, cancellationToken);

                _logger.LogInformation("Email sent successfully to {Recipient}", senderEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", _configuration["EmailSettings:SenderName"]); // to change
                throw;
            }
        }
    }
}
