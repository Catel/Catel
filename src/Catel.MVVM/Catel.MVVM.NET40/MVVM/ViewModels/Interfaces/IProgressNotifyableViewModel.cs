// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressNotifyableViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using Catel.MVVM.Tasks;

    /// <summary>
    /// The ProgressNotifyableViewModel interface.
    /// </summary>
    public interface IProgressNotifyableViewModel : IViewModel
    {
        #region Properties
        /// <summary>
        /// Gets the message.
        /// </summary>
        string DetailedMessage { get; }

        /// <summary>
        /// Gets the percent.
        /// </summary>
        int Percentage { get; }

        /// <summary>
        /// Gets the task.
        /// </summary>
        ITask Task { get; }

        /// <summary>
        /// Gets or sets the task message.
        /// </summary>
        string TaskMessage { get; }

        /// <summary>
        /// Gets or sets the task name.
        /// </summary>
        string TaskName { get; }

        /// <summary>
        /// Gets or sets the task percentage.
        /// </summary>
        int TaskPercentage { get; }

        /// <summary>
        /// Gets or sets the task percentage.
        /// </summary>
        bool TaskIsIndeterminate { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Update the progress status.
        /// </summary>
        /// <param name="currentItem">
        /// The current item.
        /// </param>
        /// <param name="totalItems">
        /// The total items.
        /// </param>
        /// <param name="task">
        /// The task
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="task" /> is <c>null</c>.
        /// </exception>
        void UpdateStatus(int currentItem, int totalItems, ITask task);
        #endregion
    }
}