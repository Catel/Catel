// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace System.ComponentModel
{
    using System;
    using System.ComponentModel;
    using Catel;

    /// <summary>
    /// Event args class containing information about events that occur in the <see cref="IAdvancedEditableObject"/>
    /// interface.
    /// </summary>
    public class EditEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EditEventArgs"/> class.
        /// </summary>
        /// <param name="editableObject">The editable object.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="editableObject"/> is <c>null</c>.</exception>
        public EditEventArgs(IEditableObject editableObject)
        {
            Argument.IsNotNull("editableObject", editableObject);

            EditableObject = editableObject;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the editable object.
        /// </summary>
        public IEditableObject EditableObject { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the call to should fail and the 
        /// object should not enter or leave the edit state.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
        #endregion
    }
}