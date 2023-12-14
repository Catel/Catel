namespace Catel.MVVM
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions for the <see cref="IViewModelManager"/> interface.
    /// </summary>
    public static class ViewModelManagerExtensions
    {
        /// <summary>
        /// Gets the first or default instance of the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>
        /// The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.
        /// </returns>
        public static TViewModel? GetFirstOrDefaultInstance<TViewModel>(this IViewModelManager viewModelManager)
            where TViewModel : IViewModel
        {
            ArgumentNullException.ThrowIfNull(viewModelManager);

            return (TViewModel?)viewModelManager.GetFirstOrDefaultInstance(typeof(TViewModel));
        }

        /// <summary>
        /// Closes all view models that are currently being managed by the <see cref="ViewModelManager" /> which
        /// match the predicate.
        /// </summary>
        /// <param name="viewModelManager">The view model manager.</param>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelManager" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelManager" /> is <c>null</c>.</exception>
        public static async Task SaveAndCloseViewModelsAsync(this IViewModelManager viewModelManager, Func<IViewModel, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(viewModelManager);
            ArgumentNullException.ThrowIfNull(predicate);

            var activeViewModels = viewModelManager.ActiveViewModels;

            foreach (var viewModel in activeViewModels)
            {
                if (predicate(viewModel))
                {
                    await viewModel.SaveAndCloseViewModelAsync();
                }
            }
        }

        /// <summary>
        /// Closes all view models that are currently being managed by the <see cref="ViewModelManager" /> which
        /// match the predicate.
        /// </summary>
        /// <param name="viewModelManager">The view model manager.</param>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelManager" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelManager" /> is <c>null</c>.</exception>
        public static async Task CancelAndCloseViewModelsAsync(this IViewModelManager viewModelManager, Func<IViewModel, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(viewModelManager);
            ArgumentNullException.ThrowIfNull(predicate);

            var activeViewModels = viewModelManager.ActiveViewModels;

            foreach (var viewModel in activeViewModels)
            {
                if (predicate(viewModel))
                {
                    await viewModel.CancelAndCloseViewModelAsync();
                }
            }
        }
    }
}
