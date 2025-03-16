namespace Meteo.Persistence.PostgreSql.Dao
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Library.Persistence;


    /// <summary>
    /// Data Access Object (DAO) for weather forecast definitions, representing location-based weather definitions.
    /// </summary>
    [Table("WeatherForecastDefinitions")]
    public sealed class WeatherForecastDefinitionDao : IDaoWithId
    {
        /// <summary>
        /// Unique identifier for the weather forecast definition.
        /// Matches WeatherForecastDefinitionId.Value.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Timestamp indicating when the definition record was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Latitude coordinate of the forecast location.
        /// Matches Coordinates.Latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate of the forecast location.
        /// Matches Coordinates.Longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Parameterless constructor required by Entity Framework.
        /// </summary>
        public WeatherForecastDefinitionDao() { }

        /// <summary>
        /// Constructor for initializing WeatherForecastDefinitionDao with the required properties.
        /// </summary>
        /// <param name="id">Unique identifier for the weather forecast definition.</param>
        /// <param name="createdAt">Timestamp indicating when the definition record was created.</param>
        /// <param name="latitude">Latitude coordinate of the forecast location.</param>
        /// <param name="longitude">Longitude coordinate of the forecast location.</param>
        public WeatherForecastDefinitionDao(Guid id, DateTimeOffset createdAt, double latitude, double longitude)
        {
            Id = id;
            CreatedAt = createdAt;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
