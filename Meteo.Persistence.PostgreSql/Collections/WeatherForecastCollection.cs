namespace Meteo.Persistence.PostgreSql.Collections
{
    using System.Threading;
    using System.Threading.Tasks;
    using Domain.Aggregates;
    using Domain.Database;
    using Domain.Enums;
    using Domain.ValueObjects;
    using Library.Domain.Database;
    using Library.Identifiers;
    using Library.Persistence;
    using Meteo.Persistence.PostgreSql;
    using Meteo.Persistence.PostgreSql.Dao;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// A repository for <see cref="WeatherForecast"/>.
    /// </summary>
    public sealed class WeatherForecastCollection : DatabaseCollection<WeatherForecast, WeatherForecastId, WeatherForecastDao>,
                                                    IWeatherForecastCollection
    {
        private readonly IdentifierProvider _identifierProvider;

        /// <summary>
        /// Database context.
        /// </summary>
        private new ApplicationDbContext Context { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WeatherForecastCollection"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="identifierProvider"></param>
        public WeatherForecastCollection(ApplicationDbContext context,
                                         IdentifierProvider identifierProvider) :
            base(context)
        {
            Context = context;
            _identifierProvider = identifierProvider;
        }

        /// <inheritdoc/>
        protected override IQueryable<WeatherForecastDao> AddDefaultsToQuery(IQueryable<WeatherForecastDao> query)
        {
            return query
                .Include(static x => x.Forecasts);
        }

        /// <inheritdoc/>
        protected override void SyncDao(WeatherForecastDao dao, WeatherForecast entity)
        {
            dao.Id = entity.Id;
            dao.CreatedAt = entity.CreatedAt;
            dao.DefinitionId = entity.DefinitionId.Value;

            dao.Forecasts = [];
            foreach (WeatherForecastForDay forecast in entity.Forecasts)
            {
                dao.Forecasts.Add(new WeatherForecastForDayDao
                {
                    Id = _identifierProvider.CreateSequentialUuid(),
                    WeatherForecastId = entity.Id.Value,
                    ForecastDateTime = forecast.ForecastDateTime.DateTime,
                    Temperature = forecast.Temperature.Value,
                    Humidity = forecast.Humidity.Percentage,
                });
            }
        }

        /// <inheritdoc/>
        protected override WeatherForecastDao MapToDao(WeatherForecast entity)
        {
            return new WeatherForecastDao
            {
                Id = entity.Id.Value,
                CreatedAt = entity.CreatedAt,
                DefinitionId = entity.DefinitionId.Value,
                Forecasts = [.. entity.Forecasts.Select(f => new WeatherForecastForDayDao
                {
                    Id = _identifierProvider.CreateSequentialUuid(),
                    WeatherForecastId = entity.Id.Value,
                    ForecastDateTime = f.ForecastDateTime.DateTime,
                    Temperature = f.Temperature.Value,
                    Humidity = f.Humidity.Percentage,
                }),],
            };
        }

        /// <inheritdoc/>
        protected override WeatherForecast MapToDomain(WeatherForecastDao dao)
        {
            IEnumerable<WeatherForecastForDay> forecast = dao
                .Forecasts
                ?.Select(static forecast =>
                {
                    return WeatherForecastForDay.Create(
                        ForecastDateTime.Create(forecast.ForecastDateTime),
                        Temperature.Create(forecast.Temperature, TemperatureUnit.Celsius),
                        forecast.Humidity
                    );
                })
                .ToList() ?? [];

            WeatherForecast result = WeatherForecast.Create(
                dao.Id,
                dao.CreatedAt,
                dao.DefinitionId,
                forecast);

            result.ClearDomainEvents();

            return result;
        }

        /// <inheritdoc/>
        public override async Task<WeatherForecast?> GetByIdAsync(
                WeatherForecastId id,
                QueryBehavior queryBehavior = QueryBehavior.Default,
                CancellationToken cancellationToken = default
            )
        {
            ArgumentNullException.ThrowIfNull(id);

            IQueryable<WeatherForecastDao> query = BuildQueryable(queryBehavior);

            WeatherForecastDao? dao = await query
                .FirstOrDefaultAsync(x => x.Id == id.Value, cancellationToken);

            return dao is null ? null : MapToDomain(dao);
        }

        /// <inheritdoc/>
        public async Task<WeatherForecast?> GetByDefinitionIdAsync(
                WeatherForecastDefinitionId id,
                QueryBehavior queryBehavior = QueryBehavior.Default,
                CancellationToken cancellationToken = default
            )
        {
            ArgumentNullException.ThrowIfNull(id);

            IQueryable<WeatherForecastDao> query = BuildQueryable(queryBehavior);

            WeatherForecastDao? dao = await query
                .FirstOrDefaultAsync(x => x.DefinitionId == id.Value, cancellationToken);

            return dao is null ? null : MapToDomain(dao);
        }

        /// <inheritdoc/>
        public override async Task<List<WeatherForecast>> GetAllAsync(
            QueryBehavior queryBehavior = QueryBehavior.Default,
            CancellationToken cancellationToken = default
        )
        {
            IQueryable<WeatherForecastDao> query = BuildQueryable(queryBehavior);

            List<WeatherForecastDao> daos = await query
                .ToListAsync(cancellationToken);

            return [.. daos.Select(MapToDomain)];
        }

        /// <inheritdoc/>
        public override async Task AddAsync(
            WeatherForecast entity,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(entity);
            await base.AddAsync(entity, cancellationToken);

            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public override async Task UpdateAsync(
            WeatherForecast entity,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(entity);
            await base.UpdateAsync(entity, cancellationToken);

            await Context.SaveChangesAsync(cancellationToken);

        }

        /// <inheritdoc/>
        public override async Task DeleteAsync(
            WeatherForecast entity,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(entity);
            await base.DeleteAsync(entity, cancellationToken);

            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}