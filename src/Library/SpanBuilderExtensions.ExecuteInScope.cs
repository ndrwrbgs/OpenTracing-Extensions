namespace OpenTracing.Contrib.Extensions
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using OpenTracing.Util;

    /// <summary>
    /// Extensions for managing a <see cref="IScope"/>. Methods that handle creation of the <see cref="IScope"/> extend <see cref="ISpanBuilder"/>.
    /// </summary>
    public static partial class SpanBuilderExtensions
    {
        public static void ExecuteInScope(
            [NotNull] this ISpanBuilder spanBuilder,
            [NotNull] Action action)
        {
            using (IScope scope = spanBuilder.StartActive(true))
            {
                action();
            }
        }

        public static T ExecuteInScope<T>(
            [NotNull] this ISpanBuilder spanBuilder,
            [NotNull] Func<T> action)
        {
            using (IScope scope = spanBuilder.StartActive(true))
            {
                return action();
            }
        }

        public static async Task ExecuteInScopeAsync(
            [NotNull] this ISpanBuilder spanBuilder,
            [NotNull] Func<Task> action)
        {
            using (IScope scope = spanBuilder.StartActive(true))
            {
                await action().ConfigureAwait(false);
            }
        }

        public static async Task ExecuteInScopeAsync(
            [NotNull] this ISpanBuilder spanBuilder,
            [NotNull] Func<ISpan, Task> action)
        {
            using (IScope scope = spanBuilder.StartActive(true))
            {
                await action(scope.Span).ConfigureAwait(false);
            }
        }

        public static async Task<T> ExecuteInScopeAsync<T>(
            [NotNull] this ISpanBuilder spanBuilder,
            [NotNull] Func<Task<T>> action)
        {
            using (IScope scope = spanBuilder.StartActive(true))
            {
                return await action().ConfigureAwait(false);
            }
        }

        public static async Task<T> ExecuteInScopeAsync<T>(
            [NotNull] this ISpanBuilder spanBuilder,
            [NotNull] Func<ISpan, Task<T>> action)
        {
            using (IScope scope = spanBuilder.StartActive(true))
            {
                return await action(scope.Span).ConfigureAwait(false);
            }
        }
    }
}