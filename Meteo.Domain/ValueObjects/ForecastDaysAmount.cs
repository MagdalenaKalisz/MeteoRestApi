namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Library.Domain;

    /// <summary>
    /// Value Object representing the amount of days in a forecast.
    /// </summary>
    public sealed class ForecastDaysAmount : ValueObject
    {
        /// <summary>
        /// The value.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForecastDaysAmount"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private ForecastDaysAmount(int value)
        {
            if (value is < 0 or > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Forecast days must be between 0 and 16.");
            }

            Value = value;
        }

        /// <summary>
        /// Factory method to create a new ForecastDaysAmount instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ForecastDaysAmount Create(int value)
        {
            return new ForecastDaysAmount(value);
        }

        /// <summary>
        /// Factory method to create a new ForecastDaysAmount instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(int value, [MaybeNullWhen(false)] out ForecastDaysAmount instance)
        {
            if (value is < 0 or > 16)
            {
                instance = null;
                return false;
            }

            instance = Create(value);
            return true;
        }

        /// <summary>
        ///  Get the equality components of the value object.
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
            return string.Create(CultureInfo.InvariantCulture, $"{Value}");
        }
    }
}
