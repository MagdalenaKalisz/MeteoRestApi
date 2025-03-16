namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Library.Domain;

    /// <summary>
    /// Value Object representing latitude.
    /// </summary>
    public sealed class Latitude : ValueObject
    {
        /// <summary>
        /// The value of the latitude.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Latitude"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        private Latitude(double value)
        {
            if (value is < (-90.0) or > 90.0)
            {
                throw new ArgumentException("Latitude must be between -90 and 90 degrees.", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Factory method to create a new Latitude instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Latitude Create(double value)
        {
            return new Latitude(value);
        }

        /// <summary>
        /// Factory method to create a new Latitude instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(double value, [MaybeNullWhen(false)] out Latitude instance)
        {
            if (value is < (-90.0) or > 90.0)
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
            return Value.ToString("F6", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="Longitude"/> to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="latitude"></param>
        public static implicit operator double(Latitude latitude)
        {
            return latitude.Value;
        }

        /// <summary>
        /// Converts the <see cref="double"/> to a <see cref="Latitude"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Latitude(double value)
        {
            return Create(value);
        }
    }
}
