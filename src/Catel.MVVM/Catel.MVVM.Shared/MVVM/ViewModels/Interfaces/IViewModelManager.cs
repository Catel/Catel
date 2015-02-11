﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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

        /// <summary>
        /// Registers a view model instance with the manager. All view models must register themselves to the manager.
        /// </summary>
        /// <param name="viewModel">The view model to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        void RegisterViewModelInstance(IViewModel viewModel);

        /// <summary>
        /// Unregisters a view model instance from the manager. All view models must unregister themselves from the manager.
        /// </summary>
        /// <param name="viewModel">The view model to unregister.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        void UnregisterViewModelInstance(IViewModel viewModel);

        /// <summary>
        /// Adds an interested view model instance. The <see cref="IViewModel"/> class will automatically register
        /// itself to the manager by using this method when decorated with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model the <paramref name="viewModel"/> is interested in.</param>
        /// <param name="viewModel">The view model instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        void AddInterestedViewModelInstance(Type viewModelType, IViewModel viewModel);

        /// <summary>
        /// Removes an interested view model instance. The <see cref="IViewModel"/> class will automatically unregister
        /// itself from the manager by using this method when decorated with the <see cref="InterestedInAttribute"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model the <paramref name="viewModel"/> was interested in.</param>
        /// <param name="viewModel">The view model instance.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        void RemoveInterestedViewModelInstance(Type viewModelType, IViewModel viewModel);
    }
}