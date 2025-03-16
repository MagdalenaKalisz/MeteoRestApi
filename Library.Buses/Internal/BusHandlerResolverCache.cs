namespace Library.Buses.Internal
{
    using System.Collections.Concurrent;
    using System.Collections.Frozen;
    using Library.Buses.Handlers;

    using Microsoft.Extensions.DependencyInjection;

    internal sealed class BusHandlerResolverCache
    {
        private readonly IServiceProviderIsService _serviceProviderIsService;
        private readonly ConcurrentDictionary<Type, FrozenSet<Type>> _cache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="BusHandlerResolverCache"/> class.`
        /// </summary>
        /// <param name="serviceProviderIsService"></param>
        public BusHandlerResolverCache(IServiceProviderIsService serviceProviderIsService)
        {
            _serviceProviderIsService = serviceProviderIsService;
        }

        /// <summary>
        /// Gets handler types for resolution.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public FrozenSet<Type> GetHandlerTypesForResolution(Type request)
        {
            return _cache.GetOrAdd(request, ResolveHandlerTypes);
        }

        private FrozenSet<Type> ResolveHandlerTypes(Type request)
        {
            List<Type> handlerTypes = new(BusConstants.KnownHandlerTypes.Count);

            foreach (Type handlerType in BusConstants.KnownHandlerTypes)
            {
                try
                {
                    if (!handlerType.IsGenericTypeDefinition)
                    {
                        Console.WriteLine($"Skipping {handlerType}: Not a generic type definition.");
                        continue;
                    }

                    Type? concreteHandlerType = null;

                    // two generic parameters
                    if (handlerType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                    {
                        Type? commandInterface = request.GetInterfaces()
                            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));

                        if (commandInterface != null)
                        {
                            Type resultType = commandInterface.GetGenericArguments()[0]; // Extract TResult
                            concreteHandlerType = handlerType.MakeGenericType(request, resultType);
                        }
                    }
                    if (handlerType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                    {
                        Type? commandInterface = request.GetInterfaces()
                            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

                        if (commandInterface != null)
                        {
                            Type resultType = commandInterface.GetGenericArguments()[0]; // Extract TResult
                            concreteHandlerType = handlerType.MakeGenericType(request, resultType);
                        }
                    }
                    // single generic parameter
                    else if (handlerType.GetGenericTypeDefinition() == typeof(IPersistedEventHandler<>))
                    {
                        concreteHandlerType = handlerType.MakeGenericType(request);
                    }
                    else if (handlerType.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    {
                        concreteHandlerType = handlerType.MakeGenericType(request);
                    }
                    else if (handlerType.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))
                    {
                        concreteHandlerType = handlerType.MakeGenericType(request);
                    }


                    if (concreteHandlerType == null)
                    {
                        Console.WriteLine($"Skipping {handlerType}: Could not determine correct generic types.");
                        continue;
                    }

                    if (!_serviceProviderIsService.IsService(concreteHandlerType))
                    {
                        continue;
                    }
                    

                    handlerTypes.Add(concreteHandlerType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to make generic type {handlerType} with {request}: {ex}");
                }
            }

            return handlerTypes.ToFrozenSet();
        }
    }
}
