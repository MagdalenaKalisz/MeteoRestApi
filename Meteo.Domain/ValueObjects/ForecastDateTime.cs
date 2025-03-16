namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Library.Domain;

    /// <summary>
    /// Value Object representing time in a forecast.
    /// </summary>
    public sealed class ForecastDateTime : ValueObject
    {
        /// <summary>
        /// Forecast time.
        /// </summary>
        public DateTimeOffset DateTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForecastDateTime"/> class.
        /// </summary>
        private ForecastDateTime(DateTimeOffset datetime)
        {
            DateTime = datetime;
        }

        /// <summary>
        /// Factory method to create a new ForecastDateTime instance.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static ForecastDateTime Create(DateTimeOffset dateTime)
        {
            return new ForecastDateTime(dateTime);
        }

        /// <summary>
        /// Factory method to create a new ForecastDateTime instance.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(DateTimeOffset dateTime, [MaybeNullWhen(false)] out ForecastDateTime instance)
        {
            try
            {
                Validate(dateTime);
                instance = Create(dateTime);
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        /// <summary>
        /// Factory method to create a new ForecastDateTime instance.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(string dateTime, [MaybeNullWhen(false)] out ForecastDateTime instance)
        {
            try
            {
                if (!DateTimeOffset.TryParse(dateTime, CultureInfo.InvariantCulture, out DateTimeOffset parsedDateTime))
                {
                    throw new FormatException("Invalid date time format.");
                }

                Validate(parsedDateTime);
                instance = Create(parsedDateTime);
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        /// <summary>
        /// Validates the date and time.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static void Validate(DateTimeOffset dateTime)
        {
            if (dateTime.Hour is < 0 or > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime), "Hour must be between 00 and 23.");
            }

            if (dateTime.Minute is < 0 or > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime), "Minute must be between 00 and 59.");
            }

            if (dateTime.Second is < 0 or > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(dateTime), "Second must be between 00 and 59.");
            }
        }

        /// <summary>
        /// Returns the date and time as a string.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DateTime;
        }

        /// <summary>
        /// /// Get the string representation of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DateTime.ToString("HH:mm", CultureInfo.InvariantCulture);
        }
    }
}
