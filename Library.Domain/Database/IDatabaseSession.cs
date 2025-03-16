namespace Library.Domain.Database
{
    /// <summary>
    /// Represents a database session (unit of work).
    /// </summary>
    public interface IDatabaseSession
    {
        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns></returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <returns></returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        /// <returns></returns>
        Task RollbackTransactionAsync();
    }
}
