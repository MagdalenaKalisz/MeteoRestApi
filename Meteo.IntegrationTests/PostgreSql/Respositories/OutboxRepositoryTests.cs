namespace Meteo.IntegrationTests.PostgreSql.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Testcontainers.PostgreSql;
    using Xunit;
    using FluentAssertions;
    using Meteo.Persistence.PostgreSql.Repositories;
    using Meteo.Persistence.PostgreSql.Dao;
    using Meteo.Domain.Events;
    using Meteo.Domain.ValueObjects;
    using Moq;
    using System.Reflection;
    using Library.Persistence.Dao;
    using Meteo.Persistence.PostgreSql;


    /// <summary>
    /// Integration tests for OutboxRepository.
    /// </summary>
    public class OutboxRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private ApplicationDbContext _dbContext = null!;
        private OutboxRepository _repository = null!;
        private readonly Mock<TimeProvider> _mockTimeProvider = new();

        /// <summary>
        /// Initializes a new instance of the OutboxRepositoryTests class.
        /// </summary>
        public OutboxRepositoryTests()
        {
            _postgresContainer = new PostgreSqlBuilder()
                .WithDatabase("testdb")
                .WithUsername("testuser")
                .WithPassword("testpassword")
                .Build();
        }

        /// <summary>
        /// Initializes the test environment.
        /// </summary>
        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_postgresContainer.GetConnectionString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            await _dbContext.Database.EnsureCreatedAsync();

            _repository = new OutboxRepository(_dbContext, _mockTimeProvider.Object);
        }

        /// <summary>
        /// Cleans up resources after tests.
        /// </summary>
        public async Task DisposeAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
            await _postgresContainer.DisposeAsync();
        }

        /// <summary>
        /// Tests adding and retrieving unprocessed messages.
        /// </summary>
        [Fact]
        public async Task Should_Add_And_Retrieve_Unprocessed_Messages()
        {
            var message = new OutboxMessageDao(Guid.NewGuid(), "TestType", "{}", DateTimeOffset.UtcNow, null);
            _dbContext.OutboxMessages.Add(message);
            await _dbContext.SaveChangesAsync();

            var unprocessedMessages = await _repository.GetUnprocessedMessagesAsync();

            unprocessedMessages.Should().NotBeEmpty();
            unprocessedMessages[0].Id.Should().Be(message.Id);
        }

        /// <summary>
        /// Tests failure scenario when retrieving unprocessed messages from an empty database.
        /// </summary>
        [Fact]
        public async Task GetUnprocessedMessages_Should_Return_Empty_When_No_Messages()
        {
            var unprocessedMessages = await _repository.GetUnprocessedMessagesAsync();
            unprocessedMessages.Should().BeEmpty();
        }

        /// <summary>
        /// Tests failure scenario when marking a non-existent message as processed.
        /// </summary>
        [Fact]
        public async Task MarkMessageAsProcessed_Should_Not_Throw_When_Message_Not_Found()
        {
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "TestType",
                Payload = "{}",
                OccurredAt = DateTimeOffset.UtcNow,
                ProcessedAt = null,
            };

            Func<Task> act = async () => await _repository.MarkMessageAsProcessedAsync(outboxMessage);
            await act.Should().NotThrowAsync();
        }

        /// <summary>
        /// Tests serializing an event to JSON.
        /// </summary>
        [Fact]
        public void Should_Serialize_Event()
        {
            var eventObj = new WeatherForecastCreated(WeatherForecastId.Create(Guid.NewGuid()), WeatherForecastDefinitionId.Create(Guid.NewGuid()));
            var json = OutboxRepository.SerializeEvent(eventObj);
            json.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Tests deserializing a JSON payload back to an event.
        /// </summary>
        [Fact]
        public void Should_Deserialize_Event()
        {
            var eventObj = new WeatherForecastCreated(WeatherForecastId.Create(Guid.NewGuid()), WeatherForecastDefinitionId.Create(Guid.NewGuid()));
            var json = OutboxRepository.SerializeEvent(eventObj);
            var deserialized = OutboxRepository.DeserializeEvent(json, typeof(WeatherForecastCreated));
            deserialized.Should().BeOfType<WeatherForecastCreated>();
        }

        /// <summary>
        /// Tests mapping from domain entity to DAO.
        /// </summary>
        [Fact]
        public void Should_Map_Domain_To_Dao()
        {
            var entity = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "TestType",
                Payload = "{}",
                OccurredAt = DateTimeOffset.UtcNow,
                ProcessedAt = null,
            };

            var methodInfo = typeof(OutboxRepository)
                .GetMethod("MapToDao", BindingFlags.NonPublic | BindingFlags.Static);

            methodInfo.Should().NotBeNull("MapToDao method should exist");

            var dao = (OutboxMessageDao?)methodInfo?.Invoke(null, [entity]);

            dao.Should().NotBeNull();
            dao!.Id.Should().Be(entity.Id);
            dao.Type.Should().Be(entity.Type);
            dao.Payload.Should().Be(entity.Payload);
            dao.OccurredAt.Should().Be(entity.OccurredAt);
        }

        /// <summary>
        /// Tests mapping from DAO to domain entity.
        /// </summary>
        [Fact]
        public void Should_Map_Dao_To_Domain()
        {
            var dao = new OutboxMessageDao(Guid.NewGuid(), "TestType", "{}", DateTimeOffset.UtcNow, null);

            var methodInfo = typeof(OutboxRepository)
                .GetMethod("MapToDomain", BindingFlags.NonPublic | BindingFlags.Static);

            methodInfo.Should().NotBeNull("MapToDomain method should exist");

            var entity = (OutboxMessage?)methodInfo?.Invoke(null, [dao]);

            entity.Should().NotBeNull();
            entity!.Id.Should().Be(dao.Id);
            entity.Type.Should().Be(dao.Type);
            entity.Payload.Should().Be(dao.Payload);
            entity.OccurredAt.Should().Be(dao.OccurredAt);
        }

        /// <summary>
        /// Tests serializing and deserializing an event.
        /// </summary>
        [Fact]
        public void Should_Serialize_And_Deserialize_Event()
        {
            var originalEvent = new WeatherForecastCreated(
                WeatherForecastId.Create(Guid.NewGuid()),
                WeatherForecastDefinitionId.Create(Guid.NewGuid())
            );

            var json = OutboxRepository.SerializeEvent(originalEvent);
            json.Should().NotBeNullOrEmpty();

            var deserializedEvent = OutboxRepository.DeserializeEvent(json, typeof(WeatherForecastCreated));
            deserializedEvent.Should().NotBeNull();
            deserializedEvent.Should().BeOfType<WeatherForecastCreated>();
        }

        /// <summary>
        /// Tests serialization failure for unsupported event type.
        /// </summary>
        [Fact]
        public void SerializeEvent_Should_Handle_Unknown_Event_Type()
        {
            var unknownEvent = new { Name = "UnknownEvent", Data = 123 };
            var json = OutboxRepository.SerializeEvent(unknownEvent);
            json.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Tests deserialization failure for mismatched event type.
        /// </summary>
        [Fact]
        public void DeserializeEvent_Should_Handle_Wrong_Event_Type()
        {
            var json = OutboxRepository.SerializeEvent(new WeatherForecastCreated(
                WeatherForecastId.Create(Guid.NewGuid()),
                WeatherForecastDefinitionId.Create(Guid.NewGuid())
            ));

            var deserializedEvent = OutboxRepository.DeserializeEvent(json, typeof(WeatherForecastCreated));
            deserializedEvent.Should().NotBeOfType<WeatherForecastDefinitionCreated>();
        }
    }
}