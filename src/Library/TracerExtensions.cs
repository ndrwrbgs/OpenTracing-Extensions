namespace OpenTracing.Contrib.Extensions
{
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    [PublicAPI]
    public static class TracerExtensions
    {
        public static ISpanBuilder BuildSpanUsingMemberName(
            this ITracer tracer,
            [CallerMemberName] string memberName = null)
        {
            return tracer
                .BuildSpan(memberName);
        }
    }
}