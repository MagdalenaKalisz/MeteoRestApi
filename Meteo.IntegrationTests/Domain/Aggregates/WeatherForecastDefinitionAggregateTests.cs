namespace Meteo.IntegrationTests.Domain.Aggregates
{
    using System;
    using FluentAssertions;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Events;
    using Meteo.Domain.ValueObjects;
    using Xunit;

    /// <summary>
    /// Unit tests for the WeatherForecastDefinition aggregate.
    /// </summary>
    public class WeatherForecastDefinitionAggregateTests
    {
        /// <summary>
        /// Tests creating a valid WeatherForecastDefinition instance.
        /// </summary>
        [Fact]
        public void Should_Create_WeatherForecastDefinition_Successfully()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(52.5200, 13.4050);
            var createdAt = DateTimeOffset.UtcNow;

            var weatherForecastDefinition = WeatherForecastDefinition.Create(definitionId, createdAt, coordinates);

            weatherForecastDefinition.Should().NotBeNull();
            weatherForecastDefinition.Id.Should().Be(definitionId);
            weatherForecastDefinition.Coordinates.Should().Be(coordinates);
        }

        /// <summary>
        /// Tests that creating a WeatherForecastDefinition adds a domain event.
        /// </summary>
        [Fact]
        public void Should_Add_Domain_Event_On_Creation()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(34.0522, -118.2437);
            var createdAt = DateTimeOffset.UtcNow;

            var weatherForecastDefinition = WeatherForecastDefinition.Create(definitionId, createdAt, coordinates);

            weatherForecastDefinition.DomainEvents.Should().ContainSingle()
                .Which.Should().BeOfType<WeatherForecastDefinitionCreated>();
        }

        /// <summary>
        /// Tests that creating a WeatherForecastDefinition with null coordinates throws an exception.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Coordinates_Are_Null()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var createdAt = DateTimeOffset.UtcNow;

            Action act = () => WeatherForecastDefinition.Create(definitionId, createdAt, null!);

            act.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests that creating a WeatherForecastDefinition with null id throws an exception.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Id_Is_Null()
        {
            var coordinates = Coordinates.Create(48.8566, 2.3522);
            var createdAt = DateTimeOffset.UtcNow;

            Action act = () => WeatherForecastDefinition.Create(null!, createdAt, coordinates);

            act.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests that creating a WeatherForecastDefinition with null creation timestamp throws an exception.
        /// </summary>
        [Fact]
        public void Should_Throw_When_CreationTimestamp_Is_Null()
        {
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var coordinates = Coordinates.Create(40.7128, -74.0060);

            Action act = () => WeatherForecastDefinition.Create(definitionId, null!, coordinates);

            act.Should().Throw<ArgumentNullException>();
        }
    }

}