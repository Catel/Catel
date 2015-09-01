// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBatchLogListenerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System.Threading.Tasks;
    using Threading;

    /// <summary>
    /// IBatchLogListener extensions.
    /// </summary>
    public static class IBatchLogListenerExtensions
    {
        /// <summary>
        /// Flushes the current queue asynchronous.
        /// </summary>
        /// <returns>Task so it can be awaited.</returns>
        [ObsoleteEx(Message = "Member will be removed because it's not truly asynchronous", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
        public static Task FlushAsync(this IBatchLogListener batchLogListener)
        {
            return TaskHelper.Run(() => batchLogListener.Flush());
        }
    }
}