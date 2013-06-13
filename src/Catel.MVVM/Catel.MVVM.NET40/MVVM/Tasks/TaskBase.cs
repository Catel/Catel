// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
    /// This class inherits from <see cref="DataObjectBase"/> in use it as model as part of the wizard view models.
    /// </remarks>
    public abstract class TaskBase : ModelBase, ITask
    {
        #region Constants

        /// <summary>Register the Message property so it is known in the class.</summary>
        public static readonly PropertyData MessageProperty = RegisterProperty("Message", typeof(string));

        /// <summary>Register the Percentage property so it is known in the class.</summary>
        public static readonly PropertyData PercentageProperty = RegisterProperty("Percentage", typeof(int));

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The task name name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> is <c>null</c>.
        /// </exception>
        protected TaskBase(string name)
        {
            Argument.IsNotNull("name", name);
            
            Name = name;
        }

        #endregion

        #region ITask Members

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
    }
}