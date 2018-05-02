// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Tasks
{
    using System;

    using Catel.Data;

    /// <summary>
    /// The task base.
    /// </summary>
    /// <remarks>
    /// This class inherits from <see cref="ModelBase" /> in use it as model as part of the wizard view models.
    /// </remarks>
    public abstract class TaskBase : ModelBase, ITask
    {
        #region Constants
        /// <summary>Register the Message property so it is known in the class.</summary>
        public static readonly PropertyData MessageProperty = RegisterProperty("Message", typeof(string));

        /// <summary>Register the Percentage property so it is known in the class.</summary>
        public static readonly PropertyData PercentageProperty = RegisterProperty("Percentage", typeof(int), default(int), (sender, args) => ((TaskBase)sender).PercentagePropertyChanged());

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));

        /// <summary>
        /// Register the IsIndeterminate property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IsIndeterminatedProperty = RegisterProperty("IsIndeterminate", typeof(bool), true);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBase" /> class.
        /// </summary>
        /// <param name="name">The task name name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="name" /> is <c>null</c>.</exception>
        protected TaskBase(string name)
        {
            Argument.IsNotNull("name", name);

            Name = name;
        }
        #endregion

        #region ITask Members
        /// <summary>
        /// Gets or sets whether this task should automatically be dispatched to the UI thread.
        /// </summary>
        public bool AutomaticallyDispatch
        {
            get { return GetValue<bool>(AutomaticallyDispatchProperty); }
            set { SetValue(AutomaticallyDispatchProperty, value); }
        }

        /// <summary>
        /// Register the AutomaticallyDispatch property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AutomaticallyDispatchProperty = RegisterProperty("AutomaticallyDispatch", typeof(bool), false);

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            private set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message
        {
            get { return GetValue<string>(MessageProperty); }
            protected internal set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public int Percentage
        {
            get { return GetValue<int>(PercentageProperty); }
            protected internal set { SetValue(PercentageProperty, value); }
        }

        /// <summary>
        /// Indicates whether the task progress is indeterminated
        /// </summary>
        public bool IsIndeterminate
        {
            get { return GetValue<bool>(IsIndeterminatedProperty); }
            protected internal set { SetValue(IsIndeterminatedProperty, value); }
        }

        /// <summary>
        /// The execute.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// The rollback.
        /// </summary>
        public virtual void Rollback()
        {
        }
        #endregion

        #region Methods
        private void PercentagePropertyChanged()
        {
            IsIndeterminate = false;
        }
        #endregion
    }
}