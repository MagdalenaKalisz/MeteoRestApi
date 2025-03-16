namespace Meteo.Persistence.Json.Configuration
{
    /// <summary>
    /// Represents configuration settings for the JSON-based outbox repository.
    /// </summary>
    public class JsonDatabaseSettings
    {
        /// <summary>
        /// Gets or sets the file name for storing outbox messages.
        /// </summary>
        public string OutboxFileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name for storing Weather Forecasts.
        /// </summary>
        public string WeatherForecastFileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file name for storing Weather Forecasts Definition.
        /// </summary>
        public string WeatherForecastDefinitionFileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        public string BasePath {get; set;} = string.Empty;
    }
}
