namespace Catel.Threading
{
    /// <summary>
    /// Extension methods for the SynchronizationContext.
    /// </summary>
    public static class SynchronizationContextExtensions
    {
        /// <summary>
        /// Acquires the scope of the <see cref="SynchronizationContext"/>. When the token is disposed, the context is released.
        /// </summary>
        /// <param name="synchronizationContext">The synchronization context.</param>
        /// <returns>IDisposableToken&lt;SynchronizationContext&gt;.</returns>
        public static IDisposableToken<SynchronizationContext> AcquireScope(this SynchronizationContext synchronizationContext)
        {
            return new DisposableToken<SynchronizationContext>(synchronizationContext, x => x.Instance.Acquire(), x => x.Instance.Release());
        }
    }
}
