namespace Library.Application.Services
{
    using Library.Application.Interfaces;
    using Library.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a background service for processing outbox messages.
    /// </summary>
    public sealed class OutboxBackgroundProcessor : ExtendedBackgroundService
    {
        private static readonly TimeSpan _period = TimeSpan.FromSeconds(5);

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OutboxBackgroundProcessor> _logger;

        private readonly PeriodicTimer _periodicTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxBackgroundProcessor"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="hostApplicationLifetime"></param>
        /// <param name="logger"></param>
        public OutboxBackgroundProcessor(IServiceScopeFactory serviceScopeFactory,
                                         IHostApplicationLifetime hostApplicationLifetime,
                                         ILogger<OutboxBackgroundProcessor> logger) :
            base(hostApplicationLifetime)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _periodicTimer = new PeriodicTimer(_period);
        }

        /// <inheritdoc/>
        protected override Task ExecuteAfterStartupAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _periodicTimer.WaitForNextTickAsync(stoppingToken))
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        return;
                    }

                    await ProcessOutboxAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // swallow
            }
        }

        /// <inheritdoc/>
        protected override Task ExecuteImmediatelyAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
        {
            try
            {
                await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();

                IOutbox outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();

                await outbox.ProcessOutboxMessagesAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing outbox messages.");
            }
        }
    }
}
