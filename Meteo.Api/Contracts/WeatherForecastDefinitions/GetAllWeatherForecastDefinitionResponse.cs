namespace Meteo.Api.Contracts.WeatherForecastDefinitions
{
    using Meteo.Application.Models;

    /// <summary>
    /// Represents the response for fetching weather forecast definitions.
    /// </summary>
    public sealed class GetAllWeatherForecastDefinitionResponse
    {
        /// <summary>
        /// The identifier of the weather forecast definition.
        /// </summary>
        public List<WeatherForecastDefinitionDto> WeatherForecastDefinitions { get; set; } = [];
    }
}
