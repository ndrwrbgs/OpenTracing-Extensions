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
        public static ICollection<KeyValuePair<string, Func<T, string>>> GetFieldAndPropertyNamesAndToStringGetters<T>()
        {
            FieldInfo[] fields = typeof(T).GetFields();

            var fieldToStringGetters = fields
                .ToArray(
                    field => new KeyValuePair<string, Func<T, string>>(
                        field.Name,
                        contractItem => field.GetValue(contractItem)?.ToString()));
            
            PropertyInfo[] properties = typeof(T).GetProperties();

            var propertyToStringGetters = properties
                .ToArray(
                    property => new KeyValuePair<string, Func<T, string>>(
                        property.Name,
                        contractItem => property.GetValue(contractItem)?.ToString()));

            var result = new KeyValuePair<string, Func<T, string>>[fieldToStringGetters.Length + propertyToStringGetters.Length];
            for (int i = 0; i < fieldToStringGetters.Length; i++)
            {
                result[i] = fieldToStringGetters[i];
            }

            for (int i = 0; i < propertyToStringGetters.Length; i++)
            {
                result[i + fieldToStringGetters.Length] = propertyToStringGetters[i];
            }

            return result;
        }
    }
}