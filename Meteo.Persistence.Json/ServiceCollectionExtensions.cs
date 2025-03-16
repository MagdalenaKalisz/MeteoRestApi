namespace Meteo.Persistence.Json
{
    using System.IO;
    using Library.Application.Interfaces;
    using Library.Domain.Database;
    using Library.Persistence;
    using Meteo.Domain.Database;
    using Meteo.Persistence.Json.Collections;
    using Meteo.Persistence.Json.Configuration;
    using Meteo.Persistence.Json.Repositories;
    using Meteo.Persistence.Json.Services;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Contains extension methods to register JSON persistence services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds JSON-based persistence services to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IServiceCollection AddJsonPersistence(this IServiceCollection services,
                                                            IConfiguration configuration,
                                                            IHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);

            services.Configure<JsonDatabaseSettings>(configuration.GetSection("JsonDatabase").Bind);

            services.AddSingleton<IWeatherForecastCollection>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<JsonDatabaseSettings>>().Value;
                string filePath = Path.Combine(settings.BasePath, settings.WeatherForecastFileName);
                return new WeatherForecastCollection(filePath);
            });

            services.AddSingleton<IWeatherForecastDefinitionCollection>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<JsonDatabaseSettings>>().Value;
                string filePath = Path.Combine(settings.BasePath, settings.WeatherForecastDefinitionFileName);
                return new WeatherForecastDefinitionCollection(filePath);
            });

            services.AddSingleton<IOutboxRepository>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<JsonDatabaseSettings>>().Value;
                string filePath = Path.Combine(settings.BasePath, settings.OutboxFileName);
                return ActivatorUtilities.CreateInstance<OutboxRepository>(serviceProvider, filePath);
            });

            // Register unit of work
            services.AddSingleton<IWeatherDatabaseSession>(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<JsonDatabaseSettings>>().Value;
                return new WeatherDatabaseSession(settings.BasePath, provider.GetRequiredService<IOutboxRepository>());
            });

            services.AddSingleton<IDatabaseSession>(provider =>
                provider.GetRequiredService<IWeatherDatabaseSession>());

            // Register Outbox Service
            services.AddScoped<IOutbox, JsonOutbox>();

            return services;
        }
    }
}
