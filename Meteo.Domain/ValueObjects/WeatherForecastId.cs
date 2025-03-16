namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Library.Domain;

    /// <summary>
    /// Value Object representing a weather forecast id.
    /// </summary>
    public sealed class WeatherForecastId : ValueObject, IId
    {
        /// <summary>
        /// The value of the weather forecast id.
        /// </summary>
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastId"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        private WeatherForecastId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Weather forecast id cannot be empty.", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Factory method to create a new WeatherForecastId instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WeatherForecastId Create(Guid value)
        {
            return new WeatherForecastId(value);
        }

        /// <summary>
        /// Factory method to create a new WeatherForecastId instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(Guid value, [MaybeNullWhen(false)] out WeatherForecastId instance)
        {
            if (value == Guid.Empty)
            {
                instance = null;
                return false;
            }

            instance = Create(value);
            return true;
        }

        /// <summary>
        /// Get the equality components of the value object.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        /// <summary>
        /// Returns the string representation of the value object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Implicit conversion from WeatherForecastId to Guid.
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator Guid(WeatherForecastId id)
        {
            return id.Value;
        }

        /// <summary>
        /// Implicit conversion from Guid to WeatherForecastId.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator WeatherForecastId(Guid value)
        {
            return Create(value);
        }
    }
}
