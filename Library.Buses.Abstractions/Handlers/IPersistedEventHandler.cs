namespace Library.Buses.Handlers
{
    /// <summary>
    /// Represents a persisted event handler.
    /// </summary>
    public interface IPersistedEventHandler : IHandlerWithoutResult
    {

    }

    /// <summary>
    /// Represents a persisted event handler.
    /// </summary>
    public interface IPersistedEventHandler<TEvent> : IPersistedEventHandler
    {
        Task IHandlerWithoutResult.HandleAsync(object request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return HandleAsync((TEvent)request, cancellationToken);
        }

        /// <summary>
        /// Handles the event.    
        /// </summary>
        /// <param name="persistedEvent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleAsync(TEvent persistedEvent, CancellationToken cancellationToken);
    }
}
