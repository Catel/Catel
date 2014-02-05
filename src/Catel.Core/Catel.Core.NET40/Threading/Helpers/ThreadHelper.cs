// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
#if NETFX_CORE
            return System.Environment.CurrentManagedThreadId;
#else
            return Thread.CurrentThread.ManagedThreadId;
#endif
        }

        /// <summary>
        /// Lets the current execution thread sleep for the specified milliseconds.
        /// <para />
        /// In WinRT, this method uses the Task to delay.
        /// </summary>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        public static void Sleep(int millisecondsTimeout)
        {
#if NETFX_CORE || PCL
            new ManualResetEvent(false).WaitOne(millisecondsTimeout);
#else
            Thread.Sleep(millisecondsTimeout);
#endif
        }

        /// <summary>
        /// Causes a thread to wait the number of times defined by the iterations parameter.
        /// </summary>
        /// <param name="iterations">The number of iterations.</param>
        public static void SpinWait(int iterations)
        {
#if NETFX_CORE
            var spinWait = new SpinWait();

            while (spinWait.Count < iterations)
            {
                spinWait.SpinOnce();
            }
#elif PCL
            Sleep(1); // alternative for PCL
#else
            Thread.SpinWait(20);
#endif
        }
    }
}