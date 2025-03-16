namespace Library.Identifiers
{
    /// <summary>
    /// Base class for all identifier providers responsible for identifier generation.
    /// </summary>
    public abstract class IdentifierProvider
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentifierProvider"/>.
        /// </summary>
        protected IdentifierProvider()
        {

        }

        /// <summary>
        /// Gets the next random <see cref="Guid"/> identifier.
        /// </summary>
        /// <returns></returns>
        public abstract Guid CreateUuid();

        /// <summary>
        /// Gets the next sequential <see cref="Guid"/> identifier.
        /// </summary>
        /// <returns></returns>
        public abstract Guid CreateSequentialUuid();
    }
}
