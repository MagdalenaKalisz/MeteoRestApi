namespace Library.Buses
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Bus builder.
    /// </summary>
    public interface IBusBuilder
    {
        /// <summary>
        /// Whether this instance was created as a result of a subsequent call of the builder.
        /// </summary>
        /// <inheritdoc/>
        bool IsSubsequentCall { get; }

        /// <summary>
        /// Service collection.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
