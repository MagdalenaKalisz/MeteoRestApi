namespace Meteo.Api.Mappers
{
    using Meteo.Api.Contracts;

    using Meteo.Api.Contracts.WeatherForecastDefinitions;
    using Meteo.Api.Contracts.WeatherForecasts;
    using Meteo.Application.Models;
    using System.Linq;

    /// <summary>
    /// WeatherForecastMapper class for mapping.
    /// </summary>
    public static class WeatherForecastMapper
    {
        /// <summary>
        /// Maps WeatherForecastDefinitionResult to WeatherForecastDefinitionDto.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static WeatherForecastDefinitionDto MapToDefinitionDto(WeatherForecastDefinitionResult result)
        {
            return new WeatherForecastDefinitionDto(
                id: result.Definition.Id.Value,
                createdAt: result.Definition.CreatedAt.Value,
                coordinates: new CoordinatesDto
                {
                    Latitude = result.Definition.Coordinates.Latitude.Value,
                    Longitude = result.Definition.Coordinates.Longitude.Value,
                },
                forecasts: result.Forecasts.ConvertAll(f => new WeatherForecastDto(
                    id: f.Id.Value,
                    createdAt: f.CreatedAt.Value,
                    definitionId: f.DefinitionId.Value,
                    forecasts: [.. f.Forecasts.Select(forecast => new WeatherForecastForDayDto(
                        forecastDateTime: forecast.ForecastDateTime.DateTime,
                        temperature: forecast.Temperature.Value,
                        temperatureUnit: forecast.Temperature.Unit.ToString(),
                        humidity: forecast.Humidity.Percentage
                    )),]
                )));
        }

        /// <summary>
        /// Maps list of WeatherForecastDefinitionResult to list of WeatherForecastDefinitionDto.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static List<WeatherForecastDefinitionDto> MapToDefinitionsDto(List<WeatherForecastDefinitionResult> results)
        {
            return results.ConvertAll(MapToDefinitionDto);
        }

    }
}
