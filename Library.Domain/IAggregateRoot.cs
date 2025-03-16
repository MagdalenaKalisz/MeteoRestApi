namespace Library.Domain
{
    using System.Collections.Generic;
    using Library.Domain.ValueObjects;

    /// <summary>
    /// Represents an aggregate root.
    /// </summary>
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Creation timestamp of the entity.
        /// </summary>
        CreationTimestamp CreatedAt { get; }

        /// <summary>
        /// The collection of domain events.
        /// </summary>
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Clears all domain events.
        /// </summary>
        void ClearDomainEvents();

        /// <summary>
        /// Clears all domain events.
        /// </summary>
        IReadOnlyCollection<IDomainEvent> GetAndClearDomainEvents();
    }

    /// <summary>
    /// Represents an aggregate root.
    /// </summary>
    public interface IAggregateRoot<TId> : IAggregateRoot, IEntity<TId>
        where TId : ValueObject, IId
    {

    }
}