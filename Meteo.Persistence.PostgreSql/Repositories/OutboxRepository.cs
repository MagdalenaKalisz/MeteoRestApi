namespace Meteo.Persistence.PostgreSql.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Library.Persistence;
    using Library.Persistence.Dao;
    using Meteo.Persistence.PostgreSql.Dao;
    using Meteo.Domain.Events;
    using Microsoft.EntityFrameworkCore;
    using System.Text.Json;
    using Meteo.Domain.ValueObjects;


    /// <summary>
    /// Represents a PostgreSQL-based outbox repository.
    /// </summary>
    public sealed class OutboxRepository : IOutboxRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TimeProvider _time;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboxRepository"/> class.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="time"></param>
        public OutboxRepository(ApplicationDbContext dbContext,
                                TimeProvider time)
        {
            _dbContext = dbContext;
            _time = time;
        }

        /// <inheritdoc/>
        public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken = default)
        {
            var daoMessages = await _dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.OccurredAt)
                .ToListAsync(cancellationToken);

            return [.. daoMessages.Select(MapToDomain)];
        }

        /// <inheritdoc/>
        public async Task MarkMessageAsProcessedAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            OutboxMessageDao? entity = await _dbContext.OutboxMessages
                .FirstOrDefaultAsync(m => m.Id == message.Id, cancellationToken);

            if (entity is not null)
            {
                entity.ProcessedAt = _time.GetUtcNow();
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task SaveOutboxMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            var outboxMessageDao = MapToDao(message);

            _dbContext.OutboxMessages.Add(outboxMessageDao);

            await _dbContext.SaveChangesAsync(cancellationToken);
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
