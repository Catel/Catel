// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskProgressLogger.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="percentage">
        /// The percentage.
        /// </param>
        void UpdateStatus(string message, int percentage);

        /// <summary>
        /// Update the task status.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void UpdateStatus(string message);


        /// <summary>
        /// Update the task status.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="indeterminate">
        /// The indeterminate state.
        /// </param>
        void UpdateStatus(string message, bool indeterminate);

        #endregion
    }
}