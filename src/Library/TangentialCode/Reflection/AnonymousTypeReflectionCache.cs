namespace OpenTracing.Contrib.Extensions.TangentialCode.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// Caches the values for <typeparamref name="T"/> to avoid performance implications of having to reflect
    /// over the type on every call.
    /// NOTE - Only valid to use if the user is NOT dynamically creating an unending multitude of dynamic runtime types.
    /// Otherwise, this would result in a memory leak. As this is fortunately not a common scenario, including it for the
    /// performance gains.
    /// </summary>
    internal sealed class AnonymousTypeReflectionCache<T>
    {
        private AnonymousTypeReflectionCache()
        {
            this.FieldNamesAndGetters = ShallowAnonymousTypeHelper.GetFieldNamesAndToStringGetters<T>().ToArray();
        }

        [NotNull]
        public static AnonymousTypeReflectionCache<T> Instance { get; } = new AnonymousTypeReflectionCache<T>();

        [NotNull] public KeyValuePair<string, Func<T, string>>[] FieldNamesAndGetters { get; set; }
    }
}