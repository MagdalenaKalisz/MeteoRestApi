namespace Meteo.Api.Contracts.WeatherForecastDefinitions
{
    /// <summary>
    /// Represents the response for creating a weather forecast definition.
    /// </summary>
    public sealed class CreateWeatherForecastDefinitionResponse
    {
        /// <summary>
        /// The identifier of the created weather forecast definition.
        /// </summary>
        public Guid Id { get; set; }
    }
}
