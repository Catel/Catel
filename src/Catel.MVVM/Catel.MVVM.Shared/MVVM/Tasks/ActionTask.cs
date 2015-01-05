// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionTask.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Tasks
{
    using System;

    /// <summary>
    /// The action task.
    /// </summary>
    public class ActionTask : TaskBase
    {
        #region Fields
        /// <summary>
        /// The _action.
        /// </summary>
        private readonly Action<ITaskProgressTracker> _action;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTask" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public ActionTask(string name, Action<ITaskProgressTracker> action)
            : base(name)
        {
            Argument.IsNotNull("action", action);

            _action = action;
        }
        #endregion

        #region Methods
        /// <summary>
        /// The execute.
        /// </summary>
        public override void Execute()
        {
            _action.Invoke(new ActionTaskTaskProgressTracker(this));
        }
        #endregion

        #region Nested type: ActionTaskTaskProgressTracker
        /// <summary>
        /// The action task progress log.
        /// </summary>
        internal class ActionTaskTaskProgressTracker : ITaskProgressTracker
        {
            #region Fields
            /// <summary>
            /// The action task.
            /// </summary>
            private readonly ActionTask _actionTask;
            
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="ActionTaskTaskProgressTracker" /> class.
            /// </summary>
            /// <param name="actionTask">
            /// The action task.
            /// </param>
            public ActionTaskTaskProgressTracker(ActionTask actionTask)
            {
                _actionTask = actionTask;
            }
            #endregion

            #region ITaskProgressTracker Members
            /// <summary>
            /// Update the task status.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="percentage">
            /// The percentage.
            /// </param>
            public void UpdateStatus(string message, int percentage)
            {
                UpdateStatus(message);
                 _actionTask.Percentage = percentage;
            }

            /// <summary>
            /// Update the task status.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            public void UpdateStatus(string message)
            {
                _actionTask.Message = message;
            }

            /// <summary>
            /// Update the task status.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            /// <param name="indeterminate">
            /// The indeterminate state.
            /// </param>
            public void UpdateStatus(string message, bool indeterminate)
            {
                UpdateStatus(message);
                _actionTask.IsIndeterminate = indeterminate;
            }
            #endregion
        }
        #endregion
    }
}