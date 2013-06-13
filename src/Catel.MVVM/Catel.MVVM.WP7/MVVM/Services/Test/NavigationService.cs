// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services.Test
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Test implementation of the <see cref="INavigationService"/>.
    /// <para />
    /// This class is a dummy implementation, no actual code is used.
    /// </summary>
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public NavigationService()
        {
        }

        #region Properties
        /// <summary>
        /// Gets a value indicating whether it is possible to navigate back.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it is possible to navigate back; otherwise, <c>false</c>.
        /// </value>
        public bool CanGoBack { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether it is possible to navigate forward.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it is possible to navigate backforward otherwise, <c>false</c>.
        /// </value>
        public bool CanGoForward { get { return false; } }

        /// <summary>
        /// Gets the last navigation URI.
        /// </summary>
        /// <value>The last navigation URI.</value>
        public string LastNavigationUri { get; private set; }

        /// <summary>
        /// Gets the last navigation parameters.
        /// </summary>
        /// <value>The last navigation parameters.</value>
        public Dictionary<string, object> LastNavigationParameters { get; private set; } 
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the application is about to be closed.
        /// </summary>
        public event EventHandler<ApplicationClosingEventArgs> ApplicationClosing;

        /// <summary>
        /// Occurs when nothing has canceled the application closing and the application is really about to be closed.
        /// </summary>
        public event EventHandler<EventArgs> ApplicationClosed;
        #endregion

        #region Methods
        /// <summary>
        /// Closes the current application. The actual implementation depends on the final target framework.
        /// </summary>
        /// <returns><c>true</c> if the application is closed; otherwise <c>false</c>.</returns>
        public bool CloseApplication()
        {
            var eventArgs = new ApplicationClosingEventArgs();
            ApplicationClosing.SafeInvoke(this, eventArgs);
            if (eventArgs.Cancel)
            {
                return false;
            }

            ApplicationClosed.SafeInvoke(this);
            return true;
        }

        /// <summary>
        /// Navigates back to the previous page.
        /// </summary>
        public void GoBack()
        {
            UpdateLastNavigationInfo("back");
        }

        /// <summary>
        /// Navigates forward to the next page.
        /// </summary>
        public void GoForward()
        {
            UpdateLastNavigationInfo("forward");
        }

        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter,
        /// and the value is the value of the parameter.</param>
        public void Navigate(string uri, Dictionary<string, object> parameters = null)
        {
            UpdateLastNavigationInfo("uri", parameters);
        }

        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <typeparam name="TViewModelType">The view model type.</typeparam>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter,
        /// and the value is the value of the parameter.</param>
        public void Navigate<TViewModelType>(Dictionary<string, object> parameters = null)
        {
            Navigate(typeof(TViewModelType), parameters);
        }

        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter,
        /// and the value is the value of the parameter.</param>
        public void Navigate(Type viewModelType, Dictionary<string, object> parameters = null)
        {
            UpdateLastNavigationInfo(viewModelType.FullName, parameters);
        }

        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        public void Navigate(Uri uri)
        {
            UpdateLastNavigationInfo("uri");
        }

        /// <summary>
        /// Registers the specified view model and the uri. Use this method to override the uri
        /// detection mechanism in Catel.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="uri">The URI to register.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement <see cref="IViewModel"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        public void Register(Type viewModelType, Uri uri)
        {
        }

        /// <summary>
        /// Registers the specified view model and the uri. Use this method to override the uri
        /// detection mechanism in Catel.
        /// </summary>
        /// <param name="name">The name of the registered page.</param>
        /// <param name="uri">The URI to register.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The <paramref name="name"/> is already registered.</exception>
        public void Register(string name, Uri uri)
        {
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to unregister.</param>
        /// <returns>
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public bool Unregister(Type viewModelType)
        {
            return Unregister(viewModelType.FullName);
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered page.</param>
        /// <returns>
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public bool Unregister(string name)
        {
            return true;
        }

        /// <summary>
        /// Returns the number of total back entries (which is the navigation history).
        /// </summary>
        public int GetBackStackCount()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the last back entry from the navigation history.
        /// </summary>
        public void RemoveBackEntry()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all the back entries from the navigation history.
        /// </summary>
        public void RemoveAllBackEntries()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves the view by naming conventions.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        public string ResolveViewByNamingConventions(Type viewModelType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears the last navigation info.
        /// </summary>
        public void ClearLastNavigationInfo()
        {
            LastNavigationUri = null;
            LastNavigationParameters = null;
        }

        /// <summary>
        /// Updates the last navigation info.
        /// </summary>
        /// <param name="uri">The URI.</param>
        private void UpdateLastNavigationInfo(Uri uri)
        {
            UpdateLastNavigationInfo(uri, null);
        }

        /// <summary>
        /// Updates the last navigation info.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="parameters">The parameters.</param>
        private void UpdateLastNavigationInfo(Uri uri, Dictionary<string, object> parameters)
        {
            UpdateLastNavigationInfo(uri.ToString(), parameters);
        }

        /// <summary>
        /// Updates the last navigation info.
        /// </summary>
        /// <param name="uri">The URI.</param>
        private void UpdateLastNavigationInfo(string uri)
        {
            UpdateLastNavigationInfo(uri, null);
        }

        /// <summary>
        /// Updates the last navigation info.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="parameters">The parameters.</param>
        private void UpdateLastNavigationInfo(string uri, Dictionary<string, object> parameters)
        {
            LastNavigationUri = uri;

            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }

            LastNavigationParameters = parameters;
        }
        #endregion
    }
}
