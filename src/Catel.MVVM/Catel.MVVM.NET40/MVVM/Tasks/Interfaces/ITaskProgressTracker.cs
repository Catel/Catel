// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskProgressLogger.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Tasks
{
    /// <summary>
    /// The progress log
    /// </summary>
    public interface ITaskProgressTracker
    {
        #region Methods

        /// <summary>
        /// Update the task status.
        /// </summary>
        /// <param name="percentage">
        /// The percentage.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void UpdateStatus(int percentage, string message);
        #endregion
    }
}