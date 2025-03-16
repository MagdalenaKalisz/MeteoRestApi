namespace Library.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// <see cref="object"/> extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Throws an exception when <paramref name="argument"/> is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument"></param>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="argument"/> is null.</exception>
        [return: NotNull]
        public static T AsNotNull<T>(this T? argument,
                                     [CallerArgumentExpression(nameof(argument))] string? argumentName = null)
            where T : class
        {
            return argument ??
                throw new ArgumentNullException(argumentName ?? nameof(argument), "Not null guard violation.");
        }

        /// <summary>
        /// Throws an exception when <paramref name="argument"/> is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument"></param>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="argument"/> is null.</exception>
        [return: NotNull]
        public static T AsNotNull<T>(this T? argument,
                                     [CallerArgumentExpression(nameof(argument))] string? argumentName = null)
            where T : struct
        {
            return argument ??
                throw new ArgumentNullException(argumentName ?? nameof(argument), "Not null guard violation.");
        }
    }
}
