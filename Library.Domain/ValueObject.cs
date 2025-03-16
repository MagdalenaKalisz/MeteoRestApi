namespace Library.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base class for all value objects.
    /// </summary>
    public abstract class ValueObject : IValueObject, IEquatable<ValueObject>, IComparable<ValueObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueObject"/> class.
        /// </summary>
        protected ValueObject()
        {
        }

        /// <summary>
        /// Get the equality components of the value object.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <inheritdoc />
        public bool Equals(ValueObject? other)
        {
            return Equals(other as IValueObject);
        }

        /// <inheritdoc />
        public bool Equals(IValueObject? other)
        {
            return other is ValueObject valueObject && EqualsValueObject(valueObject);
        }

        private bool EqualsValueObject(ValueObject other)
        {
            using IEnumerator<object> thisComponents = GetEqualityComponents().GetEnumerator();
            using IEnumerator<object> otherComponents = other.GetEqualityComponents().GetEnumerator();

            while (thisComponents.MoveNext() && otherComponents.MoveNext())
            {
                if (thisComponents.Current is null ^ otherComponents.Current is null)
                {
                    return false;
                }

                if (thisComponents.Current != null && !thisComponents.Current.Equals(otherComponents.Current))
                {
                    return false;
                }
            }

            return !thisComponents.MoveNext() && !otherComponents.MoveNext();
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as ValueObject);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(0, (hash, component) => hash ^ (component?.GetHashCode() ?? 0));
        }

        /// <inheritdoc />
        public int CompareTo(ValueObject? other)
        {
            if (other is null)
            {
                return 1; // This instance is greater than a null value.
            }

            using IEnumerator<object> thisComponents = GetEqualityComponents().GetEnumerator();
            using IEnumerator<object> otherComponents = other.GetEqualityComponents().GetEnumerator();

            while (thisComponents.MoveNext() && otherComponents.MoveNext())
            {
                int comparison = CompareComponents(thisComponents.Current, otherComponents.Current);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return !thisComponents.MoveNext() && otherComponents.MoveNext() ? -1
                 : thisComponents.MoveNext() && !otherComponents.MoveNext() ? 1
                 : 0;
        }

        /// <inheritdoc />
        int IComparable<IValueObject>.CompareTo(IValueObject? other)
        {
            return CompareTo(other as ValueObject);
        }

        private static int CompareComponents(object? left, object? right)
        {
            if (left is null && right is null)
            {
                return 0;
            }

            if (left is null)
            {
                return -1;
            }

            if (right is null)
            {
                return 1;
            }

            if (left is IComparable comparableLeft && right is IComparable comparableRight)
            {
                return comparableLeft.CompareTo(comparableRight);
            }

            throw new InvalidOperationException($"Cannot compare components of type {left.GetType()} and {right.GetType()}.");
        }

        /// <summary>
        /// Check if the value objects are equal.
        /// </summary>
        public static bool operator ==(ValueObject left, ValueObject right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Check if the value objects are not equal.
        /// </summary>
        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Compare two value objects.
        /// </summary>
        public static bool operator <(ValueObject left, ValueObject right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compare two value objects.
        /// </summary>
        public static bool operator >(ValueObject left, ValueObject right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Compare two value objects.
        /// </summary>
        public static bool operator <=(ValueObject left, ValueObject right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Compare two value objects.
        /// </summary>
        public static bool operator >=(ValueObject left, ValueObject right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
