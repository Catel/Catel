// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using Windows.Controls;

    /// <summary>
    /// Manager that can search for views belonging to a view model.
    /// </summary>
    public interface IViewManager
    {
        #region Properties
        /// <summary>
        /// Gets the active views presently registered.
        /// </summary>
        IEnumerable<IView> ActiveViews { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Registers a view so it can be linked to a view model instance.
        /// </summary>
        /// <param name="view">The view to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        void RegisterView(IView view);

        /// <summary>
        /// Unregisters a view so it can no longer be linked to a view model instance.
        /// </summary>
        /// <param name="view">The view to unregister.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        void UnregisterView(IView view);

        /// <summary>
        /// Gets the views of view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>An array containing all the views that are linked to the view.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        IView[] GetViewsOfViewModel(IViewModel viewModel);

        /// <summary>
        /// Gets the first or default instance of the specified view type.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <returns>The <see cref="TView"/> or <c>null</c> if the view is not registered.</returns>
        TView GetFirstOrDefaultInstance<TView>() where TView : IView;

        /// <summary>
        /// Gets the first or default instance of the specified view type.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        IView GetFirstOrDefaultInstance(Type viewType);
        #endregion
    }
}