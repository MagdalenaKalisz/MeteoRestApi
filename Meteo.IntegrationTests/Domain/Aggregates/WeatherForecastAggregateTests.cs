namespace Meteo.IntegrationTests.Domain.Aggregates
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Enums;
    using Meteo.Domain.Events;
    using Meteo.Domain.ValueObjects;
    using Xunit;

    /// <summary>
    /// Unit tests for the WeatherForecast aggregate.
    /// </summary>
    public class WeatherForecastAggregateTests
    {
        /// <summary>
        /// Tests creating a valid WeatherForecast instance.
        /// </summary>
        [Fact]
        public void Should_Create_WeatherForecast_Successfully()
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

            weatherForecast.Should().NotBeNull();
            weatherForecast.Forecasts.Should().HaveCount(1);
            weatherForecast.DefinitionId.Should().Be(definitionId);
        }

        /// <summary>
        /// Tests that creating a WeatherForecast with duplicate forecast hours throws an exception.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Duplicate_Hours_Exist()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var timestamp = DateTimeOffset.UtcNow;

            var forecasts = new List<WeatherForecastForDay>
        {
            WeatherForecastForDay.Create(ForecastDateTime.Create(timestamp), Temperature.Create(20.0, TemperatureUnit.Celsius), Humidity.Create(40)),
            WeatherForecastForDay.Create(ForecastDateTime.Create(timestamp), Temperature.Create(22.0, TemperatureUnit.Celsius), Humidity.Create(42)),
        };

            Action act = () => WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);

            act.Should().Throw<ArgumentException>();

        }

        /// <summary>
        /// Tests that creating a WeatherForecast with more than 24 entries per day throws an exception.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Exceeding_24_Entries_Per_Day()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var timestamp = DateTimeOffset.UtcNow;
            var forecasts = new List<WeatherForecastForDay>();

            for (int i = 0; i < 25; i++)
            {
                forecasts.Add(WeatherForecastForDay.Create(
                    ForecastDateTime.Create(timestamp.AddHours(i)),
                    Temperature.Create(20.0 + i, TemperatureUnit.Celsius),
                    Humidity.Create(40 + i)
                ));
            }

            Action act = () => WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Tests creating a WeatherForecast with an empty forecast list.
        /// </summary>
        [Fact]
        public void Should_Create_WeatherForecast_With_Empty_Forecast_List()
        {
            var forecastId = WeatherForecastId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var forecasts = new List<WeatherForecastForDay>();

            var weatherForecast = WeatherForecast.Create(forecastId, DateTimeOffset.UtcNow, definitionId, forecasts);

            weatherForecast.Should().NotBeNull();
            weatherForecast.Forecasts.Should().BeEmpty();
        }

        /// <summary>
        /// Tests that creating a WeatherForecast adds a domain event.
        /// </summary>
        [Fact]
        public void Should_Add_Domain_Event_On_Creation()
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

            weatherForecast.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<WeatherForecastCreated>();
        }
    }
}