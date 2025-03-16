namespace Meteo.Domain.ValueObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    using Library.Domain;

    /// <summary>
    /// Value Object representing temperature.
    /// </summary>
    public sealed class Temperature : ValueObject
    {
        /// <summary>
        /// Cached string representation of the value object.
        /// </summary>
        private string? _toStringCache;

        /// <summary>
        /// The value of the temperature.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// The unit of the temperature.
        /// </summary>
        public TemperatureUnit Unit { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Temperature"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        private Temperature(double value, TemperatureUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Get the equality components of the value object.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Unit;
        }

        /// <summary>
        /// Get the string representation of the value object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_toStringCache is null)
            {
                string unit = Unit switch
                {
                    TemperatureUnit.Celsius => "°C",
                    TemperatureUnit.Fahrenheit => "°F",
                    TemperatureUnit.Kelvin => "K",

                    _ => throw new InvalidOperationException("Unsupported temperature unit."),
                };

                _toStringCache = string.Create(CultureInfo.InvariantCulture, $"{Value} {unit}");
            }

            return _toStringCache;
        }

        /// <summary>
        /// Factory method to create a new Temperature instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Temperature Create(double value, TemperatureUnit unit)
        {
            if (!IsValidTemperature(value, unit))
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Temperature cannot be below absolute zero.");
            }

            return new Temperature(value, unit);
        }

        /// <summary>
        /// Factory method to create a new Temperature instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreate(double value, TemperatureUnit unit, [MaybeNullWhen(false)] out Temperature instance)
        {
            if (IsValidTemperature(value, unit))
            {
                instance = Create(value, unit);
                return true;
            }

            instance = null;
            return false;
        }

        /// <summary>
        /// Check if the temperature is valid.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static bool IsValidTemperature(double value, TemperatureUnit unit)
        {
            return unit switch
            {
                TemperatureUnit.Celsius => value >= -273.15,
                TemperatureUnit.Fahrenheit => value >= -459.67,
                TemperatureUnit.Kelvin => value >= 0,
                _ => false,
            };
        }

        /// <summary>
        /// Convert the temperature to a different unit.
        /// </summary>
        /// <param name="targetUnit"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Temperature ToUnit(TemperatureUnit targetUnit)
        {
            if (Unit == targetUnit)
            {
                return this;
            }

            return targetUnit switch
            {
                TemperatureUnit.Celsius => new Temperature(ToCelsius(), TemperatureUnit.Celsius),
                TemperatureUnit.Fahrenheit => new Temperature(ToFahrenheit(), TemperatureUnit.Fahrenheit),
                TemperatureUnit.Kelvin => new Temperature(ToKelvin(), TemperatureUnit.Kelvin),

                _ => throw new InvalidOperationException("Unsupported temperature unit."),
            };
        }

        /// <summary>
        /// Convert the temperature to Celsius.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private double ToCelsius()
        {
            return Unit switch
            {
                TemperatureUnit.Celsius => Value,
                TemperatureUnit.Fahrenheit => (Value - 32) * 5 / 9,
                TemperatureUnit.Kelvin => Value - 273.15,

                _ => throw new InvalidOperationException("Unsupported temperature unit."),
            };
        }

        /// <summary>
        /// Convert the temperature to Fahrenheit.
        /// </summary>
        /// <returns></returns>
        private double ToFahrenheit()
        {
            return (ToCelsius() * 9 / 5) + 32;
        }

        /// <summary>
        /// Convert the temperature to Kelvin.
        /// </summary>
        /// <returns></returns>
        private double ToKelvin()
        {
            return ToCelsius() + 273.15;
        }
    }
}
