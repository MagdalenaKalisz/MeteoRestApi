namespace Library.Buses
{
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Buses.Handlers;

    /// <summary>
    /// Default implementation of <see cref="IBus"/>.
    /// </summary>
    public sealed class Bus : IBus
    {
        private readonly IBusHandlerResolver _handlerResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bus"/> class.
        /// </summary>
        /// <param name="handlerResolver"></param>
        public Bus(IBusHandlerResolver handlerResolver)
        {
            _handlerResolver = handlerResolver;
        }

        /// <inheritdoc/>
        public async Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            object? result = await ExecuteAsync(request as object, cancellationToken);

            return (TResult)result!;
        }

        /// <inheritdoc/>
        public async Task<object?> ExecuteAsync(object request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            cancellationToken.ThrowIfCancellationRequested();

            IHandlerWithResult handler = (IHandlerWithResult)_handlerResolver
                .ResolveHandlers(request.GetType())
                .First();

            object? result = await handler.HandleAsync(request, cancellationToken);

            return result!;
        }

        /// <inheritdoc/>
        public async Task PublishAsync(object notification, EventPublishingTarget target, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(notification);

            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<IHandlerWithoutResult> handlers = _handlerResolver
                .ResolveHandlers(notification.GetType())
                .Cast<IHandlerWithoutResult>();

            foreach (IHandlerWithoutResult handler in handlers)
            {
                await handler.HandleAsync(notification, cancellationToken);
            }
        }
    }
}
