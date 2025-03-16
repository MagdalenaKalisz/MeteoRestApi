namespace Meteo.Application.Handlers.Commands
{
    using System.Text;

    using Library.Application;
    using Library.Application.Events;

    using Library.Application.Interfaces;
    using Library.Domain.Database;
    using Library.Identifiers;
    using Meteo.Application.Models;
    using Meteo.Application.OpenMeteo;
    using Meteo.Application.OpenMeteo.Dto;
    using Meteo.Domain.Aggregates;
    using Meteo.Domain.Database;
    using Meteo.Domain.Enums;
    using Meteo.Domain.ValueObjects;

    /// <summary>
    /// Fetches or creates (if missing) a WeatherForecast in the domain layer.
    /// Returns the domain WeatherForecast or null if something prevents creation.
    /// </summary>
    public sealed record FetchWeatherForecastDefinitionCommand(
        Coordinates Coordinates,
        ForecastDaysAmount ForecastDays,
        string? Email) : ICommand<WeatherForecastDefinitionResult?>
    {
        private sealed class Handler(
            IWeatherDatabaseSession _db,
            IOutbox _outbox,
            IOpenMeteoClient _openMeteoClient,
            TimeProvider _time,
            IdentifierProvider _identifierProvider,
            IDomainEventDispatcher _domainEventDispatcher) : ICommandHandler<FetchWeatherForecastDefinitionCommand, WeatherForecastDefinitionResult?>
        {
            public async Task<WeatherForecastDefinitionResult?> HandleAsync(FetchWeatherForecastDefinitionCommand command, CancellationToken cancellationToken)
            {
                OpenMeteoWeatherResponse? openMeteoWeatherResponse = await FetchWeatherDataAsync(command, cancellationToken);
                if (openMeteoWeatherResponse is null)
                {
                    return null;
                }

                WeatherForecastDefinition forecastDefinition = await GetOrCreateForecastDefinitionAsync(command, cancellationToken);

                List<WeatherForecast> forecastsPerDay = await ProcessWeatherDataAsync(forecastDefinition, openMeteoWeatherResponse, cancellationToken);

                await _db.DispatchAndSaveChangesAsync(_domainEventDispatcher, cancellationToken, [forecastDefinition, .. forecastsPerDay]);

                if (command.Email?.Length > 0)
                {
                    string forecastSummary = GenerateForecastSummary(forecastsPerDay);

                    var emailEvent = new EmailOutboxEvent(
                        command.Email,
                        "New Weather Forecast Created",
                        $"A new weather forecast has been created for coordinates: {command.Coordinates.Latitude}, {command.Coordinates.Longitude}.\n\n"
                        + "Here is your forecast:\n\n"
                        + forecastSummary
                    );

                    await _outbox.SaveEventAsync(emailEvent, cancellationToken);
                }

                return new WeatherForecastDefinitionResult(forecastDefinition, forecastsPerDay);
            }

            /// <summary>
            /// Fetches weather data from OpenMeteo.
            /// </summary>
            private async Task<OpenMeteoWeatherResponse?> FetchWeatherDataAsync(
                FetchWeatherForecastDefinitionCommand command,
                CancellationToken cancellationToken)
            {
                return await _openMeteoClient.GetWeatherAsync(
                    command.Coordinates,
                    command.ForecastDays,
                    cancellationToken
                );
            }

            /// <summary>
            /// Retrieves an existing forecast definition or creates a new one.
            /// </summary>
            private async Task<WeatherForecastDefinition> GetOrCreateForecastDefinitionAsync(
                FetchWeatherForecastDefinitionCommand command,
                CancellationToken cancellationToken)
            {
                WeatherForecastDefinition? forecastDefinition = await _db.WeatherForecastDefinitions.GetByCoordinatesAsync(
                    command.Coordinates,
                    QueryBehavior.NoTracking,
                    cancellationToken
                );

                if (forecastDefinition is null)
                {
                    forecastDefinition = WeatherForecastDefinition.Create(
                        _identifierProvider.CreateSequentialUuid(),
                        _time.GetUtcNow(),
                        command.Coordinates
                    );

                    await _db.WeatherForecastDefinitions.AddAsync(forecastDefinition, cancellationToken);

                    await _db.DispatchAndSaveChangesAsync(_domainEventDispatcher, cancellationToken, forecastDefinition);

                    forecastDefinition.ClearDomainEvents();
                }

                return forecastDefinition;
            }

            /// <summary>
            /// Processes weather data and creates forecasts.
            /// </summary>
            private async Task<List<WeatherForecast>> ProcessWeatherDataAsync(
                WeatherForecastDefinition forecastDefinition,
                OpenMeteoWeatherResponse openMeteoWeatherResponse,
                CancellationToken cancellationToken
            )
            {
                var groupedByDay = openMeteoWeatherResponse.Hourly.Time
                    .Zip(openMeteoWeatherResponse.Hourly.Temperature2m, (timeStr, tempVal) => new { timeStr, tempVal })
                    .Zip(openMeteoWeatherResponse.Hourly.RelativeHumidity2m, (entry, humVal) =>
                        new { entry.timeStr, entry.tempVal, humVal })
                    .GroupBy(item => DateTimeOffset.Parse(item.timeStr).Date)
                    .ToList();

                List<WeatherForecast> forecastsPerDay = [];

                foreach (var group in groupedByDay)
                {
                    WeatherForecast? existingForecast = await _db.WeatherForecasts.GetByDefinitionIdAsync(
                        forecastDefinition.Id, QueryBehavior.Default, cancellationToken
                    );

                    WeatherForecast forecast = await UpdateOrCreateForecastsAsync(existingForecast, forecastDefinition.Id, group, cancellationToken);
                    forecastsPerDay.Add(forecast);
                }

                return forecastsPerDay;
            }

            /// <summary>
            /// Updates existing forecasts or creates new ones.
            /// </summary>
            private async Task<WeatherForecast> UpdateOrCreateForecastsAsync(
                WeatherForecast? existingForecast,
                Guid forecastDefinitionId,
                IGrouping<DateTime, dynamic> group,
                CancellationToken cancellationToken
            )
            {
                DateTime forecastDate = group.Key;

                if (existingForecast?.Forecasts.Any(day => day.ForecastDateTime.DateTime.Date == forecastDate) == false)
                {
                    existingForecast = null;
                }

                if (existingForecast is null)
                {
                    existingForecast = WeatherForecast.Create(
                        _identifierProvider.CreateSequentialUuid(),
                        _time.GetUtcNow(),
                        forecastDefinitionId,
                        []
                    );

                    await _db.WeatherForecasts.AddAsync(existingForecast, cancellationToken);
                }

                List<WeatherForecastForDay> updatedForecasts = [.. existingForecast.Forecasts];

                foreach (dynamic hourlyEntry in group)
                {
                    DateTimeOffset forecastDateTime = DateTimeOffset.Parse(hourlyEntry.timeStr);

                    bool exists = updatedForecasts.Exists(f => f.ForecastDateTime.DateTime == forecastDateTime);
                    if (!exists)
                    {
                        updatedForecasts.Add(WeatherForecastForDay.Create(
                            ForecastDateTime.Create(forecastDateTime),
                            Temperature.Create(hourlyEntry.tempVal, TemperatureUnit.Celsius),
                            Humidity.Create(hourlyEntry.humVal)
                        ));
                    }
                }

                existingForecast = WeatherForecast.Create(
                    existingForecast.Id,
                    existingForecast.CreatedAt,
                    existingForecast.DefinitionId,
                    updatedForecasts
                );
                await _db.WeatherForecasts.UpdateAsync(existingForecast, cancellationToken);
                existingForecast.ClearDomainEvents();
                return existingForecast;
            }

            private static string GenerateForecastSummary(List<WeatherForecast> forecastsPerDay)
            {
                var summaryBuilder = new StringBuilder();

                foreach (var forecast in forecastsPerDay)
                {
                    summaryBuilder.AppendLine($"📅 Date: {forecast.Forecasts[0].ForecastDateTime.DateTime:yyyy-MM-dd}");

                    foreach (var dailyForecast in forecast.Forecasts.OrderBy(f => f.ForecastDateTime.DateTime))
                    {
                        summaryBuilder.AppendLine($"🕒 {dailyForecast.ForecastDateTime.DateTime:HH:mm} - " +
                                                  $"🌡 {dailyForecast.Temperature.Value}°C, " +
                                                  $"💧 {dailyForecast.Humidity.Percentage}% humidity");
                    }

                    summaryBuilder.AppendLine();
                }

                return summaryBuilder.ToString();
            }

        }
    }
}