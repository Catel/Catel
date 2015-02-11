﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelManagerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Linq;

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
        public static void SaveAndCloseViewModels(this IViewModelManager viewModelManager, Func<IViewModel, bool> predicate)
        {
            Argument.IsNotNull(() => viewModelManager);
            Argument.IsNotNull(() => predicate);

            var activeViewModels = viewModelManager.ActiveViewModels.ToList();
            foreach (var viewModel in activeViewModels)
            {
                if (predicate(viewModel))
                {
                    viewModel.SaveAndCloseViewModel();
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
        public static void CancelAndCloseViewModels(this IViewModelManager viewModelManager, Func<IViewModel, bool> predicate)
        {
            Argument.IsNotNull(() => viewModelManager);
            Argument.IsNotNull(() => predicate);

            var activeViewModels = viewModelManager.ActiveViewModels.ToList();
            foreach (var viewModel in activeViewModels)
            {
                if (predicate(viewModel))
                {
                    viewModel.CancelAndCloseViewModel();
                }
            }
        }
    }
}