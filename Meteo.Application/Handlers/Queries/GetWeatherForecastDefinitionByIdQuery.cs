namespace Meteo.Application.Handlers.Queries
{
    using System.Data;
    using Library.Domain.Database;
    using Meteo.Application.Models;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Database;
    using Meteo.Domain.Enums;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Represents the query to get a weather forecast definition by ID.
    /// </summary>
    public sealed record GetWeatherForecastDefinitionByIdQuery(
        WeatherForecastDefinitionId Id,
        TemperatureUnit TemperatureUnit) : IQuery<WeatherForecastDefinitionResult?>
    {
        private sealed class Handler(
            IWeatherDatabaseSession _db) : IQueryHandler<GetWeatherForecastDefinitionByIdQuery, WeatherForecastDefinitionResult?>
        {
            public async Task<WeatherForecastDefinitionResult?> HandleAsync(GetWeatherForecastDefinitionByIdQuery query, CancellationToken cancellationToken)
            {
                WeatherForecastDefinition? definition =
                    await _db.WeatherForecastDefinitions.GetByIdAsync(
                        query.Id, QueryBehavior.NoTracking, cancellationToken);

                if (definition is null)
                {
                    return null;
                }

                List<WeatherForecast> forecasts =
                    await _db.WeatherForecasts.GetAllAsync(QueryBehavior.NoTracking, cancellationToken);

                List<WeatherForecast> matchingForecasts = forecasts
                    .Where(f => f.DefinitionId == definition.Id)
                    .ToList();

                List<WeatherForecast> convertedForecasts = [.. matchingForecasts
                    .Select(f => WeatherForecast.Create(
                        f.Id,
                        f.CreatedAt,
                        f.DefinitionId,
                        f.Forecasts.Select(day =>
                            WeatherForecastForDay.Create(
                                day.ForecastDateTime,
                                day.Temperature.ToUnit(query.TemperatureUnit),
                                day.Humidity
                            )
                        )
                    )),];

                return new WeatherForecastDefinitionResult(
                    Definition: definition,
                    Forecasts: convertedForecasts
                );
            }
        }
    }
}
