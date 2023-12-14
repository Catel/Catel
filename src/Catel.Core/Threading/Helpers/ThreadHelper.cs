namespace Catel
{
    using System.Threading;

    /// <summary>
    /// Helper class for thread methods.
    /// </summary>
    public static class ThreadHelper
    {
        /// <summary>
        /// Gets the current thread identifier.
        /// </summary>
        /// <returns>System.String.</returns>
        public static int GetCurrentThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Lets the current execution thread sleep for the specified milliseconds.
        /// <para />
        /// In WinRT, this method uses the Task to delay.
        /// </summary>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        public static void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        /// <summary>
        /// Causes a thread to wait the number of times defined by the iterations parameter.
        /// </summary>
        /// <param name="iterations">The number of iterations.</param>
        public static void SpinWait(int iterations)
        {
            Thread.SpinWait(iterations);
        }
    }
}
