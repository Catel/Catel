// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUrlLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
    public interface IUrlLocator : ILocator
    {
        /// <summary>
        /// Registers the specified url in the local cache. This cache will also be used by the <see cref="ResolveUrl"/>
        /// method.
        /// </summary>
        /// <param name="viewModelType">The view model to resolve the url for.</param>
        /// <param name="url">The resolved url.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="url"/> is <c>null</c> or whitespace.</exception>
        void Register(Type viewModelType, string url);

        /// <summary>
        /// Resolves an url by the view model and the registered <see cref="ILocator.NamingConventions"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to resolve the url for.</param>
        /// <param name="ensurePageExists">if set to <c>true</c>, the method checks whether the page resource actually exists.</param>
        /// <returns>The resolved viurlew or <c>null</c> if the view could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Keep in mind that all results are cached. The cache itself is not automatically cleared when the
        /// <see cref="ILocator.NamingConventions"/> are changed. If the <see cref="ILocator.NamingConventions"/> are changed,
        /// the cache must be cleared manually.
        /// </remarks>
        string ResolveUrl(Type viewModelType, bool ensurePageExists = true);
    }
}