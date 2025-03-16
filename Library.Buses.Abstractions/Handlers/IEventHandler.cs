namespace Library.Buses.Handlers
{
    /// <summary>
    /// Represents a event.
    /// </summary>
    public interface IEventHandler : IHandlerWithoutResult
    {

    }

    /// <summary>
    /// Represents a event.
    /// </summary>
    public interface IEventHandler<TEvent> : IEventHandler
    {
        Task IHandlerWithoutResult.HandleAsync(object request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return HandleAsync((TEvent)request, cancellationToken);
        }

        /// <summary>
        /// Handles the event.    
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}
