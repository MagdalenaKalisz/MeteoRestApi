namespace Meteo.Application.Models;

using Meteo.Domain.Aggregates;

/// <summary>
/// Represents the result of a weather forecast definition.
/// </summary>
/// <param name="Definition"></param>
/// <param name="Forecasts"></param>
public sealed record WeatherForecastDefinitionResult(
    WeatherForecastDefinition Definition,
    List<WeatherForecast> Forecasts
);