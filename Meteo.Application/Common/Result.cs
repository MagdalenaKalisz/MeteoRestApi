namespace Meteo.Application.Common
{
    /// <summary>
    /// Represents the result of an operation, which can either be a success or a failure.
    /// </summary>
    public sealed class Result<TSuccess, TError>
    {
        /// <summary>
        /// The value of a successful result.
        /// </summary>
        public TSuccess? Value { get; }

        /// <summary>
        /// The error of a failed result.
        /// </summary>
        public TError? Error { get; }

        /// <summary>
        /// Indicates whether the result is successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates whether the result is a failure.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Initializes a new successful result.
        /// </summary>
        /// <param name="value"></param>
        private Result(TSuccess value)
        {
            Value = value;
            IsSuccess = true;
        }

        /// <summary>
        /// Initializes a new failed result.
        /// </summary>
        /// <param name="error"></param>
        private Result(TError error)
        {
            Error = error;
            IsSuccess = false;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result<TSuccess, TError> CreateSuccess(TSuccess value)
        {
            return new Result<TSuccess, TError>(value);
        }

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TSuccess, TError> CreateFailure(TError error)
        {
            return new Result<TSuccess, TError>(error);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
        }

        /// <summary>
        /// Converts the <see cref="Result{TSuccess, TError}"/> to a <typeparamref name="TSuccess"/> if successful.
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator TSuccess?(Result<TSuccess, TError> result)
        {
            return result.IsSuccess ? result.Value : default;
        }

        /// <summary>
        /// Converts the <see cref="Result{TSuccess, TError}"/> to a <typeparamref name="TError"/> if failed.
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator TError?(Result<TSuccess, TError> result)
        {
            return result.IsFailure ? result.Error : default;
        }
    }
}
