namespace Library.Buses
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommand : IRequest
    {

    }

    /// <summary>
    /// Represents a command with a known result type.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ICommand<out TResult> : ICommand, IRequest<TResult>
    {

    }
}
