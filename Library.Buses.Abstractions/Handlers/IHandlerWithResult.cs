namespace Library.Buses.Handlers
{
    /// <summary>
    /// Represents a handler with return value.
    /// </summary>
    public interface IHandlerWithResult
    {
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<object?> HandleAsync(object request, CancellationToken cancellationToken);
    }
}
