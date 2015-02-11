// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.MVVM;
    using IoC;
    using Logging;

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService : NavigationServiceBase, INavigationService
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<string, string> _registeredUris = new Dictionary<string, string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService" /> class.
        /// </summary>
        public NavigationService()
        {
            Initialize();
        }
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
        partial void Initialize();
        partial void CloseMainWindow();
        partial void NavigateBack();
        partial void NavigateForward();
        partial void NavigateToUri(Uri uri);
        partial void NavigateWithParameters(string uri, Dictionary<string, object> parameters);

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
                Log.Info("Closing of application is canceled");
                return false;
            }

            CloseMainWindow();

#pragma warning disable 162
            ApplicationClosed.SafeInvoke(this);
            return true;
#pragma warning restore 162
        }

        /// <summary>
        /// Navigates back to the previous page.
        /// </summary>
        public virtual void GoBack()
        {
            if (CanGoBack)
            {
                NavigateBack();
            }
        }

        /// <summary>
        /// Navigates forward to the next page.
        /// </summary>
        public virtual void GoForward()
        {
            if (CanGoForward)
            {
                NavigateForward();
            }
        }

        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        /// <exception cref="ArgumentException">The <paramref name="uri"/> is <c>null</c> or whitespace.</exception>
        public virtual void Navigate(string uri, Dictionary<string, object> parameters = null)
        {
            Argument.IsNotNullOrWhitespace("uri", uri);

            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }

            NavigateWithParameters(uri, parameters);
        }

        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <typeparam name="TViewModelType">The view model type.</typeparam>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        public virtual void Navigate<TViewModelType>(Dictionary<string, object> parameters = null)
        {
            Navigate(typeof(TViewModelType), parameters);
        }

        /// <summary>
        /// Navigates the specified location registered using the view model type.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="parameters">Dictionary of parameters, where the key is the name of the parameter, 
        /// and the value is the value of the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        public virtual void Navigate(Type viewModelType, Dictionary<string, object> parameters = null)
        {
            Argument.IsNotNull("viewModelType", viewModelType);

            string viewModelTypeName = viewModelType.FullName;

            lock (_registeredUris)
            {
                if (!_registeredUris.ContainsKey(viewModelTypeName))
                {
                    var navigationTarget = ResolveNavigationTarget(viewModelType);
                    _registeredUris.Add(viewModelTypeName, navigationTarget);
                }

                Navigate(_registeredUris[viewModelTypeName], parameters);
            }
        }


        /// <summary>
        /// Navigates to a specific location.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        public virtual void Navigate(Uri uri)
        {
            Argument.IsNotNull("uri", uri);

            NavigateToUri(uri);
        }

        /// <summary>
        /// Registers the specified view model and the uri. Use this method to override the uri
        /// detection mechanism in Catel.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="uri">The URI to register.</param>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType"/> does not implement <see cref="IViewModel"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uri"/> is <c>null</c>.</exception>
        public virtual void Register(Type viewModelType, Uri uri)
        {
            Argument.ImplementsInterface("viewModelType", viewModelType, typeof(IViewModel));
            Argument.IsNotNull("uri", uri);

            Register(viewModelType.FullName, uri);
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
        public virtual void Register(string name, Uri uri)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("uri", uri);

            lock (_registeredUris)
            {
                if (_registeredUris.ContainsKey(name))
                {
                    throw new Exception(Catel.ResourceHelper.GetString("ViewModelAlreadyRegistered"));
                }

                _registeredUris.Add(name, uri.ToString());

                Log.Debug("Registered view model '{0}' in combination with '{1}' in the NavigationService", name, uri);
            }
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to unregister.</param>
        /// <returns>
        /// <c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Unregister(Type viewModelType)
        {
            return Unregister(viewModelType.FullName);
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered page.</param>
        /// <returns>
        /// <c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Unregister(string name)
        {
            lock (_registeredUris)
            {
                bool result = _registeredUris.Remove(name);
                if (result)
                {
                    Log.Debug("Unregistered view model '{0}' in NavigationService", name);
                }

                return result;
            }
        }
        #endregion
    }
}
