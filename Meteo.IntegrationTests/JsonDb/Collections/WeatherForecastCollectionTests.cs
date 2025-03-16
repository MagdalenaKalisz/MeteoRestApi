namespace Meteo.IntegrationTests.JsonDb.Collections
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Meteo.Persistence.Json.Collections;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Enums;
    using Meteo.Domain.ValueObjects;
    using Xunit;

    /// <summary>
    /// Unit tests for WeatherForecastCollection using JSON persistence.
    /// </summary>
    public class WeatherForecastCollectionTests
    {
        private readonly string _testFilePath = "test_forecasts.json";
        private readonly WeatherForecastCollection _repository;

        /// <summary>
        /// Initializes a new instance of the WeatherForecastCollectionTests class.
        /// </summary>
        public WeatherForecastCollectionTests()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
            _repository = new WeatherForecastCollection(_testFilePath);
        }

        /// <summary>
        /// Tests adding and retrieving a weather forecast.
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
        }

        /// <summary>
        /// Tests retrieving all weather forecasts.
        /// </summary>
        [Fact]
        public async Task Should_Retrieve_All_WeatherForecasts()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var forecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow),
                Temperature.Create(22.0, TemperatureUnit.Celsius),
                Humidity.Create(60)
            ),
        };

            var weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);
            await _repository.AddAsync(weatherForecast);

            var allForecasts = await _repository.GetAllAsync();

            allForecasts.Should().NotBeEmpty();
        }

        /// <summary>
        /// Tests deleting a weather forecast.
        /// </summary>
        [Fact]
        public async Task Should_Delete_WeatherForecast()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var forecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow),
                Temperature.Create(20.0, TemperatureUnit.Celsius),
                Humidity.Create(40)
            ),
        };

            var weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);
            await _repository.AddAsync(weatherForecast);
            await _repository.DeleteAsync(weatherForecast);

            var retrieved = await _repository.GetByIdAsync(forecastId);
            retrieved.Should().BeNull();
        }

        /// <summary>
        /// Tests updating a weather forecast.
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
                Temperature.Create(30.0, TemperatureUnit.Celsius),
                Humidity.Create(70)
            ),
        };

            var weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);
            await _repository.AddAsync(weatherForecast);

            var newForecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow.AddHours(2)),
                Temperature.Create(15.0, TemperatureUnit.Celsius),
                Humidity.Create(55)
            ),
        };

            var updatedForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, newForecasts);
            await _repository.UpdateAsync(updatedForecast);

            var retrieved = await _repository.GetByIdAsync(forecastId);
            retrieved.Should().NotBeNull();
            retrieved.Forecasts[0].Temperature.Value.Should().Be(15.0);
        }

        /// <summary>
        /// Tests retrieving a weather forecast by definition ID.
        /// </summary>
        [Fact]
        public async Task Should_Retrieve_WeatherForecast_ByDefinitionId()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var forecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow),
                Temperature.Create(10.0, TemperatureUnit.Celsius),
                Humidity.Create(30)
            ),
        };

            var weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);
            await _repository.AddAsync(weatherForecast);

            var retrieved = await _repository.GetByDefinitionIdAsync(definitionId);

            retrieved.Should().NotBeNull();
            retrieved.DefinitionId.Should().Be(definitionId);
        }

        /// <summary>
        /// Tests updating a non-existent weather forecast should not throw.
        /// </summary>
        [Fact]
        public async Task Should_Not_Throw_When_Updating_NonExistent_Forecast()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var newForecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(
                ForecastDateTime.Create(DateTimeOffset.UtcNow.AddHours(2)),
                Temperature.Create(15.0, TemperatureUnit.Celsius),
                Humidity.Create(55)
            ),
        };

            var updatedForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, newForecasts);
            Func<Task> act = async () => await _repository.UpdateAsync(updatedForecast);

            await act.Should().NotThrowAsync();
        }

        /// <summary>
        /// Tests retrieving a weather forecast by definition ID when it does not exist.
        /// </summary>
        [Fact]
        public async Task Should_Return_Null_When_DefinitionId_Not_Found()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var retrieved = await _repository.GetByDefinitionIdAsync(definitionId);

            retrieved.Should().BeNull();
        }

        /// <summary>
        /// Tests adding a null forecast should throw an exception.
        /// </summary>
        [Fact]
        public async Task Should_Throw_When_Adding_Null_Forecast()
        {
            Func<Task> act = async () => await _repository.AddAsync(null!);

            await act.Should().ThrowAsync<NullReferenceException>();
        }
    }
}