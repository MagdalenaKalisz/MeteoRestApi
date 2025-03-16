namespace Library.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    using Domain;

    /// <summary>
    /// Represents a creation timestamp value object.
    /// </summary>
    public sealed class CreationTimestamp : ValueObject
    {
        /// <summary>
        /// The value of the timestamp.
        /// </summary>
        public DateTimeOffset Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationTimestamp"/> class.
        /// </summary>
        /// <param name="value"></param>
        [JsonConstructor]
        private CreationTimestamp(DateTimeOffset value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a new timestamp.
        /// </summary>
        public static CreationTimestamp Create(DateTimeOffset value)
        {
            return new CreationTimestamp(value);
        }

        /// <summary>
        /// Automatically generates a creation timestamp for now.
        /// </summary>
        public static CreationTimestamp CreateNow()
        {
            return new CreationTimestamp(DateTimeOffset.UtcNow);
        }

        /// <inheritdoc/>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value.ToString("O"); // ISO 8601 format
        }

        /// <summary>
        /// Converts the <see cref="CreationTimestamp"/> to a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="instance"></param>
        public static implicit operator DateTimeOffset(CreationTimestamp instance)
        {
            return instance.Value;
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> to a <see cref="CreationTimestamp"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator CreationTimestamp(DateTimeOffset value)
        {
            return Create(value);
        }
    }
}
