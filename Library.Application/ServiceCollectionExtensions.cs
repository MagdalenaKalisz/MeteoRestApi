namespace Library.Application;

using Library.Application.Interfaces;
using Library.Application.Internal;
using Library.Application.Services;
using Library.Buses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// Contains extension methods to register application services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (services.Any(static x => x.ServiceType == typeof(ApplicationCoreMarkerService)))
        {
            return services;
        }

        services.AddSingleton<ApplicationCoreMarkerService>();

        services.AddBuses();

        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddHostedService<OutboxBackgroundProcessor>();

        return services;
    }
}