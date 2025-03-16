namespace Library.Buses.Handlers
{
    /// <summary>
    /// Represents a handler without a return value.
    /// </summary>
    public interface IHandlerWithoutResult
    {
        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleAsync(object @event, CancellationToken cancellationToken);
    }
}
