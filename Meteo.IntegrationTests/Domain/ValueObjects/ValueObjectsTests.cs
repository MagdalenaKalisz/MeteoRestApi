namespace Meteo.IntegrationTests.Domain.ValueObjects
{
    using System;
    using FluentAssertions;
    using Meteo.Domain.Enums;
    using Meteo.Domain.ValueObjects;
    using Xunit;
    /// <summary>
    /// Unit tests for Value Objects.
    /// </summary>
    public class ValueObjectsTests
    {
        /// <summary>
        /// Tests creating valid Coordinates instances.
        /// </summary>
        [Fact]
        public void Should_Create_Coordinates_Successfully()
        {
            var latitude = Latitude.Create(52.5200);
            var longitude = Longitude.Create(13.4050);

            var coordinates = Coordinates.Create(latitude, longitude);

            coordinates.Should().NotBeNull();
            coordinates.Latitude.Should().Be(latitude);
            coordinates.Longitude.Should().Be(longitude);
        }

        /// <summary>
        /// Tests that creating Coordinates with null values throws an exception.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Coordinates_Are_Null()
        {
            Action act = () => Coordinates.Create(null!, Longitude.Create(13.4050));
            act.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests creating a valid ForecastDateTime.
        /// </summary>
        [Fact]
        public void Should_Create_ForecastDateTime_Successfully()
        {
            var dateTime = DateTimeOffset.UtcNow;
            var forecastDateTime = ForecastDateTime.Create(dateTime);

            forecastDateTime.Should().NotBeNull();
            forecastDateTime.DateTime.Should().Be(dateTime);
        }

        /// <summary>
        /// Tests creating an invalid ForecastDateTime.
        /// </summary>
        [Fact]
        public void Should_Throw_When_ForecastDateTime_Is_Invalid()
        {
            Action act = () => ForecastDateTime.Create(new DateTimeOffset(2023, 1, 1, 24, 0, 0, TimeSpan.Zero));
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Tests creating valid ForecastDaysAmount instances.
        /// </summary>
        [Fact]
        public void Should_Create_ForecastDaysAmount_Successfully()
        {
            var forecastDays = ForecastDaysAmount.Create(5);

            forecastDays.Should().NotBeNull();
            forecastDays.Value.Should().Be(5);
        }

        /// <summary>
        /// Tests that ForecastDaysAmount throws an exception for an out-of-range value.
        /// </summary>
        [Fact]
        public void Should_Throw_When_ForecastDaysAmount_Is_Invalid()
        {
            Action act = () => ForecastDaysAmount.Create(20);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Tests creating a valid Humidity value object.
        /// </summary>
        [Fact]
        public void Should_Create_Humidity_Successfully()
        {
            var humidity = Humidity.Create(60);

            humidity.Should().NotBeNull();
            humidity.Percentage.Should().Be(60);
        }

        /// <summary>
        /// Tests that Humidity throws an exception when the value is out of range.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Humidity_Is_Out_Of_Range()
        {
            Action act = () => Humidity.Create(110);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Tests creating a valid Latitude value object.
        /// </summary>
        [Fact]
        public void Should_Create_Latitude_Successfully()
        {
            var latitude = Latitude.Create(45.0);

            latitude.Should().NotBeNull();
            latitude.Value.Should().Be(45.0);
        }

        /// <summary>
        /// Tests that Latitude throws an exception when the value is out of range.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Latitude_Is_Out_Of_Range()
        {
            Action act = () => Latitude.Create(100.0);
            act.Should().Throw<ArgumentException>();
        }

        /// <summary>
        /// Tests creating a valid Longitude value object.
        /// </summary>
        [Fact]
        public void Should_Create_Longitude_Successfully()
        {
            var longitude = Longitude.Create(90.0);
            longitude.Should().NotBeNull();
            longitude.Value.Should().Be(90.0);
        }

        /// <summary>
        /// Tests that Longitude throws an exception when the value is out of range.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Longitude_Is_Out_Of_Range()
        {
            Assert.Throws<ArgumentException>(() => Longitude.Create(190.0));
            Assert.Throws<ArgumentException>(() => Longitude.Create(-190.0));
        }

        /// <summary>
        /// Tests creating a valid Temperature value object.
        /// </summary>
        [Fact]
        public void Should_Create_Temperature_Successfully()
        {
            var temperature = Temperature.Create(25.0, TemperatureUnit.Celsius);
            temperature.Should().NotBeNull();
            temperature.Value.Should().Be(25.0);
        }

        /// <summary>
        /// Tests that Temperature throws an exception when the value is below absolute zero.
        /// </summary>
        [Fact]
        public void Should_Throw_When_Temperature_Below_Absolute_Zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Temperature.Create(-300.0, TemperatureUnit.Celsius));
        }

        /// <summary>
        /// Tests creating a WeatherForecastForDay instance successfully.
        /// </summary>
        [Fact]
        public void Should_Create_WeatherForecastForDay_Successfully()
        {
            var forecastDateTime = ForecastDateTime.Create(DateTimeOffset.UtcNow);
            var temperature = Temperature.Create(20.0, TemperatureUnit.Celsius);
            var humidity = Humidity.Create(50);
            var forecastForDay = WeatherForecastForDay.Create(forecastDateTime, temperature, humidity);
            forecastForDay.Should().NotBeNull();
            forecastForDay.ForecastDateTime.Should().Be(forecastDateTime);
            forecastForDay.Temperature.Should().Be(temperature);
            forecastForDay.Humidity.Should().Be(humidity);
        }
    }
}
