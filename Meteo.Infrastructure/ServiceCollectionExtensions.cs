namespace Meteo.Infrastructure
{
    using System;
    using System.Reflection;
    using Library.Application.Events;

    using Library.Application.Interfaces;

    using Library.Buses;
    using Library.Buses.Handlers;

    using Meteo.Application.OpenMeteo;
    using Meteo.Infrastructure.Email;
    using Meteo.Infrastructure.EventHandlers;
    using Meteo.Infrastructure.OpenMeteo;
    using Meteo.Infrastructure.Services;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Contains extension methods to register infrastructure services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds infrastructure services to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
                                                           IConfiguration configuration,
                                                           IHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(environment);

            services
                .AddBuses()
                .RegisterFrom(Assembly.GetExecutingAssembly());

            services
                .AddHttpClient<IOpenMeteoClient, OpenMeteoClient>()
                .ConfigureHttpClient(static (serviceProvider, options) =>
                {
                    options.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
                    options.Timeout = TimeSpan.FromSeconds(30);
                });

            services.AddScoped<IEventHandler<EmailOutboxEvent>, EmailEventHandler>();

            services.
                Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
