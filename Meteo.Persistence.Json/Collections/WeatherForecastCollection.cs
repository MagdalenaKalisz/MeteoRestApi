namespace Meteo.Persistence.Json.Collections
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Domain.Database;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Database;
    using Meteo.Domain.Enums;
    using Meteo.Domain.ValueObjects;
    using Meteo.Persistence.Json.Dao;

    /// <summary>
    /// JSON-based repository for weather forecasts.
    /// </summary>
    public sealed class WeatherForecastCollection : IWeatherForecastCollection
    {
        private readonly JsonDatabase<WeatherForecastDao> _database;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath"></param>
        public WeatherForecastCollection(string filePath)
        {
            _database = new JsonDatabase<WeatherForecastDao>(filePath);
        }

        /// <summary>
        /// Retrieves a weather forecast by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="queryBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WeatherForecast?> GetByIdAsync(WeatherForecastId id, QueryBehavior queryBehavior = QueryBehavior.Default, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDao> forecasts = await _database.ReadAsync();
            WeatherForecastDao? dao = forecasts.FirstOrDefault(x => x.Id == id.Value);
            return dao is null ? null : MapToDomain(dao);
        }

        /// <summary>
        /// Retrieves all weather forecasts.
        /// </summary>
        /// <param name="queryBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<WeatherForecast>> GetAllAsync(QueryBehavior queryBehavior = QueryBehavior.Default, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDao> forecasts = await _database.ReadAsync();
            return forecasts.ConvertAll(MapToDomain);
        }

        /// <summary>
        /// Adds a new weather forecast.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task AddAsync(WeatherForecast entity, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDao> forecasts = await _database.ReadAsync();
            forecasts.Add(MapToDao(entity));
            await _database.WriteAsync(forecasts);
        }

        /// <summary>
        /// Updates an existing weather forecast.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task UpdateAsync(WeatherForecast entity, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDao> forecasts = await _database.ReadAsync();
            int index = forecasts.FindIndex(x => x.Id == entity.Id.Value);
            if (index != -1)
            {
                forecasts[index] = MapToDao(entity);
                await _database.WriteAsync(forecasts);
            }
        }

        /// <summary>
        /// Deletes a weather forecast.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task DeleteAsync(WeatherForecast entity, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDao> forecasts = await _database.ReadAsync();
            forecasts.RemoveAll(x => x.Id == entity.Id.Value);
            await _database.WriteAsync(forecasts);
        }

        /// <summary>
        /// Retrieves a weather forecast by definition ID.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="queryBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WeatherForecast?> GetByDefinitionIdAsync(WeatherForecastDefinitionId definitionId, QueryBehavior queryBehavior = QueryBehavior.Default, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDao> forecasts = await _database.ReadAsync();
            WeatherForecastDao? dao = forecasts.FirstOrDefault(x => x.DefinitionId == definitionId.Value);
            return dao is null ? null : MapToDomain(dao);
        }

        /// <summary>
        /// Maps domain model to DAO.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static WeatherForecastDao MapToDao(WeatherForecast entity)
        {
            return new WeatherForecastDao(
                entity.Id.Value,
                entity.CreatedAt,
                entity.DefinitionId.Value,
                entity.Forecasts.Select(f => new WeatherForecastForDayDao(
                    f.ForecastDateTime.DateTime,
                    f.Temperature.Value,
                    f.Humidity.Percentage)).ToList());
        }

        /// <summary>
        /// Maps DAO to domain model.
        /// </summary>
        /// <param name="dao"></param>
        /// <returns></returns>
        private static WeatherForecast MapToDomain(WeatherForecastDao dao)
        {
            return WeatherForecast.Create(
                dao.Id,
                dao.CreatedAt,
                dao.DefinitionId,
                dao.Forecasts.Select(f =>
                    WeatherForecastForDay.Create(
                        ForecastDateTime.Create(f.ForecastDateTime),
                        Temperature.Create(f.Temperature, TemperatureUnit.Celsius),
                        f.Humidity)
                ));
        }
    }
}
