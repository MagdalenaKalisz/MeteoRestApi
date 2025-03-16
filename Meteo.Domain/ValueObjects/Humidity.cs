namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Library.Domain;

    /// <summary>
    /// Value Object representing humidity.
    /// </summary>
    public sealed class Humidity : ValueObject
    {
        /// <summary>
        /// Humidity percentage.
        /// </summary>
        public double Percentage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Humidity"/> class.
        /// </summary>
        /// <param name="percentage"></param>
        private Humidity(double percentage)
        {
            Validate(percentage);
            Percentage = percentage;
        }

        /// <summary>
        /// Factory method to create a new Humidity instance.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static Humidity Create(double percentage)
        {
            return new Humidity(percentage);
        }

        /// <summary>
        /// Factory method to create a new Humidity instance.
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(double percentage, [MaybeNullWhen(false)] out Humidity instance)
        {
            try
            {
                Validate(percentage);
                instance = Create(percentage);
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        /// <summary>
        /// Validates the humidity percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void Validate(double percentage)
        {
            if (percentage is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(percentage), "Humidity must be between 0 and 100%.");
            }
        }

        /// <summary>
        /// Returns the humidity percentage as a string.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Percentage;
        }

        /// <summary>
        /// Get the string representation of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Create(CultureInfo.InvariantCulture, $"{Percentage}%");
        }

        /// <summary>
        /// Converts the <see cref="Humidity"/> to a <see cref="double"/>.
        /// </summary>
        /// <param name="humidity"></param>
        public static implicit operator double(Humidity humidity)
        {
            return humidity.Percentage;
        }

        /// <summary>
        /// Converts the <see cref="double"/> to a <see cref="Humidity"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Humidity(double value)
        {
            return Create(value);
        }
    }
}
