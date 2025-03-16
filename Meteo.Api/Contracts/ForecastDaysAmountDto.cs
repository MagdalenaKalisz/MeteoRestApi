namespace Meteo.Api.Contracts
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Coordinates.
    /// </summary>
    public sealed class ForecastDaysAmountDto
    {
        /// <summary>
        /// Days Amount
        /// </summary>
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}