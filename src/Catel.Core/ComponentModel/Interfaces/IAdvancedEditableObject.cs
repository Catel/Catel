namespace Catel.ComponentModel
{
    using System;
    using System.ComponentModel;

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
