namespace Library.Buses.Internal
{
    using System.Collections.Frozen;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Default implementation of <see cref="IBusHandlerResolver"/>.
    /// </summary>
    internal sealed class BusHandlerResolver : IBusHandlerResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly BusHandlerResolverCache _handlerResolverCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusHandlerResolverCache"/> class.`
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="handlerResolverCache"></param>
        public BusHandlerResolver(IServiceProvider serviceProvider,
                                  BusHandlerResolverCache handlerResolverCache)
        {
            _serviceProvider = serviceProvider;
            _handlerResolverCache = handlerResolverCache;
        }

        /// <summary>
        /// Gets handler types for resolution.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<object> ResolveHandlers(Type request)
        {
            FrozenSet<Type> handlerTypes = _handlerResolverCache.GetHandlerTypesForResolution(request);

            foreach (Type handlerType in handlerTypes)
            {
                yield return _serviceProvider.GetRequiredService(handlerType);
            }
        }
    }
}
