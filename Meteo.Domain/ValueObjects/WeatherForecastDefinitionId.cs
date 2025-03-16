namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Library.Domain;

    /// <summary>
    /// Value Object representing a weather forecast definition id.
    /// </summary>
    public sealed class WeatherForecastDefinitionId : ValueObject, IId
    {
        /// <inheritdoc/>
        public Guid Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastDefinitionId"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        private WeatherForecastDefinitionId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Weather forecast id cannot be empty.", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Factory method to create a new WeatherForecastDefinitionId instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WeatherForecastDefinitionId Create(Guid value)
        {
            return new WeatherForecastDefinitionId(value);
        }

        /// <summary>
        /// Factory method to create a new WeatherForecastDefinitionId instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(Guid value, [MaybeNullWhen(false)] out WeatherForecastDefinitionId instance)
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
        /// Get the string representation of the value object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        /// <summary>
        /// Implicitly convert a WeatherForecastDefinitionId instance to a Guid.
        /// </summary>
        /// <param name="id"></param>
        public static implicit operator Guid(WeatherForecastDefinitionId id)
        {
            return id.Value;
        }

        /// <summary>
        /// Implicitly convert a Guid to a WeatherForecastDefinitionId instance.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator WeatherForecastDefinitionId(Guid value)
        {
            return Create(value);
        }
    }
}
