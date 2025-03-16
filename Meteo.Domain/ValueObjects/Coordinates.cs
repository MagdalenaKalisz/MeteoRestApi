namespace Meteo.Domain.ValueObjects
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Library.Domain;

    /// <summary>
    /// Value Object representing coordinates.
    /// </summary>
    public sealed class Coordinates : ValueObject
    {
        /// <summary>
        /// The latitude of the coordinates.
        /// </summary>
        public Latitude Latitude { get; }

        /// <summary>
        /// The longitude of the coordinates.
        /// </summary>
        public Longitude Longitude { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinates"/> class.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        private Coordinates(Latitude latitude, Longitude longitude)
        {
            ArgumentNullException.ThrowIfNull(latitude);
            ArgumentNullException.ThrowIfNull(longitude);

            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Factory method to create a new Coordinates instance.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static Coordinates Create(Latitude latitude, Longitude longitude)
        {
            return new Coordinates(latitude, longitude);
        }

        /// <summary>
        /// Factory method to create a new Coordinates instance.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(Latitude? latitude,
                                     Longitude? longitude,
                                     [MaybeNullWhen(false)] out Coordinates instance)
        {
            if (latitude is null ||
                longitude is null)
            {
                instance = null;
                return false;
            }

            instance = Create(latitude, longitude);

            return true;
        }

        /// <summary>
        /// Factory method to create a new Coordinates instance.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(double latitude,
                                     double longitude,
                                     [MaybeNullWhen(false)] out Coordinates instance)
        {
            if (!Latitude.TryCreate(latitude, out Latitude? lat) ||
                !Longitude.TryCreate(longitude, out Longitude? lon))
            {
                instance = null;
                return false;
            }

            instance = new Coordinates(lat, lon);
            return true;
        }

        /// <summary>
        /// Get the hash code for the object.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }

        /// <summary>
        /// Get the string representation of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Create(CultureInfo.InvariantCulture, $"({Latitude}, {Longitude})");
        }
    }
}
