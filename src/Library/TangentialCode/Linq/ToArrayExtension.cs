namespace OpenTracing.Contrib.Extensions.TangentialCode.Linq
{
    using System;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    ///     Optimized <see cref="Enumerable.ToArray{TSource}" />
    /// </summary>
    internal static class ToArrayExtension
    {
        [NotNull]
        public static TResult[] ToArray<TSource, TResult>(
            [NotNull] this TSource[] source,
            [NotNull] Func<TSource, TResult> projection)
        {
            var result = new TResult[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = projection(source[i]);
            }

            return result;
        }
    }
}