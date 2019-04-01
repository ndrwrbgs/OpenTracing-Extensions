namespace OpenTracing.Contrib.Extensions
{
    using System;

    public interface IYieldReadyScope : IScope
    {
        /// <summary>
        /// To be called right before and disposed right after a `yield` in an IEnumerable.
        /// Stashes this <see cref="IScope"/> for later resumption after the caller handles the IEnumerable item.
        /// </summary>
        IDisposable Yielding();
    }
}