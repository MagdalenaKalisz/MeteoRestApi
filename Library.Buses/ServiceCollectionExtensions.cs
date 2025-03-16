namespace Library.Buses
{
    using Library.Buses.Internal;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers buses.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddBuses(this IServiceCollection services, Action<IBusBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configure);

            IBusBuilder builder = services.AddBuses();
            configure(builder);

            return services;
        }

        /// <summary>
        /// Registers buses.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IBusBuilder AddBuses(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            BusBuilder busBuilder = new(services);

            if (!busBuilder.IsSubsequentCall)
            {
                services.AddScoped<IBus, Bus>();
                services.AddScoped<IBusHandlerResolver, BusHandlerResolver>();
                services.AddSingleton<BusHandlerResolverCache>();
            }

            return busBuilder;
        }
    }
}
