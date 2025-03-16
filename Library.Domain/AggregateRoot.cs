namespace Library.Domain
{
    using System.Collections.Generic;

    using Domain.ValueObjects;

    /// <summary>
    /// Base class for all aggregate roots.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId>,
                                               IAggregateRoot<TId>
        where TId : ValueObject, IId
    {
        private List<IDomainEvent> _domainEvents = [];

        /// <inheritdoc/>
        public virtual CreationTimestamp CreatedAt { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{TId}"/> class.
        /// </summary>
        protected AggregateRoot(TId id, CreationTimestamp createdAt)
            : base(id)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(createdAt);

            CreatedAt = createdAt;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        /// <summary>
        /// Add a domain event to the aggregate root.
        /// </summary>
        /// <param name="domainEvent"></param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);

            _domainEvents.Add(domainEvent);
        }

        /// <inheritdoc/>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IDomainEvent> GetAndClearDomainEvents()
        {
            List<IDomainEvent> result = _domainEvents;
            _domainEvents = [];

            return result;
        }
    }
}
