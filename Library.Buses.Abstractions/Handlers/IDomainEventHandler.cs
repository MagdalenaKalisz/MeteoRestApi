namespace Library.Buses.Handlers
{
    /// <summary>
    /// Represents a domain event handler.
    /// </summary>
    public interface IDomainEventHandler : IHandlerWithoutResult
    {

    }

    /// <summary>
    /// Represents a event.
    /// </summary>
    public interface IDomainEventHandler<TEvent> : IDomainEventHandler
    {
        Task IHandlerWithoutResult.HandleAsync(object request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return HandleAsync((TEvent)request, cancellationToken);
        }

        /// <summary>
        /// Handles the event.    
        /// </summary>
        /// <param name="domainEvent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
    }
}
