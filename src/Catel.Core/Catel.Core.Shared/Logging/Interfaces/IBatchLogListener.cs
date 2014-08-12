// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBatchLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System.Threading.Tasks;

    /// <summary>
    /// Log listener base which allows to write log files in batches.
    /// </summary>
    public interface IBatchLogListener
    {
        #region Methods
        /// <summary>
        /// Flushes the current queue asynchronous.
        /// </summary>
        /// <returns>Task so it can be awaited.</returns>
        Task Flush();
        #endregion
    }
}