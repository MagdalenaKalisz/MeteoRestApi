namespace Library.Persistence
{
    using Library.Domain.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Represents a base class for a unit of work.
    /// </summary>
    public abstract class DatabaseSession : IDatabaseSession
    {
        private IDbContextTransaction? _transaction;

        /// <summary>
        /// The database context.
        /// </summary>
        protected DbContext Context { get; }

        /// <summary>
        /// The transaction.
        /// </summary>
        protected IDbContextTransaction Transaction => _transaction ?? throw new InvalidOperationException("Transaction has not been started.");

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSession"/> class.
        /// </summary>
        /// <param name="context"></param>
        protected DatabaseSession(DbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            Context = context;
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task BeginTransactionAsync()
        {
            _transaction = await Context.Database.BeginTransactionAsync();
        }

        /// <inheritdoc/>
        public async Task CommitTransactionAsync()
        {
            try
            {
                await Context.SaveChangesAsync();
                _transaction?.Commit();
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_transaction is not null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            finally
            {
                if (_transaction is not null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }
    }
}
