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
    /// Represents the query to get all weather forecast definitions.
    /// </summary>
    public sealed record GetAllWeatherForecastDefinitionQuery(
        TemperatureUnit TemperatureUnit,
        int Page,
        int PerPage) : IQuery<List<WeatherForecastDefinitionResult>>
    {
        private sealed class Handler(
            IWeatherDatabaseSession _db) : IQueryHandler<GetAllWeatherForecastDefinitionQuery, List<WeatherForecastDefinitionResult>>
        {
            public async Task<List<WeatherForecastDefinitionResult>> HandleAsync(
                GetAllWeatherForecastDefinitionQuery query,
                CancellationToken cancellationToken)
            {
                List<WeatherForecastDefinition> definitions =
                    await _db.WeatherForecastDefinitions.GetAllAsync(QueryBehavior.NoTracking, cancellationToken);

                List<WeatherForecast> forecasts =
                    await _db.WeatherForecasts.GetAllAsync(QueryBehavior.NoTracking, cancellationToken);

                List<WeatherForecast> convertedForecasts = [.. forecasts
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

                Dictionary<WeatherForecastDefinitionId, List<WeatherForecast>> forecastsByDefinitionId = convertedForecasts
                    .GroupBy(cf => cf.DefinitionId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                List<WeatherForecastDefinitionResult> results = [.. definitions
                    .Select(def =>
                        new WeatherForecastDefinitionResult(
                            Definition: def,
                            Forecasts: forecastsByDefinitionId.TryGetValue(def.Id, out List<WeatherForecast>? groupedForecasts)
                                ? groupedForecasts
                                : []
                        )
                    ),
                ];

                return [.. results
                    .Skip((query.Page - 1) * query.PerPage)
                    .Take(query.PerPage),];
            }
        }
    }
}
