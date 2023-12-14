namespace Catel.Windows.Threading
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// Extension methods for the dispatcher.
    /// </summary>
    public static partial class DispatcherExtensions
    {
        /// <summary>
        /// Gets the managed thread identifier for the specified dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns>The managed thread id.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcher" /> is <c>null</c>.</exception>
        public static int GetThreadId(this Dispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(dispatcher);

            return dispatcher.Thread.ManagedThreadId;
        }
    }
}
