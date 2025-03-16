namespace Library.Buses.Internal
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Command Bus builder.
    /// </summary>
    internal sealed class BusBuilder : IBusBuilder
    {
        /// <inheritdoc/>
        public bool IsSubsequentCall { get; }

        /// <inheritdoc/>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="BusBuilder"/>.
        /// </summary>
        /// <param name="services"></param>
        public BusBuilder(IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            Services = services;
            IsSubsequentCall = services.Any(static x => x.ServiceType == typeof(BusMarkerService));

            if (!IsSubsequentCall)
            {
                services.AddSingleton<BusMarkerService>();
            }
        }
    }
}