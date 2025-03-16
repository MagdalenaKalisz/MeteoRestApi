namespace Library.Buses.Handlers
{
    using Library.Buses;

    /// <summary>
    /// Represents a command handler.
    /// </summary>
    public interface ICommandHandler : IHandlerWithResult
    {

    }

    /// <summary>
    /// Represents a command handler.
    /// </summary>
    public interface ICommandHandler<TCommand, TResult> : ICommandHandler
        where TCommand : ICommand<TResult>
    {
        async Task<object?> IHandlerWithResult.HandleAsync(object request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            return await HandleAsync((TCommand)request, cancellationToken);
        }

        /// <summary>
        /// Handles the command.    
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
    }
}
