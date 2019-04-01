namespace OpenTracing.Contrib.Extensions
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using OpenTracing.Util;

    /// <summary>
    /// Extensions for when inside an IEnumerable and, even though the compiler makes it that way, you don't want
    /// the caller code to 'belong to' or appear to be 'called by' your internal spans.
    ///
    /// <see href="https://github.com/opentracing/opentracing-csharp/issues/106">Instrumentation inside an IEnumerable</see>
    /// </summary>
    [PublicAPI]
    public static class EnumerableYieldExtensions
    {
        /// <summary>
        /// Wraps the enumeration with the specified <see cref="spanBuilder"/>, returning to the parent context (removing
        /// the 'enumerate' scope from <see cref="IScopeManager.Active"/>) before each yield return to the caller.
        /// </summary>
        public static IEnumerable<T> WrapWithTracing<T>(
            this IEnumerable<T> source,
            Func<ISpanBuilder> spanBuilder,
            IScopeManager scopeManager)
        {
            using (IYieldReadyScope scope = spanBuilder().StartActiveYieldReadyScope(scopeManager))
            {
                foreach (var item in source)
                {
                    using (scope.Yielding())
                        yield return item;
                }
            }
        }

        /// <summary>
        /// Wraps the enumeration with the specified <see cref="spanBuilder"/>, returning to the parent context (removing
        /// the 'enumerate' scope from <see cref="IScopeManager.Active"/>) before each yield return to the caller.
        /// </summary>
        public static IEnumerable<T> WrapWithTracing<T>(
            this IEnumerable<T> source,
            Func<ISpanBuilder> spanBuilder)
        {
            using (IYieldReadyScope scope = spanBuilder().StartActiveYieldReadyScope())
            {
                foreach (var item in source)
                {
                    using (scope.Yielding())
                        yield return item;
                }
            }
        }

        public static IYieldReadyScope StartActiveYieldReadyScope(
            this ISpanBuilder spanBuilder,
            bool finishOnDispose = true)
        {
            return StartActiveYieldReadyScope(spanBuilder, finishOnDispose, GlobalTracer.Instance.ScopeManager);
        }

        public static IYieldReadyScope StartActiveYieldReadyScope(
            this ISpanBuilder spanBuilder,
            IScopeManager scopeManager)
        {
            return StartActiveYieldReadyScope(spanBuilder, true, scopeManager);
        }

        public static IYieldReadyScope StartActiveYieldReadyScope(
            this ISpanBuilder spanBuilder,
            bool finishOnDispose,
            IScopeManager scopeManager)
        {
            IScope startActive = spanBuilder.StartActive(false);
            return new YieldReadyScope(startActive, finishOnDispose, scopeManager);
        }

        private sealed class YieldReadyScope : IYieldReadyScope
        {
            private IScope currentlyActiveScope;
            private readonly bool finishOnDispose;
            private readonly IScopeManager scopeManager;

            public YieldReadyScope(
                IScope startActive,
                bool finishOnDispose,
                IScopeManager scopeManager)
            {
                this.currentlyActiveScope = startActive;
                this.finishOnDispose = finishOnDispose;
                this.scopeManager = scopeManager;

                this.Span = this.currentlyActiveScope.Span;
            }

            public void Dispose()
            {
                // Finish of the overall Scope
                if (this.finishOnDispose)
                {
                    this.currentlyActiveScope.Span.Finish();
                }

                // Disposes the SCOPE not finishing the
                this.currentlyActiveScope.Dispose();
            }

            public ISpan Span { get; }

            public IDisposable Yielding()
            {
                // Disposes the SCOPE not finishing the SPAN
                this.currentlyActiveScope.Dispose();

                return new ActivateSpanOnDispose(
                    this.Span,
                    this.scopeManager,
                    setCurrentlyActiveOn: this);
            }

            private sealed class ActivateSpanOnDispose : IDisposable
            {
                private readonly ISpan span;
                private readonly IScopeManager scopeManager;
                private readonly YieldReadyScope setCurrentlyActiveOn;

                public ActivateSpanOnDispose(
                    ISpan span,
                    IScopeManager scopeManager,
                    YieldReadyScope setCurrentlyActiveOn)
                {
                    this.span = span;
                    this.scopeManager = scopeManager;
                    this.setCurrentlyActiveOn = setCurrentlyActiveOn;
                }

                public void Dispose()
                {
                    var newScope = this.scopeManager.Activate(this.span, false);
                    this.setCurrentlyActiveOn.currentlyActiveScope = newScope;
                }
            }
        }
    }
}