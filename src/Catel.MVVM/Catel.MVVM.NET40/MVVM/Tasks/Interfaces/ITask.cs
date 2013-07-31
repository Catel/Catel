// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITask.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Tasks
{
    using System.ComponentModel;

    /// <summary>
    /// The TaskBase interface.
    /// </summary>
    public interface ITask
    {
        #region Properties
        /// <summary>
        /// Occurs when a property of this object has changed.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the message
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the percentage
        /// </summary>
        int Percentage { get; }

        /// <summary>
        /// Indicates whether the task progress is indeterminate. 
        /// </summary>
        bool IsIndeterminate { get; }

        /// <summary>
        /// Gets or sets whether this task should automatically be dispatched to the UI thread.
        /// </summary>
        bool AutomaticallyDispatch { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// The execute.
        /// </summary>
        void Execute();

        /// <summary>
        /// The rollback.
        /// </summary>
        void Rollback();
        #endregion
    }
}