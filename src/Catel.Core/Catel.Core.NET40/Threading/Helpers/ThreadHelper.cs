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