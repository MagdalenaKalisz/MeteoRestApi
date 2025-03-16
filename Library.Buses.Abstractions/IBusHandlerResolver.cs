namespace Library.Buses
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a bus handler resolver.
    /// </summary>
    public interface IBusHandlerResolver
    {
        /// <summary>
        /// Resolves handlers for a request of the specified type.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<object> ResolveHandlers(Type request);
    }
}