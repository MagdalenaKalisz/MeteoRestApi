namespace Library.Domain
{
    /// <summary>
    /// Base class for entities.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public abstract class Entity<TId> : IEntity<TId>
        where TId : ValueObject, IId
    {
        /// <inheritdoc/>
        public TId Id { get; }

        /// <summary>
        /// The entity constructor.
        /// </summary>
        /// <param name="id"></param>
        protected Entity(TId id)
        {
            ArgumentNullException.ThrowIfNull(id);
            Id = id;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other)
            {
                return false;
            }

            return EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }

        /// <summary>
        /// Determines whether two specified instances of Entity are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified instances of Entity are not equal
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        {
            return !(left == right);
        }
    }
}
