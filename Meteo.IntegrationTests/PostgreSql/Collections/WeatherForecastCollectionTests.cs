
namespace Meteo.IntegrationTests.PostgreSql.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Testcontainers.PostgreSql;
    using Xunit;
    using FluentAssertions;
    using Meteo.Persistence.PostgreSql;
    using Meteo.Persistence.PostgreSql.Collections;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.ValueObjects;
    using Meteo.Domain.Enums;
    using Library.Identifiers;
    using Moq;

    /// <summary>
    /// Integration tests for WeatherForecastCollection.
    /// </summary>
    public class WeatherForecastCollectionTests : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer;
        private ApplicationDbContext _dbContext = null!;
        private WeatherForecastCollection _repository = null!;
        private readonly Mock<IdentifierProvider> _mockIdentifierProvider = new();

        /// <summary>
        /// Initializes a new instance of the WeatherForecastCollectionTests class.
        /// </summary>
        public WeatherForecastCollectionTests()
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

            _mockIdentifierProvider
                .Setup(m => m.CreateSequentialUuid())
                .Returns(Guid.NewGuid());
            _repository = new WeatherForecastCollection(_dbContext, _mockIdentifierProvider.Object);
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
        /// Tests adding and retrieving a weather forecast by ID.
        /// </summary>
        [Fact]
        public async Task Should_Add_And_Retrieve_WeatherForecast_ById()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());

            var forecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow),
                Temperature.Create(25.0, TemperatureUnit.Celsius),
                Humidity.Create(50)
            ),
        };

            var weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);

            await _repository.AddAsync(weatherForecast);
            var retrieved = await _repository.GetByIdAsync(forecastId);

            retrieved.Should().NotBeNull();
            retrieved.Id.Should().Be(forecastId);
            retrieved.Forecasts.Should().HaveCount(1);
        }

        /// <summary>
        /// Tests updating an existing weather forecast.
        /// </summary>
        [Fact]
        public async Task Should_Update_WeatherForecast()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());

            var forecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow),
                Temperature.Create(22.0, TemperatureUnit.Celsius),
                Humidity.Create(55)
            ),
        };

            var weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);
            await _repository.AddAsync(weatherForecast);

            var newForecast = WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow.AddHours(2)),
                Temperature.Create(18.0, TemperatureUnit.Celsius),
                Humidity.Create(65)
            );

            weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, [newForecast]);
            await _repository.UpdateAsync(weatherForecast);

            var updatedForecast = await _repository.GetByIdAsync(forecastId);

            updatedForecast.Should().NotBeNull();
            updatedForecast.Forecasts.Should().ContainSingle();
            updatedForecast.Forecasts[0].Temperature.Value.Should().Be(18.0);
        }
    }
}