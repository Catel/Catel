// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Interface which allows custom instantation of view models. This way, if a view model contains a complex constructor or needs 
    /// caching, this factory can be used.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates a new view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model that needs to be created.</param>
        /// <param name="dataContext">The data context of the view model.</param>
        /// <returns>The newly created <see cref="IViewModel"/> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement the <see cref="IViewModel"/> interface.</exception>
        IViewModel CreateViewModel(Type viewModelType, object dataContext);

        /// <summary>
        /// Creates a new view model.
        /// <para />
        /// This is a convenience wrapper around the <see cref="ViewModelFactory.CreateViewModel"/> method. This method cannot be overriden.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="dataContext">The data context.</param>
        /// <returns>The newly created <see cref="IViewModel"/> or <c>null</c> if no view model could be created.</returns>
        /// <exception cref="ArgumentException">The <c>TViewModel</c> does not implement the <see cref="IViewModel"/> interface.</exception>
        TViewModel CreateViewModel<TViewModel>(object dataContext)
            where TViewModel : IViewModel;
    }
}