namespace Meteo.IntegrationTests.Infrastructure
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Moq.Protected;
    using Xunit;
    using FluentAssertions;
    using Meteo.Infrastructure.OpenMeteo;
    using Meteo.Application.OpenMeteo;
    using Meteo.Application.OpenMeteo.Dto;
    using Domain.ValueObjects;
    using System.Text.Json;
    using Meteo.Domain.ValueObjects;


    /// <summary>
    /// Unit tests for OpenMeteoClient.
    /// </summary>
    public class OpenMeteoClientTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<OpenMeteoClient>> _mockLogger;
        private readonly OpenMeteoClient _client;

        /// <summary>
        /// Initializes a new instance of the OpenMeteoClientTests class.
        /// </summary>
        public OpenMeteoClientTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.open-meteo.com/"),
            };
            _mockLogger = new Mock<ILogger<OpenMeteoClient>>();
            _client = new OpenMeteoClient(_httpClient, _mockLogger.Object);
        }

        /// <summary>
        /// Tests retrieving weather data successfully.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_Should_Return_WeatherData_When_ApiCall_Is_Successful()
        {
            var coordinates = Coordinates.Create(52.5200, 13.4050);
            var forecastDays = ForecastDaysAmount.Create(3);
            var responseContent = JsonSerializer.Serialize(new OpenMeteoWeatherResponse());

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent),
                });

            var result = await _client.GetWeatherAsync(coordinates, forecastDays);

            result.Should().NotBeNull();
        }

        /// <summary>
        /// Tests that the method returns null when the API request fails.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_Should_Return_Null_When_ApiCall_Fails()
        {
            var coordinates = Coordinates.Create(48.8566, 2.3522);
            var forecastDays = ForecastDaysAmount.Create(5);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request"),
                });

            var result = await _client.GetWeatherAsync(coordinates, forecastDays);

            result.Should().BeNull();
        }

        /// <summary>
        /// Tests handling a canceled request.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_Should_Throw_When_Request_Is_Canceled()
        {
            var coordinates = Coordinates.Create(40.7128, -74.0060);
            var forecastDays = ForecastDaysAmount.Create(7);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new OperationCanceledException());

            Func<Task> act = async () => await _client.GetWeatherAsync(coordinates, forecastDays);

            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        /// <summary>
        /// Tests handling an exception during the API call.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_Should_Return_Null_When_Exception_Occurs()
        {
            var coordinates = Coordinates.Create(34.0522, -118.2437);
            var forecastDays = ForecastDaysAmount.Create(4);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Network Error"));

            var result = await _client.GetWeatherAsync(coordinates, forecastDays);

            result.Should().BeNull();
        }
        /// <summary>
        /// Tests handling an invalid JSON response.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_Should_Return_Null_When_Invalid_Json_Response()
        {
            var coordinates = Coordinates.Create(51.5074, -0.1278);
            var forecastDays = ForecastDaysAmount.Create(3);
            const string invalidJson = "{ invalid json }";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(invalidJson),
                });

            var result = await _client.GetWeatherAsync(coordinates, forecastDays);

            result.Should().BeNull();
        }

        /// <summary>
        /// Tests handling a null response from the API.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_Should_Return_Null_When_Response_Is_Null()
        {
            var coordinates = Coordinates.Create(37.7749, -122.4194);
            var forecastDays = ForecastDaysAmount.Create(2);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpResponseMessage?)null!);

            var result = await _client.GetWeatherAsync(coordinates, forecastDays);

            result.Should().BeNull();
        }
    }
}