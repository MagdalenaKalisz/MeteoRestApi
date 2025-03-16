
namespace Meteo.IntegrationTests.Api
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Library.Buses;
    using Library.Domain.ValueObjects;

    using Meteo.Api.Contracts;
    using Meteo.Api.Contracts.WeatherForecastDefinitions;
    using Meteo.Api.Mappers;

    using Meteo.Application.Handlers.Commands;
    using Meteo.Application.Handlers.Queries;

    using Meteo.Application.Models;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Enums;

    using Meteo.Domain.ValueObjects;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for Weather API endpoints.
    /// </summary>
    public class WeatherDefinitionsApiTests
    {
        private readonly Mock<IBus> _mockBus = new();

        /// <summary>
        /// Tests that a weather forecast definition is created successfully.
        /// </summary>
        [Fact]
        public async Task Should_Create_WeatherDefinition_When_Valid_Request()
        {
            var request = new CreateWeatherForecastDefinitionRequest
            {
                Coordinates = new CoordinatesDto { Latitude = 52.5200, Longitude = 13.4050 },
                Email = "test@example.com",
            };

            var coordinates = Coordinates.Create(Latitude.Create(52.5200), Longitude.Create(13.4050));
            var forecastId = WeatherForecastDefinitionId.Create(Guid.NewGuid());

            _mockBus
                .Setup(bus => bus.ExecuteAsync(It.IsAny<CreateWeatherForecastDefinitionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecastId);

            var result = await CreateWeatherDefinitionHandler(request, _mockBus.Object, CancellationToken.None);

            var okResult = result.Should().BeOfType<Ok<CreateWeatherForecastDefinitionResponse>>().Which;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.Id.Should().Be(forecastId);

        }

        /// <summary>
        /// Tests that the API returns a validation error when coordinates are invalid.
        /// </summary>
        [Fact]
        public async Task Should_Return_ValidationError_When_Coordinates_Are_Invalid()
        {
            var request = new CreateWeatherForecastDefinitionRequest
            {
                Coordinates = new CoordinatesDto { Latitude = 999.0, Longitude = 13.4050 },
                Email = "test@example.com",
            };

            var result = await CreateWeatherDefinitionHandler(request, _mockBus.Object, CancellationToken.None);

            var validationResult = result.Should().BeOfType<ProblemHttpResult>().Subject;
            validationResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// The actual handler logic extracted for unit testing.
        /// </summary>
        private static async Task<IResult> CreateWeatherDefinitionHandler(
            CreateWeatherForecastDefinitionRequest body,
            IBus bus,
            CancellationToken cancellationToken)
        {
            (Coordinates? coordinates, IResult validationResult) = ValidateCoordinates(body.Coordinates);

            if (coordinates is null)
            {
                return validationResult;
            }

            CreateWeatherForecastDefinitionCommand command = new(coordinates, body.Email);

            WeatherForecastDefinitionId? weatherForecastDefinitionId = await bus.ExecuteAsync(command, cancellationToken);

            if (weatherForecastDefinitionId is null)
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["coordinates"] = ["Weather Definition with these coordinates already exists."],
                    });
            }

            return Results.Ok(new CreateWeatherForecastDefinitionResponse { Id = weatherForecastDefinitionId });
        }

        /// <summary>
        /// Should fetch weather forecasts when valid request.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Fetch_WeatherForecasts_When_Valid_Request()
        {
            var request = new FetchWeatherForecastDefinitionRequest
            {
                Coordinates = new CoordinatesDto { Latitude = 52.5200, Longitude = 13.4050 },
                ForecastDaysAmount = new ForecastDaysAmountDto { Value = 3 },
                Email = "test@example.com",
            };

            var coordinates = Coordinates.Create(Latitude.Create(52.5200), Longitude.Create(13.4050));
            var definition = WeatherForecastDefinition.Create(
            WeatherForecastDefinitionId.Create(Guid.NewGuid()),
            CreationTimestamp.Create(DateTimeOffset.UtcNow),
            Coordinates.Create(Latitude.Create(52.5200), Longitude.Create(13.4050))
        );

            var forecasts = new List<WeatherForecast>(); // Provide empty or mock forecasts as needed.

            var forecastResult = new WeatherForecastDefinitionResult(definition, forecasts);


            _mockBus.Setup(bus => bus.ExecuteAsync(It.IsAny<FetchWeatherForecastDefinitionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecastResult);

            var result = await FetchWeatherForecastsHandler(request, _mockBus.Object, CancellationToken.None);

            var okResult = result.Should().BeOfType<Ok<FetchWeatherForecastDefinitionResponse>>().Which;
            okResult.Value.Should().NotBeNull();
        }

        /// <summary>
        /// Should return validation error when fetch coordinates are invalid.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Return_ValidationError_When_Fetch_Coordinates_Are_Invalid()
        {
            var request = new FetchWeatherForecastDefinitionRequest
            {
                Coordinates = new CoordinatesDto { Latitude = 999.0, Longitude = 13.4050 },
                Email = "test@example.com",
            };

            var result = await FetchWeatherForecastsHandler(request, _mockBus.Object, CancellationToken.None);

            result.Should().BeOfType<ProblemHttpResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Should delete weather forecast definition when valid id.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Delete_WeatherForecastDefinition_When_Valid_Id()
        {
            var forecastId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            _mockBus.Setup(bus => bus.ExecuteAsync(It.IsAny<DeleteWeatherForecastDefinitionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: true);

            var result = await DeleteWeatherForecastDefinitionHandler(forecastId, _mockBus.Object, CancellationToken.None);

            result.Should().BeOfType<Ok>();
        }

        /// <summary>
        /// Should return validation error when deleting invalid id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Return_ValidationError_When_Deleting_Invalid_Id()
        {
            var result = await DeleteWeatherForecastDefinitionHandler(Guid.Empty, _mockBus.Object, CancellationToken.None);

            result.Should().BeOfType<ProblemHttpResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Should delete weather forecast definition by coordinates
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Delete_WeatherForecastDefinition_By_Coordinates()
        {
            const double latitude = 52.5200;
            const double longitude = 13.4050;

            _mockBus.Setup(bus => bus.ExecuteAsync(It.IsAny<DeleteWeatherForecastDefinitionByCoordinatesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value: true);

            var result = await DeleteWeatherForecastDefinitionByCoordinatesHandler(latitude, longitude, _mockBus.Object, CancellationToken.None);

            result.Should().BeOfType<Ok<Dictionary<string, string[]>>>();
        }

        /// <summary>
        /// Should return validation error when deleting invalid coordinates
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Return_ValidationError_When_Deleting_Invalid_Coordinates()
        {
            const double latitude = 999.0;
            const double longitude = 13.4050;

            var result = await DeleteWeatherForecastDefinitionByCoordinatesHandler(latitude, longitude, _mockBus.Object, CancellationToken.None);

            result.Should().BeOfType<ProblemHttpResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Should delete all weather forecast definitions
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Delete_All_WeatherForecastDefinitions()
        {
            _mockBus.Setup(bus => bus.ExecuteAsync(It.IsAny<DeleteAllWeatherForecastDefinitionsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await DeleteAllWeatherForecastDefinitionsHandler(_mockBus.Object, CancellationToken.None);

            result.Should().BeOfType<Ok<Dictionary<string, string[]>>>();
        }

        /// <summary>
        /// Should get all weather forecast definitions
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Get_All_WeatherForecastDefinitions()
        {
            var forecastDefinitions = new List<WeatherForecastDefinitionResult>();
            _mockBus.Setup(bus => bus.ExecuteAsync(It.IsAny<GetAllWeatherForecastDefinitionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecastDefinitions);

            var result = await GetAllWeatherForecastDefinitionsHandler(_mockBus.Object, CancellationToken.None);

            result.Should().BeOfType<Ok<GetAllWeatherForecastDefinitionResponse>>();
        }

        /// <summary>
        /// Should get weather forecast by id when valid request
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Get_WeatherForecast_By_Id_When_Valid_Request()
        {
            var forecastId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var createdAt = CreationTimestamp.Create(DateTime.UtcNow);
            var coordinates = Coordinates.Create(Latitude.Create(52.5200), Longitude.Create(13.4050));

            var weatherForecastDefinition = WeatherForecastDefinition.Create(definitionId, createdAt, coordinates);

            var forecastResult = new WeatherForecastDefinitionResult(weatherForecastDefinition, new List<WeatherForecast>());

            _mockBus.Setup(bus => bus.ExecuteAsync(It.IsAny<GetWeatherForecastDefinitionByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecastResult);

            var result = await GetWeatherForecastByIdHandler(forecastId.Value, TemperatureUnit.Celsius, _mockBus.Object, CancellationToken.None);

            var okResult = result.Should().BeOfType<Ok<GetWeatherForecastDefinitionResponse>>().Which;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.WeatherForecastDefinition.Should().BeEquivalentTo(WeatherForecastMapper.MapToDefinitionDto(forecastResult));

        }

        /// <summary>
        /// Should return validation error when invalid id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Return_ValidationError_When_Invalid_Id()
        {
            var result = await GetWeatherForecastByIdHandler(Guid.Empty, TemperatureUnit.Celsius, _mockBus.Object, CancellationToken.None);
            result.Should().BeOfType<ProblemHttpResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Should get weather forecast by coordinates when valid request
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_Get_WeatherForecast_By_Coordinates_When_Valid_Request()
        {
            var forecastId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var definitionId = WeatherForecastDefinitionId.Create(Guid.NewGuid());
            var createdAt = CreationTimestamp.Create(DateTime.UtcNow);
            var coordinates = Coordinates.Create(Latitude.Create(52.5200), Longitude.Create(13.4050));

            var weatherForecastDefinition = WeatherForecastDefinition.Create(definitionId, createdAt, coordinates);
            var forecastResult = new WeatherForecastDefinitionResult(weatherForecastDefinition, new List<WeatherForecast>());

            _mockBus.Setup(bus => bus.ExecuteAsync(It.IsAny<GetWeatherForecastDefinitionByCoordinatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecastResult);

            var result = await GetWeatherForecastByCoordinatesHandler(52.5200, 13.4050, TemperatureUnit.Celsius, _mockBus.Object, CancellationToken.None);

            var okResult = result.Should().BeOfType<Ok<GetWeatherForecastDefinitionResponse>>().Which;
            okResult.Value.Should().NotBeNull();
            okResult.Value!.WeatherForecastDefinition.Should().BeEquivalentTo(WeatherForecastMapper.MapToDefinitionDto(forecastResult));
        }


        private static async Task<IResult> GetWeatherForecastByIdHandler(Guid id, TemperatureUnit temperatureUnit, IBus bus, CancellationToken cancellationToken)
        {
            if (!WeatherForecastDefinitionId.TryCreate(id, out WeatherForecastDefinitionId? validatedId))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["id"] = ["Weather forecast definition identifier is invalid."] });
            }

            GetWeatherForecastDefinitionByIdQuery query = new(validatedId, temperatureUnit);
            WeatherForecastDefinitionResult? forecastResult = await bus.ExecuteAsync(query, cancellationToken);

            if (forecastResult is null)
            {
                return Results.NoContent();
            }

            return Results.Ok(new GetWeatherForecastDefinitionResponse { WeatherForecastDefinition = WeatherForecastMapper.MapToDefinitionDto(forecastResult) });
        }

        private static async Task<IResult> GetWeatherForecastByCoordinatesHandler(double latitude, double longitude, TemperatureUnit temperatureUnit, IBus bus, CancellationToken cancellationToken)
        {
            if (!Latitude.TryCreate(latitude, out Latitude? validatedLatitude))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["latitude"] = ["Latitude is invalid."] });
            }

            if (!Longitude.TryCreate(longitude, out Longitude? validatedLongitude))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["longitude"] = ["Longitude is invalid."] });
            }

            if (!Coordinates.TryCreate(validatedLatitude, validatedLongitude, out Coordinates? coordinates))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["coordinates"] = ["Coordinates are invalid."] });
            }

            GetWeatherForecastDefinitionByCoordinatesQuery query = new(coordinates, temperatureUnit);
            WeatherForecastDefinitionResult? forecastResult = await bus.ExecuteAsync(query, cancellationToken);

            if (forecastResult is null)
            {
                return Results.NoContent();
            }

            return Results.Ok(new GetWeatherForecastDefinitionResponse { WeatherForecastDefinition = WeatherForecastMapper.MapToDefinitionDto(forecastResult) });
        }

        private static async Task<IResult> GetAllWeatherForecastDefinitionsHandler(IBus bus, CancellationToken cancellationToken)
        {
            GetAllWeatherForecastDefinitionQuery query = new(TemperatureUnit.Celsius, 1, 100);
            List<WeatherForecastDefinitionResult> weatherForecastDefinitions = await bus.ExecuteAsync(query, cancellationToken);

            return Results.Ok(new GetAllWeatherForecastDefinitionResponse
            {
                WeatherForecastDefinitions = WeatherForecastMapper.MapToDefinitionsDto(weatherForecastDefinitions),
            });
        }

        private static async Task<IResult> DeleteWeatherForecastDefinitionByCoordinatesHandler(double latitude, double longitude, IBus bus, CancellationToken cancellationToken)
        {
            CoordinatesDto coordinatesDto = new() { Latitude = latitude, Longitude = longitude };
            (Coordinates? coordinates, IResult validationResult) = ValidateCoordinates(coordinatesDto);

            if (coordinates is null)
            {
                return validationResult;
            }

            DeleteWeatherForecastDefinitionByCoordinatesCommand command = new(coordinates);
            bool success = await bus.ExecuteAsync(command, cancellationToken);

            if (!success)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["WeatherForecastDefinition"] = [$"Weather forecast definition with coordinates {coordinates} does not exist."] });
            }

            return Results.Ok(new Dictionary<string, string[]> { ["id"] = [$"Weather forecast definition with coordinates {coordinates} has been deleted."] });
        }

        private static async Task<IResult> DeleteAllWeatherForecastDefinitionsHandler(IBus bus, CancellationToken cancellationToken)
        {
            DeleteAllWeatherForecastDefinitionsCommand command = new();
            bool success = await bus.ExecuteAsync(command, cancellationToken);

            if (!success)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["WeatherForecastDefinition"] = ["Weather Forecast Definitions are already empty"] });
            }

            return Results.Ok(new Dictionary<string, string[]> { ["WeatherForecastDefinition"] = ["All weather forecast definitions have been deleted."] });
        }

        private static async Task<IResult> FetchWeatherForecastsHandler(FetchWeatherForecastDefinitionRequest body, IBus bus, CancellationToken cancellationToken)
        {
            (Coordinates? coordinates, IResult validationResult) = ValidateCoordinates(body.Coordinates);

            if (coordinates is null)
            {
                return validationResult;
            }

            ForecastDaysAmount forecastDaysAmount = ForecastDaysAmount.Create(body.ForecastDaysAmount?.Value ?? 1);
            FetchWeatherForecastDefinitionCommand command = new(coordinates, forecastDaysAmount, body.Email);
            WeatherForecastDefinitionResult? forecastResult = await bus.ExecuteAsync(command, cancellationToken);

            if (forecastResult is null)
            {
                return Results.Problem("Problem occurred while fetching weather forecast definition.");
            }

            return Results.Ok(new FetchWeatherForecastDefinitionResponse
            {
                WeatherForecastDefinition = WeatherForecastMapper.MapToDefinitionDto(forecastResult),
            });
        }

        private static async Task<IResult> DeleteWeatherForecastDefinitionHandler(Guid id, IBus bus, CancellationToken cancellationToken)
        {
            if (!WeatherForecastDefinitionId.TryCreate(id, out WeatherForecastDefinitionId? validatedId))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["id"] = ["Weather forecast definition identifier is invalid."] });
            }

            DeleteWeatherForecastDefinitionCommand command = new(validatedId);
            bool success = await bus.ExecuteAsync(command, cancellationToken);

            if (!success)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["id"] = ["Weather forecast definition with identifier does not exist."] });
            }

            return Results.Ok();
        }

        /// <summary>
        /// Validates the coordinates input.
        /// </summary>
        private static (Coordinates?, IResult) ValidateCoordinates(CoordinatesDto? coordinatesDto)
        {
            if (coordinatesDto is null)
            {
                return (null, Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["coordinates"] = ["Coordinates must not be null."],
                    }));
            }

            if (!Latitude.TryCreate(coordinatesDto.Latitude, out Latitude? validatedLatitude))
            {
                return (null, Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["coordinates.latitude"] = ["Latitude is invalid."],
                    }));
            }

            if (!Longitude.TryCreate(coordinatesDto.Longitude, out Longitude? validatedLongitude))
            {
                return (null, Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["coordinates.longitude"] = ["Longitude is invalid."],
                    }));
            }

            if (!Coordinates.TryCreate(validatedLatitude, validatedLongitude, out Coordinates? coordinates))
            {
                return (null, Results.ValidationProblem(
                    new Dictionary<string, string[]>
                    {
                        ["coordinates"] = ["Coordinates are invalid."],
                    }));
            }

            return (coordinates, Results.Ok());
        }
    }
}
