// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangeUndo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System;
    using Reflection;

    /// <summary>
    /// This implements the undo mechanics for a property change.
    /// </summary>
    /// <remarks>
    /// Note that this does not support index array properties.
    /// </remarks>
    public class PropertyChangeUndo : UndoBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeUndo"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c>.</exception>
        public PropertyChangeUndo(object target, string propertyName, object oldValue, object newValue = null, object tag = null)
            : base(target, tag)
        {
            Argument.IsNotNull("target", target);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            CanRedo = true;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Method that will actually undo the action.
        /// </summary>
        protected override void UndoAction()
        {
            PropertyHelper.SetPropertyValue(Target, PropertyName, OldValue);
        }

        /// <summary>
        /// Method that will actually redo the action. There is no need to check for <see cref="IMementoSupport.CanRedo"/> because
        /// this will be done internally.
        /// </summary>
        protected override void RedoAction()
        {
            PropertyHelper.SetPropertyValue(Target, PropertyName, NewValue);
        }
        #endregion
    }
}