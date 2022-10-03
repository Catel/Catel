namespace System.ComponentModel
{
    /// <summary>
    /// Event args implementation called when the object is about to call <see cref="IEditableObject.BeginEdit"/>. This way,
    /// it is possible to cancel the events.
    /// </summary>
    public class CancelEditEventArgs : EditEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEditEventArgs"/> class.
        /// </summary>
        /// <param name="editableObject">The editable object.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="editableObject"/> is <c>null</c>.</exception>
        public CancelEditEventArgs(IEditableObject editableObject)
            : base(editableObject)
        {
        }
    }
}
