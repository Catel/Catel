﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAdvancedEditableObject.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace System.ComponentModel
{
    using System;

    /// <summary>
    /// Interface extending the <see cref="IEditableObject"/> interface with events which allows preventing the
    /// object from being edited.
    /// </summary>
    public interface IAdvancedEditableObject : IEditableObject
    {
        /// <summary>
        /// Occurs when the object is about to be edited. This event gives an opportunity to cancel the call
        /// to <see cref="IEditableObject.BeginEdit"/>.
        /// </summary>
        event EventHandler<BeginEditEventArgs> BeginEditing;

        /// <summary>
        /// Occurs when the editing of the object has just been canceled.
        /// </summary>
        event EventHandler<CancelEditEventArgs> CancelEditing;

        /// <summary>
        /// Occurs when the edit cancel has been completed or canceled.
        /// </summary>
        event EventHandler<EventArgs> CancelEditingCompleted;

        /// <summary>
        /// Occurs when the editing of the object has just been ended.
        /// </summary>
        event EventHandler<EndEditEventArgs> EndEditing;
    }
}