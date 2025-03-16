namespace Library.Persistence
{
    /// <summary>
    /// Represents a data access object with identifier.
    /// </summary>
    public interface IDaoWithId : IDao
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        Guid Id { get; set; }
    }
}
