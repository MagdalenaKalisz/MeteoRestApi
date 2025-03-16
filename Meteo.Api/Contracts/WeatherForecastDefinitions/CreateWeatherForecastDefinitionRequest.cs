namespace Meteo.Api.Contracts.WeatherForecastDefinitions
{
    using System.ComponentModel.DataAnnotations;


    /// <summary>
    /// Create weather forecast definition request.
    /// </summary>
    public sealed class CreateWeatherForecastDefinitionRequest
    {
        /// <summary>
        /// Coordinates.
        /// </summary>
        public CoordinatesDto? Coordinates { get; set; }

        /// <summary>
        /// Email (optional).
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
    }
}
