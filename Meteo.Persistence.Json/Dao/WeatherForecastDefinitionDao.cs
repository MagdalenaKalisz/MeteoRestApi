namespace Meteo.Persistence.Json.Dao
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Weather forecast definition for JSON storage.
    /// </summary>
    public sealed class WeatherForecastDefinitionDao
    {
        /// <summary>
        /// Forecast definition ID.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Creation timestamp.
        /// </summary>
        public DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Latitude coordinate.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Longitude coordinate.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// JSON Constructor for Deserialization
        /// </summary>
        [JsonConstructor]
        public WeatherForecastDefinitionDao(Guid id, DateTimeOffset createdAt, double latitude, double longitude)
        {
            Id = id;
            CreatedAt = createdAt;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
