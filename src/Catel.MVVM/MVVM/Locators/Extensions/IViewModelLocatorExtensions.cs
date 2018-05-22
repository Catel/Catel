// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModelLocatorExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="IViewModelLocator"/>.
    /// </summary>
    public static class IViewModelLocatorExtensions
    {
        /// <summary>
        /// Registers the specified view model in the local cache. This cache will also be used by the
        /// <see cref="ResolveViewModel{TView}" /> method.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModelLocator">The view model locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelLocator" /> is <c>null</c>.</exception>
        public static void Register<TView, TViewModel>(this IViewModelLocator viewModelLocator)
        {
            Argument.IsNotNull("viewModelLocator", viewModelLocator);

            viewModelLocator.Register(typeof(TView), typeof(TViewModel));
        }

        /// <summary>
        /// Resolves a view model type by the view and the registered <see cref="ILocator.NamingConventions" />.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="viewModelLocator">The view model locator.</param>
        /// <returns>The resolved view model or <c>null</c> if the view model could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelLocator" /> is <c>null</c>.</exception>
        /// <remarks>Keep in mind that all results are cached. The cache itself is not automatically cleared when the
        /// <see cref="ILocator.NamingConventions" /> are changed. If the <see cref="ILocator.NamingConventions" /> are changed,
        /// the cache must be cleared manually.</remarks>
        public static Type ResolveViewModel<TView>(this IViewModelLocator viewModelLocator)
        {
            Argument.IsNotNull("viewModelLocator", viewModelLocator);

            return viewModelLocator.ResolveViewModel(typeof(TView));
        }
    }
}