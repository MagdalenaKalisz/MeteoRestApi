namespace Meteo.Domain.Enums
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents temperature measurement units.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TemperatureUnit
    {
        /// <summary>
        /// The Celsius temperature unit (°C).
        /// </summary>
        Celsius,

        /// <summary>
        /// The Fahrenheit temperature unit (°F).
        /// </summary>
        Fahrenheit,

        /// <summary>
        /// The Kelvin temperature unit (K).
        /// </summary>
        Kelvin,
    }
}
