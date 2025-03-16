namespace Library.Domain
{
    /// <summary>
    /// Represents an identifier.
    /// </summary>
    public interface IId
    {
        /// <summary>
        /// The value of the identifier.
        /// </summary>
        Guid Value { get; }
    }
}