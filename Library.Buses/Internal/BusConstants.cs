namespace Library.Buses.Internal
{
    using System.Collections.Frozen;
    using Library.Buses.Handlers;

    /// <summary>
    /// Common bus constants.
    /// </summary>
    internal static class BusConstants
    {
        /// <summary>
        /// All known handler types.
        /// </summary>
        public static readonly FrozenSet<Type> KnownHandlerTypes =
            new Type[]
            {
                typeof(ICommandHandler<,>),
                typeof(IQueryHandler<,>),
                typeof(IDomainEventHandler<>),
                typeof(IEventHandler<>),
                typeof(IPersistedEventHandler<>),
            }
            .ToFrozenSet();
    }
}
