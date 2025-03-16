namespace Meteo.IntegrationTests.JsonDb.Collections
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Meteo.Persistence.Json.Collections;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.ValueObjects;
    using Xunit;

    /// <summary>
    /// Unit tests for WeatherForecastDefinitionCollection using JSON persistence.
    /// </summary>
    public class WeatherForecastDefinitionCollectionTests
    {
        private readonly string _testFilePath = "test_forecast_definitions.json";
        private readonly WeatherForecastDefinitionCollection _repository;

        /// <summary>
        /// Initializes a new instance of the WeatherForecastDefinitionCollectionTests class.
        /// </summary>
        public WeatherForecastDefinitionCollectionTests()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
            _repository = new WeatherForecastDefinitionCollection(_testFilePath);
        }

        /// <summary>
        /// Tests adding and retrieving a weather forecast definition.
        /// </summary>
        [Fact]
        public async Task Should_Add_And_Retrieve_WeatherForecastDefinition_ById()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(52.5200, 13.4050);
            var forecastDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, coordinates);

            await _repository.AddAsync(forecastDefinition);

            var retrieved = await _repository.GetByIdAsync(definitionId);

            retrieved.Should().NotBeNull();
            retrieved.Id.Should().Be(definitionId);
        }

        /// <summary>
        /// Tests retrieving all weather forecast definitions.
        /// </summary>
        [Fact]
        public async Task Should_Retrieve_All_WeatherForecastDefinitions()
        {
            var allDefinitions = await _repository.GetAllAsync();
            allDefinitions.Should().BeEmpty();
        }

        /// <summary>
        /// Tests deleting a weather forecast definition.
        /// </summary>
        [Fact]
        public async Task Should_Delete_WeatherForecastDefinition()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(40.7128, -74.0060);
            var forecastDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, coordinates);

            await _repository.AddAsync(forecastDefinition);
            await _repository.DeleteAsync(forecastDefinition);

            var retrieved = await _repository.GetByIdAsync(definitionId);
            retrieved.Should().BeNull();
        }

        /// <summary>
        /// Tests retrieving a weather forecast definition by coordinates.
        /// </summary>
        [Fact]
        public async Task Should_Retrieve_WeatherForecastDefinition_ByCoordinates()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(34.0522, -118.2437);
            var forecastDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, coordinates);

            await _repository.AddAsync(forecastDefinition);

            var retrieved = await _repository.GetByCoordinatesAsync(coordinates);

            retrieved.Should().NotBeNull();
            retrieved.Coordinates.Should().Be(coordinates);
        }

        /// <summary>
        /// Tests updating a weather forecast definition.
        /// </summary>
        [Fact]
        public async Task Should_Update_WeatherForecastDefinition()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(48.8566, 2.3522);
            var forecastDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, coordinates);

            await _repository.AddAsync(forecastDefinition);

            var newCoordinates = Coordinates.Create(51.5074, -0.1278);
            var updatedDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, newCoordinates);
            await _repository.UpdateAsync(updatedDefinition);

            var retrieved = await _repository.GetByIdAsync(definitionId);
            retrieved.Should().NotBeNull();
            retrieved.Coordinates.Should().Be(newCoordinates);
        }

        /// <summary>
        /// Tests that retrieving a non-existent weather forecast definition returns null.
        /// </summary>
        [Fact]
        public async Task Should_Return_Null_When_DefinitionId_Not_Found()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var retrieved = await _repository.GetByIdAsync(definitionId);
            retrieved.Should().BeNull();
        }

        /// <summary>
        /// Tests that adding a null forecast definition throws an exception.
        /// </summary>
        [Fact]
        public async Task Should_Throw_When_Adding_Null_ForecastDefinition()
        {
            Func<Task> act = async () => await _repository.AddAsync(null!);
            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}