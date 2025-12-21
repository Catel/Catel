namespace Catel.MVVM
{
    using System;
    using Data;

    /// <summary>
    /// Extension methods for view model classes.
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Gets the view model command manager for the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>IViewModelCommandManager.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public static IViewModelCommandManager GetViewModelCommandManager(this ViewModelBase viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            return viewModel.ViewModelCommandManager;
        }
    }
}
