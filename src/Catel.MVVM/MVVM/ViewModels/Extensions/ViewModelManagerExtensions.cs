// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelManagerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions for the <see cref="IViewModelManager"/> interface.
    /// </summary>
    public static class ViewModelManagerExtensions
    {
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
            Argument.IsNotNull("viewModelManager", viewModelManager);
            Argument.IsNotNull("predicate", predicate);

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
            Argument.IsNotNull("viewModelManager", viewModelManager);
            Argument.IsNotNull("predicate", predicate);

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