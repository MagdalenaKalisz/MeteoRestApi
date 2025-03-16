namespace Library.Buses.Handlers
{
    using Library.Buses;

    /// <summary>
    /// Represents a query handler.
    /// </summary>
    public interface IQueryHandler : IHandlerWithResult
    {

    }

    /// <summary>
    /// Represents a query handler.
    /// </summary>
    public interface IQueryHandler<TQuery, TResult> : IQueryHandler
        where TQuery : IQuery<TResult>
    {
        async Task<object?> IHandlerWithResult.HandleAsync(object request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await HandleAsync((TQuery)request, cancellationToken);
        }

        /// <summary>
        /// Handles the query.    
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
    }
}
