namespace Meteo.Persistence.Json.Dao
{
    using System.Text.Json.Serialization;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// DAO representation of the WeatherForecastDefinitionCreated event.
    /// </summary>
    public class WeatherForecastDefinitionCreatedDao
    {
        /// <summary>
        /// Weather Forecast Definition Id
        /// </summary>
        public Guid DefinitionId { get; set; }
        /// <summary>
        /// Weather Forecast Definition Latitude
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Weather Forecast Definition Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Constructor for WeatherForecastDefinitionCreatedDao
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        [JsonConstructor]
        public WeatherForecastDefinitionCreatedDao(Guid definitionId, double latitude, double longitude)
        {
            DefinitionId = definitionId;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
