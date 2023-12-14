namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// EventArgs base which implements the Cancel property.
    /// </summary>
    public abstract class CancellableEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
        /// </summary>
        public CancellableEventArgs()
        {
            Cancel = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the action should be canceled.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the action should be canceled; otherwise, <c>false</c>.
        /// </value>
        public bool Cancel { get; set; }
    }
}
