namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Library.Domain;

    /// <summary>
    /// Value Object representing longitude.
    /// </summary>
    public sealed class Longitude : ValueObject
    {
        /// <summary>
        /// Longitude value in degrees.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Longitude"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        private Longitude(double value)
        {
            Validate(value);
            Value = value;
        }

        /// <summary>
        /// Factory method to create a new <see cref="Longitude"/> instance.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Longitude Create(double value)
        {
            return new Longitude(value);
        }

        /// <summary>
        /// Factory method to create a new <see cref="Longitude"/> instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(double value, [MaybeNullWhen(false)] out Longitude instance)
        {
            try
            {
                Validate(value);
                instance = Create(value);
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        /// <summary>
        /// Validates the longitude value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void Validate(double value)
        {
            if (value is < -180.0 or > 180.0)
            {
                throw new ArgumentException("Longitude must be between -180 and 180 degrees.", nameof(value));
            }
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
        /// <param name="longitude"></param>
        public static implicit operator double(Longitude longitude)
        {
            return longitude.Value;
        }

        /// <summary>
        /// Converts the <see cref="double"/> to a <see cref="Longitude"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Longitude(double value)
        {
            return Create(value);
        }
    }
}
