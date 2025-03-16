namespace Meteo.Infrastructure.EventHandlers.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Application.Events;
    using Library.Application.Interfaces;
    using Library.Buses.Handlers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using FluentAssertions;
    using Meteo.Infrastructure.EventHandlers;

    /// <summary>
    /// Unit tests for EmailEventHandler.
    /// </summary>
    public class EmailEventHandlerTests
    {
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<EmailEventHandler>> _mockLogger;
        private readonly EmailEventHandler _handler;

        /// <summary>
        /// Initializes a new instance of the EmailEventHandlerTests class.
        /// </summary>
        public EmailEventHandlerTests()
        {
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<EmailEventHandler>>();
            _handler = new EmailEventHandler(_mockEmailService.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Tests handling an email event successfully.
        /// </summary>
        [Fact]
        public async Task HandleAsync_Should_Send_Email_Successfully()
        {
            var emailEvent = new EmailOutboxEvent("recipient@example.com", "Test Subject", "Test Body");
            var cancellationToken = CancellationToken.None;

            _mockEmailService.Setup(e => e.SendEmailAsync(emailEvent.To, emailEvent.Subject, emailEvent.Body, cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _handler.HandleAsync(emailEvent, cancellationToken);

            _mockEmailService.Verify(e => e.SendEmailAsync(emailEvent.To, emailEvent.Subject, emailEvent.Body, cancellationToken), Times.Once);
        }
    }
}