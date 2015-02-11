﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeginEditEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace System.ComponentModel
{
    /// <summary>
    /// Event args implementation called when the object is about to call <see cref="IEditableObject.BeginEdit"/>. This way,
    /// it is possible to cancel the events.
    /// </summary>
    public class BeginEditEventArgs : EditEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeginEditEventArgs"/> class.
        /// </summary>
        /// <param name="editableObject">The editable object.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="editableObject"/> is <c>null</c>.</exception>
        public BeginEditEventArgs(IEditableObject editableObject)
            : base(editableObject)
        {
        }
    }
}