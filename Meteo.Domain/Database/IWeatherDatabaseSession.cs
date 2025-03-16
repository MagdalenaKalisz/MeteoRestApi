namespace Meteo.Domain.Database
{
    using Library.Domain.Database;

    /// <summary>
    /// Weather database (unit of work).
    /// </summary>
    public interface IWeatherDatabaseSession : IDatabaseSession
    {
        /// <summary>
        /// <see cref="Aggregates.WeatherForecast"/> collection.
        /// </summary>
        IWeatherForecastCollection WeatherForecasts { get; }

        /// <summary>
        /// <see cref="Aggregates.WeatherForecastDefinition"/> collection.
        /// </summary>
        IWeatherForecastDefinitionCollection WeatherForecastDefinitions { get; }
    }
}
