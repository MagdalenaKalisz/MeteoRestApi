namespace Meteo.Api.Contracts.WeatherForecastDefinitions
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Meteo.Api.Contracts.WeatherForecasts;

    /// <summary>
    /// Represents the response for a weather forecast definition.
    /// </summary>
    public sealed class WeatherForecastDefinitionDto
    {
        /// <summary>
        /// Unique identifier for the weather forecast definition.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Timestamp indicating when the definition was created.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// The coordinates of the forecast location.
        /// </summary>
        [JsonPropertyName("coordinates")]
        public CoordinatesDto? Coordinates { get; set; }

        /// <summary>
        /// The list of weather forecasts for the specified days.
        /// </summary>
        [JsonPropertyName("forecasts")]
        public List<WeatherForecastDto>? Forecasts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherForecastDefinitionDto"/> class.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="createdAt"></param>
        /// <param name="coordinates"></param>
        /// <param name="forecasts"></param>
        public WeatherForecastDefinitionDto(Guid id, DateTimeOffset createdAt, CoordinatesDto? coordinates, List<WeatherForecastDto>? forecasts)
        {
            Id = id;
            CreatedAt = createdAt;
            Coordinates = coordinates;
            Forecasts = forecasts;
        }
    }
}
