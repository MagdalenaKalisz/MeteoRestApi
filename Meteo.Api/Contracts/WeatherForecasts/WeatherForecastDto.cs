namespace Meteo.Api.Contracts.WeatherForecasts
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Meteo.Api.Contracts.WeatherForecastDefinitions;

    /// <summary>
    /// Represents the response for a weather forecast.
    /// </summary>
    public sealed class WeatherForecastDto
    {
        /// <summary>
        /// The identifier of the weather forecast.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The creation timestamp of the weather forecast.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// The identifier of the associated weather forecast definition.
        /// </summary>
        [JsonPropertyName("definitionId")]
        public Guid DefinitionId { get; set; }

        /// <summary>
        /// The list of hourly weather forecasts.
        /// </summary>
        [JsonPropertyName("forecastsForDay")]
        public List<WeatherForecastForDayDto>? ForecastsForDay { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastDto"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="definitionId"></param>
        /// <param name="forecasts"></param>
        public WeatherForecastDto(Guid id, DateTimeOffset createdAt, Guid definitionId, List<WeatherForecastForDayDto>? forecasts)
        {
            Id = id;
            CreatedAt = createdAt;
            DefinitionId = definitionId;
            ForecastsForDay = forecasts;
        }
    }
}
