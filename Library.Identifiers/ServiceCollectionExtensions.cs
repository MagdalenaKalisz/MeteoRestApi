namespace Library.Identifiers
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="DefaultIdentifierProvider"/> as an <see cref="IdentifierProvider"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultIdentifierProvider(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddIdentifierProvider<DefaultIdentifierProvider>();
        }

        /// <summary>
        /// Registers an <see cref="IdentifierProvider"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentifierProvider<T>(this IServiceCollection services)
            where T : IdentifierProvider
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Replace(ServiceDescriptor.Singleton<IdentifierProvider, T>());

            return services;
        }
    }
}
