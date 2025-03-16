namespace Library.Domain
{
    using System;

    /// <summary>
    /// Represents a value object.
    /// </summary>
    public interface IValueObject : IEquatable<IValueObject>, IComparable<IValueObject>
    {
    }
}
