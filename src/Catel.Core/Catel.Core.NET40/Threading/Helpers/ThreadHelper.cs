// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
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
            return System.Threading.Thread.CurrentThread.ManagedThreadId;
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
            new System.Threading.ManualResetEvent(false).WaitOne(millisecondsTimeout);
#else
            System.Threading.Thread.Sleep(millisecondsTimeout);
#endif
        }
    }
}