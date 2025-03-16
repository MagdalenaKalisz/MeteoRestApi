namespace Library.Buses
{
    /// <summary>
    /// Represents a request.
    /// </summary>
    public interface IRequest
    {

    }

    /// <summary>
    /// Represents a request with a result.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IRequest<out TResult> : IRequest
    {

    }
}