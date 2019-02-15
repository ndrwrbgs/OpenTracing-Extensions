namespace OpenTracing.Contrib.Extensions
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;

    using OpenTracing.Contrib.Extensions.TangentialCode.Reflection;

    /// <summary>
    /// Extensions for <see cref="ISpan"/>, for logging a single item
    /// </summary>
    [PublicAPI]
    public static partial class SpanExtensions
    {
        public static ISpan Log(
            this ISpan span,
            string key,
            object value)
        {
            return span
                .Log(new[]
                {
                    new KeyValuePair<string, object>(key, value),
                });
        }

        public static ISpan Log(
            this ISpan span,
            DateTimeOffset timestamp,
            string key,
            object value)
        {
            return span
                .Log(
                    timestamp,
                    new[]
                    {
                        new KeyValuePair<string, object>(key, value),
                    });
        }

        public static ISpan Log(
            this ISpan span,
            KeyValuePair<string, object> field)
        {
            return span
                .Log(
                    new[]
                    {
                        field
                    });
        }

        public static ISpan Log(
            this ISpan span,
            DateTimeOffset timestamp,
            KeyValuePair<string, object> field)
        {
            return span
                .Log(
                    timestamp,
                    new[]
                    {
                        field
                    });
        }

        public static ISpan SetTagsFromAnonymousType<T>(
            this ISpan span,
            T anonymousType)
        {
            foreach (var fieldInfo in AnonymousTypeReflectionCache<T>.Instance.FieldNamesAndGetters)
            {
                var fieldName = fieldInfo.Key;
                var fieldGetter = fieldInfo.Value;

                var fieldValue = fieldGetter(anonymousType);

                span = span
                    .SetTag(fieldName, fieldValue);
            }

            return span;
        }
    }
}