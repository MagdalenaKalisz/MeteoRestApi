namespace Library.Application.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a service for sending emails.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="to"></param>
        Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
}
