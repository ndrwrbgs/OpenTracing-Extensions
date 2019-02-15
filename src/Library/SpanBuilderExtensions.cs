namespace OpenTracing.Contrib.Extensions
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;
    using OpenTracing.Contrib.Extensions.TangentialCode.Reflection;

    [PublicAPI]
    public static partial class SpanBuilderExtensions
    {
        /// <remarks>
        /// <see cref="CallerLineNumberAttribute"/> is explicitly NOT used, since that can change
        /// over time and is likely not valid as a 'Tag' on a <see cref="ISpan"/>, since 'Tag's are
        /// meant to apply to the entirety of the span. Instead, see <see cref="ISpan.Log(IEnumerable{KeyValuePair{string,object}})"/>
        /// options like <see cref="SpanExtensions.LogMemberInfo"/>.
        /// </remarks>
        public static ISpanBuilder WithCallerMemberNameTag(
            ISpanBuilder spanBuilder,
            string tagName = Constants.CallerMemberNameTagName,
            [CallerMemberName] string callerMemberName = null)
        {
            return spanBuilder
                .WithTag(tagName, callerMemberName);
        }

        /// <summary>
        /// Uses reflection over the anonymous type defined by <typeparamref name="T"/>
        /// (via <paramref name="anonymousType"/>) to set each field of <typeparamref name="T"/>
        /// as a tag on <paramref name="spanBuilder"/>.
        /// </summary>
        public static ISpanBuilder WithTagsFromAnonymousType<T>(
            ISpanBuilder spanBuilder,
            T anonymousType)
        {
            foreach (var fieldInfo in AnonymousTypeReflectionCache<T>.Instance.FieldNamesAndGetters)
            {
                var fieldName = fieldInfo.Key;
                var fieldGetter = fieldInfo.Value;

                var fieldValue = fieldGetter(anonymousType);

                spanBuilder = spanBuilder
                    .WithTag(fieldName, fieldValue);
            }

            return spanBuilder;
        }
    }
}
