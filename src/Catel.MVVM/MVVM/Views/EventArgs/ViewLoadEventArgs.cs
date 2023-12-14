namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// EventArgs implementation for when a <see cref="IView"/> is loaded.
    /// </summary>
    public class ViewLoadEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLoadEventArgs"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public ViewLoadEventArgs(IView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            View = view;
        }

        /// <summary>
        /// Gets the view that has just been loaded.
        /// </summary>
        /// <value>The view.</value>
        public IView View { get; private set; }
    }
}
