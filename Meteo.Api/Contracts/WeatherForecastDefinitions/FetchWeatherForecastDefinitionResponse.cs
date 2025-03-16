namespace Meteo.Api.Contracts.WeatherForecastDefinitions
{
    using Meteo.Api.Contracts.WeatherForecasts;

    using Meteo.Application.Models;

    /// <summary>
    /// Represents the response for fetching weather forecast definitions.
    /// </summary>
    public sealed class FetchWeatherForecastDefinitionResponse
    {
        /// <summary>
        /// Weather forecast definition.
        /// </summary>
        public WeatherForecastDefinitionDto? WeatherForecastDefinition { get; set; }
    }
}
