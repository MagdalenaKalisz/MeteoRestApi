namespace Library.Buses
{
    /// <summary>
    /// Represents a bus.
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// Executes the request (<see cref="ICommand{TResult}"/> or <see cref="IQuery{TResult}"/>).  
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> ExecuteAsync<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the request (<see cref="ICommand{TResult}"/> or <see cref="IQuery{TResult}"/>).  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<object?> ExecuteAsync(object request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes the notification to the specified target.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="target"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PublishAsync(object notification, EventPublishingTarget target, CancellationToken cancellationToken = default);
    }
}
