namespace Meteo.Domain.Database;

/// <summary>
/// Represents the <see cref="WeatherForecast"/> collection (repository).
/// </summary>
public interface IWeatherForecastCollection : IDatabaseCollection<WeatherForecast, WeatherForecastId>
{
    /// <summary>
    /// Get a weather forecast by definition Id
    /// </summary>
    /// <param name="definitionId"></param>
    /// <param name="queryBehavior"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<WeatherForecast?> GetByDefinitionIdAsync(WeatherForecastDefinitionId definitionId,
    QueryBehavior queryBehavior = QueryBehavior.Default,
    CancellationToken cancellationToken = default);
}
