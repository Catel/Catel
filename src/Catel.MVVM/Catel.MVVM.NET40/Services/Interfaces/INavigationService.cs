// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using Catel.MVVM;

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public interface INavigationService
    {
        #region Properties
        /// <summary>
        /// Gets a value indicating whether it is possible to navigate back.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it is possible to navigate back; otherwise, <c>false</c>.
        /// </value>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets a value indicating whether it is possible to navigate forward.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it is possible to navigate forward otherwise, <c>false</c>.
        /// </value>
        bool CanGoForward { get; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the application is about to be closed.
        /// </summary>
        event EventHandler<ApplicationClosingEventArgs> ApplicationClosing;

        /// <summary>
        /// Occurs when nothing has canceled the application closing and the application is really about to be closed. 
        /// </summary>
        event EventHandler<EventArgs> ApplicationClosed;
        #endregion

        #region Methods
        /// <summary>
        /// Closes the current application. The actual implementation depends on the final target framework.
        /// </summary>
        /// <returns><c>true</c> if the application is closed; otherwise <c>false</c>.</returns>
        bool CloseApplication();

        /// <summary>
        /// Navigates back to the previous page.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Navigates forward to the next page.
        /// </summary>
        void GoForward();

        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        /// <exception cref="ArgumentException">The <paramref name="uri"/> is <c>null</c> or whitespace.</exception>
        void Navigate(string uri, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <typeparam name="TViewModelType">The view model type.</typeparam>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        void Navigate<TViewModelType>(Dictionary<string, object> parameters = null);

        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        void Navigate(Type viewModelType, Dictionary<string, object> parameters = null);

        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        void Navigate(Uri uri);

        /// <summary>
        /// Registers the specified view model and the uri. Use this method to override the uri
        /// detection mechanism in Catel.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="uri">The URI to register.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement <see cref="IViewModel"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        void Register(Type viewModelType, Uri uri);

        /// <summary>
        /// Registers the specified view model and the uri. Use this method to override the uri
        /// detection mechanism in Catel.
        /// </summary>
        /// <param name="name">The name of the registered page.</param>
        /// <param name="uri">The URI to register.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="name"/> is already registered.</exception>
        void Register(string name, Uri uri);

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to unregister.</param>
        /// <returns>
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        bool Unregister(Type viewModelType);

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered page.</param>
        /// <returns>
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        bool Unregister(string name);

        /// <summary>
        /// Returns the number of total back entries (which is the navigation history).
        /// </summary>
        int GetBackStackCount();

        /// <summary>
        /// Removes the last back entry from the navigation history.
        /// </summary>
        void RemoveBackEntry();

        /// <summary>
        /// Removes all the back entries from the navigation history.
        /// </summary>
        void RemoveAllBackEntries();
        #endregion
    }
}
