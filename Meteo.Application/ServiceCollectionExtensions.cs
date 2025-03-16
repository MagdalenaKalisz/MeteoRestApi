namespace Meteo.Application;

using System.Reflection;
using Library.Application;
using Library.Buses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Contains extension methods to register application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(this IServiceCollection services,
                                                    IConfiguration configuration,
                                                    IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        services.AddApplicationCore();

        services
            .AddBuses()
            .RegisterFrom(Assembly.GetExecutingAssembly());

        return services;
    }
}