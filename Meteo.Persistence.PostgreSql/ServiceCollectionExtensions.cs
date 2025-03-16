namespace Meteo.Persistence.PostgreSql
{
    using Domain.Database;
    using Library.Application.Interfaces;
    using Library.Domain.Database;
    using Library.Persistence;
    using Meteo.Persistence.PostgreSql.Collections;
    using Meteo.Persistence.PostgreSql.Repositories;
    using Meteo.Persistence.PostgreSql.Services;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Contains extension methods to register persistence services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds persistence services to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IServiceCollection AddPostgreSqlPersistence(this IServiceCollection services,
                                                                  IConfiguration configuration,
                                                                  IHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IWeatherForecastCollection, WeatherForecastCollection>();
            services.AddScoped<IWeatherForecastDefinitionCollection, WeatherForecastDefinitionCollection>();
            services.AddScoped<IOutboxRepository, OutboxRepository>();

            // Register unit of work
            services.AddScoped<IWeatherDatabaseSession, PostgreSqlWeatherDatabaseSession>();
            services.AddScoped<IDatabaseSession>(static provider => provider.GetRequiredService<IWeatherDatabaseSession>());

            // Register services
            services.AddScoped<IOutbox, PostgreSqlOutbox>();

            return services;
        }
    }
}
