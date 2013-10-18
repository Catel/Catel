// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBatchLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    /// <summary>
    /// Log listener base which allows to write log files in batches.
    /// </summary>
    public interface IBatchLogListener
    {
        #region Methods
        /// <summary>
        /// Flushes the current queue.
        /// </summary>
        void Flush();
        #endregion
    }
}