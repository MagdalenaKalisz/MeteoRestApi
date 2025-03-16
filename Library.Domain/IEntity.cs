namespace Library.Domain
{
    /// <summary>
    /// Represents an entity.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The entity identifier.
        /// </summary>
        object Id { get; }
    }

    /// <summary>
    /// Represents an entity.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IEntity<TId> : IEntity
        where TId : ValueObject, IId
    {
        /// <summary>
        /// The entity identifier.
        /// </summary>
        new TId Id { get; }

        object IEntity.Id => Id.Value;
    }
}