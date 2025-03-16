namespace Library.Domain.Database;
using Library.Domain;

/// <summary>
/// Represents a database collection.
/// This interface acts as a base interface for all database collections.
/// It can be thought of as a place where business requirements of what can be done with a collection of entities are defined.
/// It is a one step closer to the specification pattern usage.
/// </summary>
/// <typeparam name="TAggregateRoot"></typeparam>
/// <typeparam name="TId"></typeparam>
public interface IDatabaseCollection<TAggregateRoot, TId>
    where TAggregateRoot : AggregateRoot<TId>
    where TId : ValueObject, IId
{
    /// <summary>
    /// Get an entity by ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="queryBehavior"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TAggregateRoot?> GetByIdAsync(TId id,
                                       QueryBehavior queryBehavior = QueryBehavior.Default,
                                       CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities.
    /// </summary>
    /// <param name="queryBehavior"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<TAggregateRoot>> GetAllAsync(QueryBehavior queryBehavior = QueryBehavior.Default,
                                           CancellationToken cancellationToken = default);

    /// <summary>
    /// Add an entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(TAggregateRoot entity,
                  CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an entity that is tracked.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(TAggregateRoot entity,
                     CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an entity that is tracked.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(TAggregateRoot entity,
                     CancellationToken cancellationToken = default);
}
