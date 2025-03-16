namespace Library.Persistence
{
    using Library.Domain;
    using Library.Domain.Database;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a base class for repositories.
    /// </summary>
    /// <typeparam name="TAggregateRoot"></typeparam>
    /// <typeparam name="TAggregateRootId"></typeparam>
    /// <typeparam name="TDao"></typeparam>
    public abstract class DatabaseCollection<TAggregateRoot, TAggregateRootId, TDao>
        where TAggregateRoot : AggregateRoot<TAggregateRootId>
        where TAggregateRootId : ValueObject, IId
        where TDao : class, IDaoWithId, new()
    {
        /// <summary>
        /// The database context.
        /// </summary>
        protected DbContext Context { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DatabaseCollection{TAggregateRoot, TAggregateRootId, TDao}"/>.
        /// </summary>
        /// <param name="context"></param>
        protected DatabaseCollection(DbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            Context = context;
        }

        /// <inheritdoc/>
        public virtual async Task<TAggregateRoot?> GetByIdAsync(TAggregateRootId id,
                                                                QueryBehavior queryBehavior = QueryBehavior.Default,
                                                                CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);

            IQueryable<TDao> query = BuildQueryable(queryBehavior);
            TDao? result = await query.FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

            return result is null
                ? null
                : MapToDomain(result);
        }

        /// <inheritdoc/>
        public virtual async Task AddAsync(TAggregateRoot forecast,
                                           CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(forecast);

            TDao dao = MapToDao(forecast);
            await Context.Set<TDao>().AddAsync(dao, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<List<TAggregateRoot>> GetAllAsync(QueryBehavior queryBehavior = QueryBehavior.Default,
                                                                    CancellationToken cancellationToken = default)
        {
            IQueryable<TDao> query = BuildQueryable(queryBehavior);
            List<TDao> result = await query.ToListAsync(cancellationToken);

            return [.. result.Select(MapToDomain)];
        }

        /// <inheritdoc/>
        public virtual Task UpdateAsync(TAggregateRoot entity,
                                        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            TDao? dao = Context.ChangeTracker.Entries<TDao>()
                .Select(e => e.Entity)
                .FirstOrDefault(e => e.Id == entity.Id.Value);

            if (dao is null)
            {
                dao = MapToDao(entity);
                Context.Set<TDao>().Attach(dao);
            }
            else
            {
                SyncDao(dao, entity);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual Task DeleteAsync(TAggregateRoot entity,
                                        CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            TDao? dao = Context.ChangeTracker.Entries<TDao>()
                .Select(static x => x.Entity)
                .FirstOrDefault(x => x.Id == entity.Id.Value);

            if (dao is null)
            {
                dao = new TDao { Id = entity.Id.Value };
                Context.Set<TDao>().Attach(dao);
            }

            Context.Entry(dao).State = EntityState.Deleted;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Builds a queryable object.
        /// </summary>
        /// <param name="queryBehavior"></param>
        /// <returns></returns>
        protected IQueryable<TDao> BuildQueryable(QueryBehavior queryBehavior)
        {
            IQueryable<TDao> query = Context.Set<TDao>();

            if (queryBehavior.HasFlag(QueryBehavior.NoTracking))
            {
                query = query.AsNoTracking();
            }

            return AddDefaultsToQuery(query);
        }

        /// <summary>
        /// Adds default clauses to the query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected abstract IQueryable<TDao> AddDefaultsToQuery(IQueryable<TDao> query);

        /// <summary>
        /// Synchronizes the DAO object <typeparamref name="TAggregateRoot"/> with the domain object <typeparamref name="TDao"/>.
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="entity"></param>
        protected abstract void SyncDao(TDao dao, TAggregateRoot entity);

        /// <summary>
        /// Maps a domain object <typeparamref name="TAggregateRoot"/> to a DAO object <typeparamref name="TDao"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract TDao MapToDao(TAggregateRoot entity);

        /// <summary>
        /// Maps a DAO object <typeparamref name="TDao"/> to a domain object <typeparamref name="TAggregateRoot"/>.
        /// </summary>
        /// <param name="dao"></param>
        /// <returns></returns>
        protected abstract TAggregateRoot MapToDomain(TDao dao);
    }
}
