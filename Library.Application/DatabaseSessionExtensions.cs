namespace Library.Application
{
    using Library.Application.Interfaces;
    using Library.Domain;
    using Library.Domain.Database;

    /// <summary>
    /// <see cref="IDatabaseSession"/> extensions.  
    /// </summary>
    public static class DatabaseSessionExtensions
    {
        /// <summary>
        /// Dispatches the domain events of the specified aggregate roots and saves the changes.
        /// </summary>
        /// <param name="databaseSession"></param>
        /// <param name="domainEventDispatcher"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="aggregateRoots"></param>
        /// <returns></returns>
        public static Task DispatchAndSaveChangesAsync(this IDatabaseSession databaseSession,
                                                       IDomainEventDispatcher domainEventDispatcher,
                                                       CancellationToken cancellationToken = default,
                                                       params IEnumerable<IAggregateRoot> aggregateRoots)
        {
            ArgumentNullException.ThrowIfNull(databaseSession);
            ArgumentNullException.ThrowIfNull(domainEventDispatcher);
            ArgumentNullException.ThrowIfNull(aggregateRoots);

            return domainEventDispatcher.DispatchAndSaveChangesAsync(
                databaseSession.SaveChangesAsync,
                cancellationToken,
                aggregateRoots);
        }
    }
}
