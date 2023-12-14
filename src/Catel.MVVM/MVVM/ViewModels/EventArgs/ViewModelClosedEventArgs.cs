namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> implementation for the <see cref="IViewModel.ClosedAsync"/> event.
    /// </summary>
    public class ViewModelClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelClosedEventArgs" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public ViewModelClosedEventArgs(IViewModel viewModel, bool? result)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            ViewModel = viewModel;
            Result = result;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public IViewModel ViewModel { get; private set; }

        /// <summary>
        /// Gets the result to pass to the view. This will, for example, be used as <c>DialogResult</c>
        /// </summary>
        /// <value>The result.</value>
        public bool? Result { get; private set; }
    }
}
