// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBatchLogListenerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System.Threading.Tasks;

    /// <summary>
    /// IBatchLogListener extensions.
    /// </summary>
    public static class IBatchLogListenerExtensions
    {
        /// <summary>
        /// Flushes the current queue asynchronous.
        /// </summary>
        /// <returns>Task so it can be awaited.</returns>
        public static async Task FlushAsync(this IBatchLogListener batchLogListener)
        {
            await Task.Factory.StartNew(() => batchLogListener.Flush());
        }
    }
}