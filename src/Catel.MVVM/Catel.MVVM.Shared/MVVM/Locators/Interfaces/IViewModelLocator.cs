// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Resolver that will resolve view model types based on the view type. For example, if a view with the type
    /// name <c>MyAssembly.Views.PersonView</c> is inserted, this could result in the view model type
    /// <c>MyAssembly.ViewModels.PersonViewModel</c>.
    /// </summary>
    public interface IViewModelLocator : ILocator
    {
        /// <summary>
        /// Registers the specified view model in the local cache. This cache will also be used by the 
        /// <see cref="ResolveViewModel"/> method.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        void Register(Type viewType, Type viewModelType);

        /// <summary>
        /// Resolves a view model type by the view and the registered <see cref="ILocator.NamingConventions"/>.
        /// </summary>
        /// <param name="viewType">Type of the view to resolve the view model for.</param>
        /// <returns>The resolved view model or <c>null</c> if the view model could not be resolved.</returns>
        /// <remarks>
        /// Keep in mind that all results are cached. The cache itself is not automatically cleared when the
        /// <see cref="ILocator.NamingConventions"/> are changed. If the <see cref="ILocator.NamingConventions"/> are changed,
        /// the cache must be cleared manually.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        Type ResolveViewModel(Type viewType);
    }
}