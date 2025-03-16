namespace Meteo.Persistence.PostgreSql.Collections
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Aggregates;
    using Domain.Database;
    using Domain.ValueObjects;
    using Library.Domain.Database;
    using Library.Persistence;
    using Meteo.Persistence.PostgreSql;
    using Meteo.Persistence.PostgreSql.Dao;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// A repository for <see cref="WeatherForecastDefinition"/>.
    /// </summary>
    public sealed class WeatherForecastDefinitionCollection : DatabaseCollection<WeatherForecastDefinition, WeatherForecastDefinitionId, WeatherForecastDefinitionDao>,
                                                              IWeatherForecastDefinitionCollection
    {
        /// <summary>
        /// Database context.
        /// </summary>
        private new ApplicationDbContext Context { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WeatherForecastDefinitionCollection"/>.
        /// </summary>
        /// <param name="context"></param>
        public WeatherForecastDefinitionCollection(ApplicationDbContext context) :
            base(context)
        {
            Context = context;
        }

        /// <inheritdoc/>
        public async Task<WeatherForecastDefinition?> GetByCoordinatesAsync(Coordinates coordinates,
                                                                            QueryBehavior queryBehavior = QueryBehavior.Default,
                                                                            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(coordinates);

            double latitude = coordinates.Latitude;
            double longitude = coordinates.Longitude;

            IQueryable<WeatherForecastDefinitionDao> query = BuildQueryable(queryBehavior);

            WeatherForecastDefinitionDao? result = await query
                .FirstOrDefaultAsync(x => x.Latitude == latitude && x.Longitude == longitude, cancellationToken);

            return result is null
                ? null
                : MapToDomain(result);
        }

        /// <inheritdoc/>
        protected override IQueryable<WeatherForecastDefinitionDao> AddDefaultsToQuery(IQueryable<WeatherForecastDefinitionDao> query)
        {
            return query;
        }

        /// <inheritdoc/>
        protected override void SyncDao(WeatherForecastDefinitionDao dao, WeatherForecastDefinition entity)
        {
            dao.Id = entity.Id.Value;
            dao.CreatedAt = entity.CreatedAt;
            dao.Latitude = entity.Coordinates.Latitude;
            dao.Longitude = entity.Coordinates.Longitude;
        }

        /// <inheritdoc/>
        protected override WeatherForecastDefinitionDao MapToDao(WeatherForecastDefinition entity)
        {
            return new WeatherForecastDefinitionDao
            {
                Id = entity.Id.Value,
                CreatedAt = entity.CreatedAt,
                Latitude = entity.Coordinates.Latitude,
                Longitude = entity.Coordinates.Longitude,
            };
        }

        /// <inheritdoc/>
        protected override WeatherForecastDefinition MapToDomain(WeatherForecastDefinitionDao dao)
        {
            WeatherForecastDefinition result = WeatherForecastDefinition.Create(
                dao.Id,
                dao.CreatedAt,
                Coordinates.Create(dao.Latitude, dao.Longitude)
            );

            result.ClearDomainEvents();

            return result;
        }

        /// <inheritdoc/>
        public override async Task<WeatherForecastDefinition?> GetByIdAsync(
                   WeatherForecastDefinitionId id,
                   QueryBehavior queryBehavior = QueryBehavior.Default,
                   CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);

            IQueryable<WeatherForecastDefinitionDao> query = BuildQueryable(queryBehavior);

            WeatherForecastDefinitionDao? dao = await query
                .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

            return dao is null ? null : MapToDomain(dao);
        }

        /// <inheritdoc/>
        public override async Task<List<WeatherForecastDefinition>> GetAllAsync(
            QueryBehavior queryBehavior = QueryBehavior.Default,
            CancellationToken cancellationToken = default)
        {
            IQueryable<WeatherForecastDefinitionDao> query = BuildQueryable(queryBehavior);

            List<WeatherForecastDefinitionDao> daos = await query
                .ToListAsync(cancellationToken);

            return daos.ConvertAll(MapToDomain);
        }

        /// <inheritdoc/>
        public override async Task AddAsync(WeatherForecastDefinition entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await base.AddAsync(entity, cancellationToken);

            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task UpdateAsync(WeatherForecastDefinition entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await base.UpdateAsync(entity, cancellationToken);

            await Context.SaveChangesAsync(cancellationToken);

        }

        /// <inheritdoc/>
        public override async Task DeleteAsync(WeatherForecastDefinition entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await base.DeleteAsync(entity, cancellationToken);

            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
