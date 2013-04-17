// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITask.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
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
        /// Occurs when a property of this object has changed.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

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