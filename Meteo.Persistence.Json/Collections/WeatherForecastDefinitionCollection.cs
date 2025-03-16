namespace Meteo.Persistence.Json.Collections
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Domain.Database;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Database;
    using Meteo.Domain.ValueObjects;
    using Meteo.Persistence.Json.Dao;


    /// <summary>
    /// JSON-based repository for weather forecast definitions.
    /// </summary>
    public sealed class WeatherForecastDefinitionCollection : IWeatherForecastDefinitionCollection
    {
        private readonly JsonDatabase<WeatherForecastDefinitionDao> _database;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath"></param>
        public WeatherForecastDefinitionCollection(string filePath)
        {
            _database = new JsonDatabase<WeatherForecastDefinitionDao>(filePath);
        }

        /// <summary>
        /// Retrieves a weather forecast definition by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="queryBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WeatherForecastDefinition?> GetByIdAsync(WeatherForecastDefinitionId id, QueryBehavior queryBehavior = QueryBehavior.Default, CancellationToken cancellationToken = default)
        {
             List<WeatherForecastDefinitionDao> forecastDefinitions = await _database.ReadAsync();
            WeatherForecastDefinitionDao? dao = forecastDefinitions.FirstOrDefault(x => x.Id == id.Value);
            return dao is null ? null : MapToDomain(dao);
        }

        /// <summary>
        /// Retrieves all weather forecast definitions.
        /// </summary>
        /// <param name="queryBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<WeatherForecastDefinition>> GetAllAsync(QueryBehavior queryBehavior = QueryBehavior.Default, CancellationToken cancellationToken = default)
        {
             List<WeatherForecastDefinitionDao> forecastsDefinitions = await _database.ReadAsync();
            return forecastsDefinitions.ConvertAll(MapToDomain);
        }

        /// <summary>
        /// Adds a new weather forecast definition.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task AddAsync(WeatherForecastDefinition entity, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDefinitionDao> definitions = await _database.ReadAsync();
            definitions.Add(MapToDao(entity));
            await _database.WriteAsync(definitions);
        }

        /// <summary>
        /// Updates an existing weather forecast definition.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task UpdateAsync(WeatherForecastDefinition entity, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDefinitionDao> forecastDefinitions = await _database.ReadAsync();
            int index = forecastDefinitions.FindIndex(x => x.Id == entity.Id.Value);
            if (index != -1)
            {
                forecastDefinitions[index] = MapToDao(entity);
                await _database.WriteAsync(forecastDefinitions);
            }
        }

        /// <summary>
        /// Deletes a weather forecast definition.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        public async Task DeleteAsync(WeatherForecastDefinition entity, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDefinitionDao> forecastDefinitions = await _database.ReadAsync();
            forecastDefinitions.RemoveAll(x => x.Id == entity.Id.Value);
            await _database.WriteAsync(forecastDefinitions);
        }

        /// <summary>
        /// Retrieves a weather forecast definition by coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="queryBehavior"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WeatherForecastDefinition?> GetByCoordinatesAsync(Coordinates coordinates, QueryBehavior queryBehavior = QueryBehavior.Default, CancellationToken cancellationToken = default)
        {
            List<WeatherForecastDefinitionDao> forecastDefinitions = await _database.ReadAsync();
            WeatherForecastDefinitionDao? dao = forecastDefinitions.FirstOrDefault(x => Coordinates.Create(x.Latitude,x.Longitude).Equals(coordinates));
            return dao is null ? null : MapToDomain(dao);
        }

         /// <summary>
        /// Maps domain model to DAO.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static WeatherForecastDefinitionDao MapToDao(WeatherForecastDefinition entity)
        {
            return new WeatherForecastDefinitionDao(
                    entity.Id,
                    entity.CreatedAt,
                    entity.Coordinates.Latitude,
                    entity.Coordinates.Longitude);
        }

        /// <summary>
        /// Maps DAO to domain model.
        /// </summary>
        /// <param name="dao"></param>
        /// <returns></returns>
        private static WeatherForecastDefinition MapToDomain(WeatherForecastDefinitionDao dao)
        {
            return WeatherForecastDefinition.Create(dao.Id,
                    dao.CreatedAt,
                    Coordinates.Create(
                        dao.Latitude,
                        dao.Longitude));
        }
    }
}
