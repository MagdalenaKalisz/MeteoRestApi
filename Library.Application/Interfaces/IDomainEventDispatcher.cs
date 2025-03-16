namespace Library.Application.Interfaces
{
    using Library.Domain;

    /// <summary>
    /// Represents a domain event dispatcher.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Dispatches the domain events of the specified aggregate roots and saves the changes.
        /// </summary>
        /// <param name="continuation"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="aggregateRoots"></param>
        /// <returns></returns>
        Task DispatchAndSaveChangesAsync(Func<CancellationToken, Task> continuation,
                                         CancellationToken cancellationToken = default,
                                         params IEnumerable<IAggregateRoot> aggregateRoots);
    }
}
