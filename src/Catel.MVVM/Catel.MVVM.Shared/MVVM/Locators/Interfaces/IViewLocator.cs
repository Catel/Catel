// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Resolver that will resolve view types based on the view model type. For example, if a view model with the type
    /// name <c>MyAssembly.ViewModels.PersonViewModel</c> is inserted, this could result in the view type
    /// <c>MyAssembly.Views.PersonView</c>.
    /// </summary>
    public interface IViewLocator : ILocator
    {
        /// <summary>
        /// Registers the specified view in the local cache. This cache will also be used by the 
        /// <see cref="ResolveView"/> method.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        void Register(Type viewModelType, Type viewType);

        /// <summary>
        /// Resolves a view type by the view model and the registered <see cref="ILocator.NamingConventions"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to resolve the view for.</param>
        /// <returns>The resolved view or <c>null</c> if the view could not be resolved.</returns>
        /// <remarks>
        /// Keep in mind that all results are cached. The cache itself is not automatically cleared when the
        /// <see cref="ILocator.NamingConventions"/> are changed. If the <see cref="ILocator.NamingConventions"/> are changed,
        /// the cache must be cleared manually.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        Type ResolveView(Type viewModelType);
    }
}