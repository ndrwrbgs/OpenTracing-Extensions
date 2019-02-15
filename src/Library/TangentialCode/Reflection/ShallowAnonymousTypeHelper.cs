namespace OpenTracing.Contrib.Extensions.TangentialCode.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using JetBrains.Annotations;
    using OpenTracing.Contrib.Extensions.TangentialCode.Linq;

    /// <summary>
    ///     Uses Reflection to get shallow information about a specific <see cref="Type"/>.
    ///     Does NOT walk down the <see cref="Type" /> tree
    /// </summary>
    internal static class ShallowAnonymousTypeHelper
    {
        [NotNull]
        public static ICollection<KeyValuePair<string, Func<T, string>>> GetFieldNamesAndToStringGetters<T>()
        {
            FieldInfo[] fields = typeof(T).GetFields();

            return fields
                .ToArray(
                    field => new KeyValuePair<string, Func<T, string>>(
                        field.Name,
                        contractItem => field.GetValue(contractItem)?.ToString()));
        }
    }
}