namespace Meteo.Domain.Database;

/// <summary>
/// Represents the <see cref="WeatherForecastDefinition"/> collection (repository).
/// </summary>
public interface IWeatherForecastDefinitionCollection : IDatabaseCollection<WeatherForecastDefinition, WeatherForecastDefinitionId>
{
    /// <summary>
    /// Get a weather forecast definition by coordinates.
    /// </summary>
    /// <param name="coordinates"></param>
    /// <param name="queryBehavior"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<WeatherForecastDefinition?> GetByCoordinatesAsync(Coordinates coordinates,
                                                           QueryBehavior queryBehavior = QueryBehavior.Default,
                                                           CancellationToken cancellationToken = default);
}
