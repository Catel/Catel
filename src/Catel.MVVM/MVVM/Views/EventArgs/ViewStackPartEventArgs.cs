namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Event args when a part of a view stack is raises an event.
    /// </summary>
    public class ViewStackPartEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewStackPartEventArgs"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public ViewStackPartEventArgs(IView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            View = view;
        }

        /// <summary>
        /// Gets the view that has been loaded.
        /// </summary>
        /// <value>The view.</value>
        public IView View { get; private set; }
    }
}
