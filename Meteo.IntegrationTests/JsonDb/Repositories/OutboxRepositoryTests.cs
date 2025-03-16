namespace Meteo.IntegrationTests.JsonDb.Repositories
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Meteo.Persistence.Json.Repositories;
    using Meteo.Domain.Events;
    using Moq;
    using Xunit;
    using Library.Persistence.Dao;

    /// <summary>
    /// Unit tests for OutboxRepository using JSON persistence.
    /// </summary>
    public class OutboxRepositoryTests
    {
        private readonly string _testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "test_outbox.json");
        private readonly OutboxRepository _repository;
        private readonly Mock<TimeProvider> _mockTimeProvider = new Mock<TimeProvider>();

        /// <summary>
        /// Initializes a new instance of the OutboxRepositoryTests class.
        /// </summary>
        public OutboxRepositoryTests()
        {
            _repository = new OutboxRepository(_mockTimeProvider.Object, _testFilePath);
        }

        /// <summary>
        /// Tests saving and retrieving unprocessed messages.
        /// </summary>
        [Fact]
        public async Task Should_Save_And_Retrieve_Unprocessed_Messages()
        {
            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "TestType",
                Payload = "{\"data\": \"test\"}",
                OccurredAt = DateTimeOffset.UtcNow,
            };

            await _repository.SaveOutboxMessageAsync(message);

            var retrievedMessages = await _repository.GetUnprocessedMessagesAsync();

            retrievedMessages.Should().NotBeEmpty();
            retrievedMessages.Should().Contain(m => m.Id == message.Id);
        }

        /// <summary>
        /// Tests marking an outbox message as processed.
        /// </summary>
        [Fact]
        public async Task Should_Mark_Message_As_Processed()
        {
            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "TestType",
                Payload = "{\"data\": \"test\"}",
                OccurredAt = DateTimeOffset.UtcNow,
            };

            await _repository.SaveOutboxMessageAsync(message);

            // Ensure mock setup
            _mockTimeProvider!.Setup(m => m.GetUtcNow()).Returns(DateTimeOffset.UtcNow);

            await _repository.MarkMessageAsProcessedAsync(message);

            var retrievedMessages = await _repository.GetUnprocessedMessagesAsync();
            retrievedMessages.Should().NotContain(m => m.Id == message.Id);
        }

        /// <summary>
        /// Tests that unprocessed messages persist after a simulated shutdown and get processed upon recovery.
        /// </summary>
        [Fact]
        public async Task Should_Recover_And_Process_Unprocessed_Messages_After_Shutdown()
        {
            var message = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "TestType",
                Payload = "{\"data\": \"test\"}",
                OccurredAt = DateTimeOffset.UtcNow,
            };

            await _repository.SaveOutboxMessageAsync(message);

            var newRepository = new OutboxRepository(_mockTimeProvider.Object, _testFilePath);

            var retrievedMessages = await newRepository.GetUnprocessedMessagesAsync();

            retrievedMessages.Should().NotBeEmpty();
            retrievedMessages.Should().Contain(m => m.Id == message.Id);

            _mockTimeProvider.Setup(m => m.GetUtcNow()).Returns(DateTimeOffset.UtcNow);
            await newRepository.MarkMessageAsProcessedAsync(message);

            var remainingMessages = await newRepository.GetUnprocessedMessagesAsync();
            remainingMessages.Should().NotContain(m => m.Id == message.Id);
        }


        /// <summary>
        /// Tests serializing an event to JSON.
        /// </summary>
        [Fact]
        public void Should_Serialize_Event_To_Json()
        {
            var eventObj = new WeatherForecastCreated(Guid.NewGuid(), Guid.NewGuid());
            var json = OutboxRepository.SerializeEvent(eventObj);

            json.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Tests deserializing an event from JSON.
        /// </summary>
        [Fact]
        public void Should_Deserialize_Event_From_Json()
        {
            var eventObj = new WeatherForecastCreated(Guid.NewGuid(), Guid.NewGuid());
            var json = OutboxRepository.SerializeEvent(eventObj);

            var deserializedEvent = OutboxRepository.DeserializeEvent(json, typeof(WeatherForecastCreated));

            deserializedEvent.Should().NotBeNull();
            deserializedEvent.Should().BeOfType<WeatherForecastCreated>();
        }
    }
}