namespace Meteo.Infrastructure.OpenMeteo
{
    using System.Text.Json;
    using Domain.ValueObjects;
    using Meteo.Application.OpenMeteo;
    using Meteo.Application.OpenMeteo.Dto;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Default implementation of the <see cref="IOpenMeteoClient"/> interface.
    /// </summary>
    public sealed class OpenMeteoClient : IOpenMeteoClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenMeteoClient> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenMeteoClient"/> class.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public OpenMeteoClient(HttpClient httpClient, ILogger<OpenMeteoClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<OpenMeteoWeatherResponse?> GetWeatherAsync(
            Coordinates coordinates,
            ForecastDaysAmount forecastDays,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(coordinates);
            ArgumentNullException.ThrowIfNull(forecastDays);

            string url = $"forecast?latitude={coordinates.Latitude}&longitude={coordinates.Longitude}" +
                         $"&hourly=temperature_2m,relative_humidity_2m&forecast_days={forecastDays.Value}";

            _logger.LogDebug("Fetching data from OpenMeteo API: {Url}", url);

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogError("API request failed. Status: {StatusCode}, Content: {Content}",
                                     response.StatusCode, errorContent);
                    return null;
                }

                string content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogTrace("API Response: {Content}", content);

                OpenMeteoWeatherResponse? deserializedResponse = JsonSerializer.Deserialize<OpenMeteoWeatherResponse>(content);

                if (deserializedResponse is null)
                {
                    _logger.LogWarning("OpenMeteo response was null after deserialization. Content: {Content}", content);

                    return null;
                }

                return deserializedResponse;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Fetching weather data was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching weather data from OpenMeteo API.");
                return null;
            }
        }
    }
}
