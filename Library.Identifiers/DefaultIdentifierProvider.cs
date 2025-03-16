namespace Library.Identifiers
{
    /// <summary>
    /// Default <see cref="IdentifierProvider"/>.
    /// </summary>
    public sealed class DefaultIdentifierProvider : IdentifierProvider
    {
        /// <summary>
        /// Time provider instance.
        /// </summary>
        private readonly TimeProvider _time;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultIdentifierProvider"/>.
        /// </summary>
        /// <param name="timeProvider"></param>
        public DefaultIdentifierProvider(TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(timeProvider);

            _time = timeProvider;
        }

        /// <inheritdoc/>
        public override Guid CreateUuid()
        {
            return Guid.NewGuid();
        }

        /// <inheritdoc/>
        public override Guid CreateSequentialUuid()
        {
            return Guid.CreateVersion7(_time.GetUtcNow());
        }
    }
}
