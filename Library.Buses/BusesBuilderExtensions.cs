namespace Library.Buses
{
    using System.Reflection;
    using Library.Buses.Handlers;
    using Library.Buses.Internal;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// <see cref="IBusBuilder"/> extensions.
    /// </summary>
    public static class BusesBuilderExtensions
    {
        /// <summary>
        /// Registers handlers from specified type.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBusBuilder Register<T>(this IBusBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            return builder.Register(typeof(T));
        }

        /// <summary>
        /// Registers buses.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IBusBuilder RegisterFrom(this IBusBuilder builder, params IEnumerable<Assembly> assemblies)
        {
            ArgumentNullException.ThrowIfNull(builder);

            foreach (Assembly assembly in assemblies)
            {
                Type[] typesToRegister = assembly
                    .GetTypes()
                    .Where(IsValidImplementationType)
                    .ToArray();

                foreach (Type type in typesToRegister)
                {
                    builder.Register(type);
                }
            }

            return builder;
        }

        /// <summary>
        /// Registers buses.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IBusBuilder Register(this IBusBuilder builder, Type type)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(type);

            if (!IsValidImplementationType(type))
            {
                throw new ArgumentException($"Type '{type}' is not a valid handler implementation type.", nameof(type));
            }

            IServiceCollection services = builder.Services;

            Type[] concreteHandlers = [.. type
                .GetInterfaces()
                .Where(static x =>
                {
                    return x.IsGenericType && BusConstants.KnownHandlerTypes.Contains(x.GetGenericTypeDefinition());
                }),];

            foreach (Type handlerInterface in concreteHandlers)
            {
                services.AddTransient(handlerInterface, type);
            }

            return builder;
        }

        private static bool IsValidImplementationType(Type implementationType)
        {
            if (!implementationType.IsClass || implementationType.IsAbstract)
            {
                return false;
            }

            bool implementsIHandler = implementationType
                .GetInterfaces()
                .Any(static x => x == typeof(IHandlerWithoutResult) || x == typeof(IHandlerWithResult));

            if (!implementsIHandler)
            {
                return false;
            }

            bool isKnownHandlerType = implementationType
                .GetInterfaces()
                .Any(static x => x.IsGenericType && BusConstants.KnownHandlerTypes.Contains(x.GetGenericTypeDefinition()));

            return implementsIHandler;
        }
    }
}
