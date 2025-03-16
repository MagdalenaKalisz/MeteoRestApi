namespace Meteo.Api.Endpoints
{
    using Domain.Enums;
    using Domain.ValueObjects;
    using Library.Buses;
    using Meteo.Api.Contracts;
    using Meteo.Api.Contracts.WeatherForecastDefinitions;
    using Meteo.Api.Mappers;

    using Meteo.Application.Common;

    using Meteo.Application.Handlers.Commands;
    using Meteo.Application.Handlers.Queries;
    using Meteo.Application.Models;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Weather definitions API extensions.
    /// </summary>
    public static class WeatherDefinitionsApiExtensions
    {
        /// <summary>
        /// Adds weather definitions API endpoints.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEndpointRouteBuilder AddWeatherDefinitionsApi(this IEndpointRouteBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.MapPost(
                "/definition",
                static async (CreateWeatherForecastDefinitionRequest body,
                                     IBus bus,
                                     CancellationToken cancellationToken) =>
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
                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Registers a weather forecast definition. " +
                                "If email address is provided then it will send message with WeatherForecastDefinition details. Added for Outbox purpose");

            builder.MapPost(
                "/forecasts",
                static async (FetchWeatherForecastDefinitionRequest body,
                                     IBus bus,
                                     CancellationToken cancellationToken) =>
                {
                    (Coordinates? coordinates, IResult validationResult) = ValidateCoordinates(body.Coordinates);

                    if (coordinates is null)
                    {
                        return validationResult;
                    }

                    ForecastDaysAmount forecastDaysAmount = ForecastDaysAmount.Create(body.ForecastDaysAmount?.Value ?? 1);

                    FetchWeatherForecastDefinitionCommand command = new(coordinates, forecastDaysAmount, body.Email);

                    WeatherForecastDefinitionResult? weatherForecastDefinitionResult = await bus.ExecuteAsync(command, cancellationToken);

                    if (weatherForecastDefinitionResult is null)
                    {
                        return Results.Problem("Problem occurred while fetching weather forecast definition.");
                    }

                    return Results.Ok(new FetchWeatherForecastDefinitionResponse
                    {
                        WeatherForecastDefinition = WeatherForecastMapper.MapToDefinitionDto(weatherForecastDefinitionResult),
                    });
                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Fetches data from open meteo and adds or updates weather database. " +
                                "If email address is provided then it will send message with WeatherForecastDefinition details. Added for Outbox purpose");

            builder.MapDelete(
                "/by-id/{id:guid}",
                static async ([FromRoute(Name = "id")] Guid id,
                                     IBus bus,
                                     CancellationToken cancellationToken) =>
                {
                    if (!WeatherForecastDefinitionId.TryCreate(id, out WeatherForecastDefinitionId? validatedId))
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                ["id"] = ["Weather forecast definition identifier is invalid."],
                            });
                    }

                    DeleteWeatherForecastDefinitionCommand command = new(validatedId);

                    bool success = await bus.ExecuteAsync(command, cancellationToken);

                    if (!success)
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                ["id"] = ["Weather forecast definition with identifier " + validatedId + " does not exist."],
                            });
                    }

                    return Results.Ok(
                        new Dictionary<string, string[]>
                        {
                            ["id"] = [validatedId + " has been deleted."],
                        });
                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Deletes a weather forecast definition by its identifier.");

            builder.MapDelete(
                "/by-coordinates/{latitude:double},{longitude:double}",
                static async ([FromRoute(Name = "latitude")] double latitude,
                                     [FromRoute(Name = "longitude")] double longitude,
                                     IBus bus,
                                     CancellationToken cancellationToken) =>
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
                        return Results.ValidationProblem(
                          new Dictionary<string, string[]>
                          {
                              ["WeatherForecastDefinition"] = ["Weather forecast definition with coordinates " + coordinates + " does not exist."],
                          });
                    }

                    return Results.Ok(
                        new Dictionary<string, string[]>
                        {
                            ["id"] = [$"Weather forecast definition with coordinates {coordinates} has been deleted."],
                        });
                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Registers a weather forecast definition by coordinates.");

            builder.MapDelete(
                "/",
                static async (IBus bus,
                        CancellationToken cancellationToken) =>
                {
                    DeleteAllWeatherForecastDefinitionsCommand command = new();

                    bool success = await bus.ExecuteAsync(command, cancellationToken);

                    if (!success)
                    {
                        return Results.ValidationProblem(
                        new Dictionary<string, string[]>
                        {
                            ["WeatherForecastDefinition"] = ["Weather Forecast Definitions are already empty"],
                        });
                    }

                    return Results.Ok(
                        new Dictionary<string, string[]>
                        {
                            ["WeatherForecastDefinition"] = ["All weather forecast definitions have been deleted."],
                        });
                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Deletes all weather forecast definitions and related weather forecasts.");

            builder.MapGet(
                "/",
                static async ([FromQuery(Name = "temperatureUnit")] TemperatureUnit? temperatureUnit,
                               [FromQuery(Name = "page")] int? page,
                               [FromQuery(Name = "perPage")] int? perPage,
                               IBus bus,
                               CancellationToken cancellationToken) =>
                {
                    TemperatureUnit selectedUnit = temperatureUnit ?? TemperatureUnit.Celsius;
                    int selectedPage = page ?? 1;
                    int selectedPerPage = perPage ?? 100;

                    GetAllWeatherForecastDefinitionQuery query = new(selectedUnit, selectedPage, selectedPerPage);

                    List<WeatherForecastDefinitionResult> weatherForecastDefinitions = await bus.ExecuteAsync(query, cancellationToken);

                    return Results.Ok(new GetAllWeatherForecastDefinitionResponse
                    {
                        WeatherForecastDefinitions = WeatherForecastMapper.MapToDefinitionsDto(weatherForecastDefinitions),
                    });
                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Gets all weather forecast definitions.");

            builder.MapGet(
                "/by-id/{id:guid}/forecast",
                static async ([FromRoute(Name = "id")] Guid id,
                                     [FromQuery(Name = "temperatureUnit")] TemperatureUnit? temperatureUnit,
                                     IBus bus,
                                     CancellationToken cancellationToken) =>
                {
                    TemperatureUnit selectedUnit = temperatureUnit ?? TemperatureUnit.Celsius;

                    if (!WeatherForecastDefinitionId.TryCreate(id, out WeatherForecastDefinitionId? validatedId))
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                ["id"] = ["Weather forecast definition identifier is invalid."],
                            });
                    }

                    GetWeatherForecastDefinitionByIdQuery query = new(validatedId, selectedUnit);

                    WeatherForecastDefinitionResult? weatherForecastDefinition = await bus.ExecuteAsync(query, cancellationToken);

                    if (weatherForecastDefinition is null)
                    {
                        return Results.NoContent();
                    }

                    return Results.Ok(new GetWeatherForecastDefinitionResponse
                                        {
                        WeatherForecastDefinition = WeatherForecastMapper.MapToDefinitionDto(weatherForecastDefinition),
                    });

                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Gets a weather forecast for a given definition identifier.");

            builder.MapGet(
                "/by-coordinates/{latitude:double},{longitude:double}/forecast",
                static async ([FromRoute(Name = "latitude")] double latitude,
                              [FromRoute(Name = "longitude")] double longitude,
                              [FromQuery(Name = "temperatureUnit")] TemperatureUnit? temperatureUnit,
                              IBus bus,
                              CancellationToken cancellationToken) =>
                {
                    TemperatureUnit selectedUnit = temperatureUnit ?? TemperatureUnit.Celsius;

                    if (!Latitude.TryCreate(latitude, out Latitude? validatedLatitude))
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                ["latitude"] = ["Latitude is invalid."],
                            });
                    }

                    if (!Longitude.TryCreate(longitude, out Longitude? validatedLongitude))
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                ["longitude"] = ["Longitude is invalid."],
                            });
                    }

                    if (!Coordinates.TryCreate(validatedLatitude, validatedLongitude, out Coordinates? coordinates))
                    {
                        return Results.ValidationProblem(
                            new Dictionary<string, string[]>
                            {
                                ["coordinates"] = ["Coordinates are invalid."],
                            });
                    }

                    GetWeatherForecastDefinitionByCoordinatesQuery query = new(coordinates, selectedUnit);

                    WeatherForecastDefinitionResult? weatherForecastDefinition = await bus.ExecuteAsync(query, cancellationToken);

                    if (weatherForecastDefinition is null)
                    {
                        return Results.NoContent();
                    }

                    return Results.Ok(new GetWeatherForecastDefinitionResponse 
                                            {
                            WeatherForecastDefinition = WeatherForecastMapper.MapToDefinitionDto(weatherForecastDefinition),
                        });
                })
                .WithOpenApi()
                .WithTags("Weather")
                .WithDescription("Get a weather forecast for given coordinates.");

            return builder;
        }

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
