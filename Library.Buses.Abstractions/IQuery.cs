namespace Library.Buses
{
    /// <summary>
    /// Represents a query.
    /// </summary>
    public interface IQuery : IRequest
    {

    }

    /// <summary>
    /// Represents a command with a known result type.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IQuery<out TResult> : IQuery, IRequest<TResult>
    {

    }
}
