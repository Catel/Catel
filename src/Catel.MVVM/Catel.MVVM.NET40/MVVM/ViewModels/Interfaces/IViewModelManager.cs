// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for the view model manager which allows the retrieval of currently alive view models.
    /// </summary>
    public interface IViewModelManager
    {
        #region Properties
        /// <summary>
        /// Gets the active view models presently registered.
        /// </summary>
        IEnumerable<IViewModel> ActiveViewModels { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the model of a view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        void RegisterModel(IViewModel viewModel, object model);

        /// <summary>
        /// Unregisters the model of a view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        void UnregisterModel(IViewModel viewModel, object model);

        /// <summary>
        /// Unregisters all models of a view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        void UnregisterAllModels(IViewModel viewModel);

        /// <summary>
        /// Gets the view models of a model.
        /// </summary>
        /// <param name="model">The model to find the linked view models for.</param>
        /// <returns>An array containing all the view models.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        IViewModel[] GetViewModelsOfModel(object model);

        /// <summary>
        /// Gets the view model by its unique identifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns>The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.</returns>
        IViewModel GetViewModel(int uniqueIdentifier);

        /// <summary>
        /// Gets the first or default instance of the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns>The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.</returns>
        TViewModel GetFirstOrDefaultInstance<TViewModel>() 
            where TViewModel : IViewModel;

        /// <summary>
        /// Gets the first or default instance of the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns>The <see cref="IViewModel"/> or <c>null</c> if the view model is not registered.</returns>
        /// <exception cref="System.ArgumentException">The <paramref name="viewModelType"/> is not of type <see cref="IViewModel"/>.</exception>
        IViewModel GetFirstOrDefaultInstance(Type viewModelType);

        /// <summary>
        /// Gets the child view models of the specified view model.
        /// </summary>
        /// <param name="parentViewModel">The parent view model.</param>
        /// <returns>The child view models.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parentViewModel"/> is <c>null</c>.</exception>
        IEnumerable<IRelationalViewModel> GetChildViewModels(IViewModel parentViewModel);

        /// <summary>
        /// Gets the child view models of the specified view model unique identifier.
        /// </summary>
        /// <param name="parentUniqueIdentifier">The parent unique identifier.</param>
        /// <returns>The child view models.</returns>
        IEnumerable<IRelationalViewModel> GetChildViewModels(int parentUniqueIdentifier);
        #endregion
    }
}