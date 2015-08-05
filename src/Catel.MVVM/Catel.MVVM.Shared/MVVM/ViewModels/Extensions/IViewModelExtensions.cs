// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IViewModel"/>.
    /// </summary>
    public static class IViewModelExtensions
    {
        /// <summary>
        /// Saves the data, but also closes the view model in the same call if the save succeeds.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public static async Task<bool> SaveAndCloseViewModelAsync(this IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var result = await viewModel.SaveViewModel();
            if (result)
            {
                await viewModel.CloseViewModel(true);
            }

            return result;
        }

        /// <summary>
        /// Cancels the editing of the data, but also closes the view model in the same call.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public static async Task<bool> CancelAndCloseViewModelAsync(this IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var result = await viewModel.CancelViewModel();
            if (result)
            {
                await viewModel.CloseViewModel(false);
            }

            return result;
        }
    }
}