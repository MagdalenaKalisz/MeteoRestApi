namespace Library.Extensions
{
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Extended background service.
    /// </summary>
    public abstract class ExtendedBackgroundService : BackgroundService
    {
        private readonly SemaphoreSlim _startupLock = new(1, 1);

        private readonly IHostApplicationLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of <see cref="ExtendedBackgroundService"/>.
        /// </summary>
        /// <param name="lifetime"></param>
        protected ExtendedBackgroundService(IHostApplicationLifetime lifetime)
        {
            ArgumentNullException.ThrowIfNull(lifetime);

            _lifetime = lifetime;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            await ExecuteImmediatelyAsync(stoppingToken);

            await _startupLock.WaitAsync(stoppingToken);

            _lifetime.ApplicationStarted.Register(() =>
            {
                if (_startupLock.CurrentCount == 0)
                {
                    _startupLock.Release();
                }
            });

            await _startupLock.WaitAsync(stoppingToken);

            if (stoppingToken.IsCancellationRequested ||
                _startupLock.CurrentCount != 0)
            {
                _startupLock.Release();
            }

            await ExecuteAfterStartupAsync(stoppingToken);
        }

        /// <summary>
        /// This method is called when <see cref="IHostedService"/> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="cancellationToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract Task ExecuteImmediatelyAsync(CancellationToken cancellationToken);

        /// <summary>
        /// This method is called after the <see cref="IHostedService"/> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="cancellationToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
        /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
        protected abstract Task ExecuteAfterStartupAsync(CancellationToken cancellationToken);

        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();

            GC.SuppressFinalize(this);

            _startupLock.Dispose();
        }
    }
}
