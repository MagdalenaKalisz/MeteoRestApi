namespace Library.Domain.Database
{
    /// <summary>
    /// Specifies the behavior of a query.
    /// </summary>
    [Flags]
    public enum QueryBehavior
    {
        /// <summary>
        /// Default behavior.
        /// </summary>
        Default = 0,

        /// <summary>
        /// The query will not track changes.
        /// </summary>
        NoTracking = 1 << 0,
    }
}
