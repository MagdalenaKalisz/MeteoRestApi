namespace Meteo.Persistence.Json.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Persistence;
    using Library.Persistence.Dao;
    using Meteo.Persistence.Json.Dao;
    using System.Text.Json;
    using Meteo.Domain.Events;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Represents a JSON-based outbox repository.
    /// </summary>
    public sealed class OutboxRepository : IOutboxRepository
    {
        private readonly JsonDatabase<OutboxMessageDao> _database;
        private readonly TimeProvider _time;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxRepository"/> class.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="filePath"></param>
        public OutboxRepository(TimeProvider time, string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath)!;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
            }

            _database = new JsonDatabase<OutboxMessageDao>(filePath);
            _time = time;
        }

        /// <inheritdoc/>
        public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken = default)
        {
            List<OutboxMessageDao> daos = await _database.ReadAsync();

            return [.. daos
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.OccurredAt)
                .Select(MapToDomain),];
        }

        /// <inheritdoc/>
        public async Task MarkMessageAsProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            List<OutboxMessageDao> daos = await _database.ReadAsync();
            OutboxMessageDao? messageToUpdate = daos.FirstOrDefault(m => m.Id == message.Id);

            if (messageToUpdate is null)
            {
                throw new InvalidOperationException($"Message with Id {message.Id} not found in the database.");
            }

            messageToUpdate.ProcessedAt = _time.GetUtcNow(); // Ensure this sets the timestamp

            await _database.WriteAsync(daos); // Ensure the change is persisted
        }


        /// <inheritdoc/>
        public async Task SaveOutboxMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            List<OutboxMessageDao> daos = await _database.ReadAsync();
            daos.Add(MapToDao(message));

            await _database.WriteAsync(daos);
        }

        /// <summary>
        /// Serializes the event to a DAO.
        /// </summary>
        public static string SerializeEvent(object @event, CancellationToken cancellationToken = default)
        {
            string payload;

            if (@event is WeatherForecastDefinitionCreated weatherForecastDefinitionCreated)
            {
                var dao = new WeatherForecastDefinitionCreatedDao(
                    weatherForecastDefinitionCreated.DefinitionId,
                    weatherForecastDefinitionCreated.Coordinates.Latitude,
                    weatherForecastDefinitionCreated.Coordinates.Longitude
                );

                payload = JsonSerializer.Serialize(dao);
            }
            else if (@event is WeatherForecastCreated weatherForecastCreated)
            {
                var dao = new WeatherForecastCreatedDao(
                    weatherForecastCreated.ForecastId.Value,
                    weatherForecastCreated.DefinitionId.Value
                );

                payload = JsonSerializer.Serialize(dao);
            }
            else
            {
                payload = JsonSerializer.Serialize(@event);
            }

            return payload;
        }

        /// <summary>
        /// Deserializes the payload back into the event type.
        /// </summary>
        public static object? DeserializeEvent(string payload, Type eventType, CancellationToken cancellationToken = default)
        {
            if (eventType == typeof(WeatherForecastDefinitionCreated))
            {
                var dao = JsonSerializer.Deserialize<WeatherForecastDefinitionCreatedDao>(payload);
                if (dao != null)
                {
                    return new WeatherForecastDefinitionCreated(
                        dao.DefinitionId,
                        Coordinates.Create(dao.Latitude, dao.Longitude)
                    );
                }
            }
            else if (eventType == typeof(WeatherForecastCreated))
            {
                var dao = JsonSerializer.Deserialize<WeatherForecastCreatedDao>(payload);
                if (dao != null)
                {
                    return new WeatherForecastCreated(
                        WeatherForecastId.Create(dao.ForecastId),
                        WeatherForecastDefinitionId.Create(dao.DefinitionId)
                    );
                }
            }

            return JsonSerializer.Deserialize(payload, eventType);
        }

        /// <summary>
        /// Maps an <see cref="OutboxMessage"/> domain object to <see cref="OutboxMessageDao"/> for storage.
        /// </summary>
        /// <param name="entity"></param>
        private static OutboxMessageDao MapToDao(OutboxMessage entity)
        {
            return new OutboxMessageDao(
                entity.Id,
                entity.Type,
                entity.Payload,
                entity.OccurredAt,
                entity.ProcessedAt
            );
        }

        /// <summary>
        /// Maps an <see cref="OutboxMessageDao"/> to the domain model <see cref="OutboxMessage"/>.
        /// </summary>
        /// <param name="dao"></param>
        private static OutboxMessage MapToDomain(OutboxMessageDao dao)
        {
            return new OutboxMessage
            {
                Id = dao.Id,
                Type = dao.Type,
                Payload = dao.Payload,
                OccurredAt = dao.OccurredAt,
                ProcessedAt = dao.ProcessedAt,
            };
        }
    }
}