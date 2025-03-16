namespace Meteo.IntegrationTests.PostgreSql.Collections
{
    using Microsoft.EntityFrameworkCore;
    using Testcontainers.PostgreSql;
    using Xunit;
    using FluentAssertions;
    using Meteo.Persistence.PostgreSql;
    using Meteo.Persistence.PostgreSql.Collections;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.ValueObjects;
    using Meteo.Persistence.PostgreSql.Dao;
    using System.Reflection;

    /// <summary>
    /// Integration tests for WeatherForecastDefinitionCollection.
    /// </summary>
    public class WeatherForecastDefinitionCollectionTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private ApplicationDbContext _dbContext = null!;
        private WeatherForecastDefinitionCollection _repository = null!;

        /// <summary>
        /// Initializes a new instance of the WeatherForecastDefinitionCollectionTests class.
        /// </summary>
        public WeatherForecastDefinitionCollectionTests()
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

            _repository = new WeatherForecastDefinitionCollection(_dbContext);
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
        /// Tests adding and retrieving a weather forecast definition by ID.
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
            retrieved.Coordinates.Should().Be(coordinates);
        }

        /// <summary>
        /// Tests retrieving a weather forecast definition by coordinates.
        /// </summary>
        [Fact]
        public async Task Should_Retrieve_WeatherForecastDefinition_ByCoordinates()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(48.856600, 2.352200);

            var forecastDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, coordinates);
            await _repository.AddAsync(forecastDefinition);

            var retrieved = await _repository.GetByCoordinatesAsync(coordinates);

            retrieved.Should().NotBeNull();
            retrieved.Id.Should().Be(definitionId);
            retrieved.Coordinates.Should().Be(coordinates);
        }

        /// <summary>
        /// Tests updating an existing weather forecast definition.
        /// </summary>
        [Fact]
        public async Task Should_Update_WeatherForecastDefinition()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(40.7128, -74.0060);

            var forecastDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, coordinates);
            await _repository.AddAsync(forecastDefinition);

            var newCoordinates = Coordinates.Create(34.0522, -118.2437);
            var updatedDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, newCoordinates);
            await _repository.UpdateAsync(updatedDefinition);

            var retrieved = await _repository.GetByIdAsync(definitionId);

            retrieved.Should().NotBeNull();
            retrieved.Coordinates.Should().Be(newCoordinates);
        }

        /// <summary>
        /// Tests that AddAsync throws an ArgumentNullException when a null forecast definition is added.
        /// </summary>
        [Fact]
        public async Task AddAsync_Should_Throw_ArgumentNullException_When_Definition_Is_Null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _repository.AddAsync(null!);
            });
        }

        /// <summary>
        /// Tests retrieving all weather forecast definitions.
        /// </summary>
        [Fact]
        public async Task Should_Retrieve_All_WeatherForecastDefinitions()
        {
            var definitionId1 = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var definitionId2 = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates1 = Coordinates.Create(35.6895, 139.6917);
            var coordinates2 = Coordinates.Create(51.5074, -0.1278);

            var forecastDefinition1 = WeatherForecastDefinition.Create(definitionId1.Value, DateTimeOffset.UtcNow, coordinates1);
            var forecastDefinition2 = WeatherForecastDefinition.Create(definitionId2.Value, DateTimeOffset.UtcNow, coordinates2);

            await _repository.AddAsync(forecastDefinition1);
            await _repository.AddAsync(forecastDefinition2);

            var allDefinitions = await _repository.GetAllAsync();

            allDefinitions.Should().HaveCount(2);
        }

        /// <summary>
        /// Tests mapping from domain entity to DAO.
        /// </summary>
        [Fact]
        public void Should_Map_Domain_To_Dao()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(52.520000, 13.405000);
            var forecastDefinition = WeatherForecastDefinition.Create(definitionId.Value, DateTimeOffset.UtcNow, coordinates);

            // Use reflection to access the protected method
            var methodInfo = typeof(WeatherForecastDefinitionCollection)
                .GetMethod("MapToDao", BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Should().NotBeNull("MapToDao method should exist");

            var dao = (WeatherForecastDefinitionDao?)methodInfo?.Invoke(_repository, new object[] { forecastDefinition });

            dao.Should().NotBeNull();
            dao!.Id.Should().Be(forecastDefinition.Id.Value);
            dao.Latitude.Should().Be(forecastDefinition.Coordinates.Latitude);
            dao.Longitude.Should().Be(forecastDefinition.Coordinates.Longitude);
        }

        /// <summary>
        /// Tests mapping from DAO to domain entity.
        /// </summary>
        [Fact]
        public void Should_Map_Dao_To_Domain()
        {
            var dao = new WeatherForecastDefinitionDao
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                Latitude = 48.8566,
                Longitude = 2.3522,
            };

            var methodInfo = typeof(WeatherForecastDefinitionCollection)
                .GetMethod("MapToDomain", BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Should().NotBeNull("MapToDomain method should exist");

            var domainEntity = (WeatherForecastDefinition?)methodInfo?.Invoke(_repository, [dao]);

            domainEntity.Should().NotBeNull();
            domainEntity!.Id.Value.Should().Be(dao.Id);
            domainEntity.Coordinates.Latitude.Value.Should().Be(dao.Latitude);
            domainEntity.Coordinates.Longitude.Value.Should().Be(dao.Longitude);
        }
    }
}