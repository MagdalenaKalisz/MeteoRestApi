namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Library.Domain;

    /// <summary>
    /// Value Object representing a weather forecast for a day.
    /// </summary>
    public sealed class WeatherForecastForDay : ValueObject
    {
        /// <summary>
        /// Forecast date and time.
        /// </summary>
        public ForecastDateTime ForecastDateTime { get; }

        /// <summary>
        /// Temperature.
        /// </summary>
        public Temperature Temperature { get; }

        /// <summary>
        /// Humidity.
        /// </summary>
        public Humidity Humidity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastForDay"/> class.
        /// </summary>
        /// <param name="forecastDateTime"></param>
        /// <param name="temperature"></param>
        /// <param name="humidity"></param>
        private WeatherForecastForDay(ForecastDateTime forecastDateTime,
                                      Temperature temperature,
                                      Humidity humidity)
        {
            ArgumentNullException.ThrowIfNull(temperature);
            ArgumentNullException.ThrowIfNull(humidity);

            ForecastDateTime = forecastDateTime;
            Temperature = temperature;
            Humidity = humidity;
        }

        /// <summary>
        /// Factory method to create a new WeatherForecastForDay instance.
        /// </summary>
        /// <param name="forecastDateTime"></param>
        /// <param name="temperature"></param>
        /// <param name="humidity"></param>
        /// <returns></returns>
        public static WeatherForecastForDay Create(ForecastDateTime forecastDateTime, Temperature temperature, Humidity humidity)
        {
            return new WeatherForecastForDay(forecastDateTime, temperature, humidity);
        }

        /// <summary>
        /// Factory method to create a new WeatherForecastForDay instance.
        /// </summary>
        /// <param name="forecastDateTime"></param>
        /// <param name="temperature"></param>
        /// <param name="humidity"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(ForecastDateTime? forecastDateTime,
                                    Temperature? temperature,
                                     Humidity? humidity,
                                     [MaybeNullWhen(false)] out WeatherForecastForDay instance)
        {
            if (forecastDateTime is null || temperature is null || humidity is null)
            {
                instance = null;
                return false;
            }

            instance = new WeatherForecastForDay(forecastDateTime, temperature, humidity);
            return true;
        }

        /// <summary>
        /// Factory method to create a new WeatherForecastForDay instance.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="temperatureValue"></param>
        /// <param name="unit"></param>
        /// <param name="humidityPercentage"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(DateTimeOffset dateTime,
                                     double temperatureValue,
                                     TemperatureUnit unit,
                                     double humidityPercentage,
                                     [MaybeNullWhen(false)] out WeatherForecastForDay instance)
        {

            if (humidityPercentage is < 0 or > 100)
            {
                instance = null;
                return false;
            }

            if (!ForecastDateTime.TryCreate(dateTime, out ForecastDateTime? forecastDateTime) || !Temperature.TryCreate(temperatureValue, unit, out Temperature? temp) || !Humidity.TryCreate(humidityPercentage, out Humidity? humidity))
            {
                instance = null;
                return false;
            }

            instance = Create(forecastDateTime, temp, humidity);
            return true;
        }

        /// <summary>
        /// Get the equality components of the value object.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Temperature;
            yield return Humidity;
        }

        /// <summary>
        /// Get the string representation of the value object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Temperature: {Temperature}, Humidity: {Humidity}";
        }
    }
}
