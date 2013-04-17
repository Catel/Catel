// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using IoC;
    using Logging;
    using Windows;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
#elif WINDOWS_PHONE
    using System.Windows.Navigation;
    using System.Net;
    using Phone.Controls;
#elif SILVERLIGHT
    using System.Windows.Navigation;
    using System.Windows.Browser;
#else
    using System.Windows.Navigation;
#endif

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public class NavigationService : ViewModelServiceBase, INavigationService
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static object _rootFrame;

        private static readonly Dictionary<string, string> _registeredUris = new Dictionary<string, string>();

#if NET || SL4 || SL5
        private bool _appClosingByMainWindow;
        private bool _appClosedFromService;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        public NavigationService()
        {
#if NET || SL4 || SL5
            var mainWindow = Catel.Environment.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Closing += (sender, e) =>
                {
                    if (!_appClosedFromService)
                    {
                        _appClosingByMainWindow = true;

                        if (!CloseApplication())
                        {
                            Log.Debug("INavigationService.CloseApplication has canceled the closing of the main window");
                            e.Cancel = true;
                        }

                        _appClosingByMainWindow = false;
                    }
                };
            }
            else
            {
                Log.Warning("Application.Current.MainWindow is null, cannot prevent application closing via service");
            }
#endif

            if (RootFrame != null)
            {
                RootFrame.Navigating += OnRootFrameNavigating;
                RootFrame.Navigated += OnRootFrameNavigated;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether it is possible to navigate back.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it is possible to navigate back; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanGoBack
        {
            get { return RootFrame.CanGoBack; }
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to navigate forward.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it is possible to navigate backforward otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanGoForward
        {
            get { return RootFrame.CanGoForward; }
        }

        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>The root frame.</value>
#if NETFX_CORE
        private Frame RootFrame
#elif WINDOWS_PHONE
        private Microsoft.Phone.Controls.PhoneApplicationFrame RootFrame
#elif SILVERLIGHT
        private System.Windows.Controls.Frame RootFrame
#else
        private NavigationWindow RootFrame
#endif
        {
            get { return GetApplicationRootFrame(); }
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
        /// <summary>
        /// Called when the root frame is about to navigate to a new uri.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnRootFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            
        }

        /// <summary>
        /// Called when the root frame has just navigated to a new uri.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs" /> instance containing the event data.</param>
        protected virtual void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            
        }

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

#if NET || SL4 || SL5
            _appClosedFromService = true;

            var mainWindow = Catel.Environment.MainWindow;
            if (mainWindow == null)
            {
                const string error = "No main window found (not running SL out of browser? Cannot close application without a window.";
                Log.Error(error);
                throw new NotSupportedException(error);
            }

            if (!_appClosingByMainWindow)
            {
                mainWindow.Close();
            }
#else
            Log.Error("Closing an application is not possible in Windows Phone 7");
            throw new NotSupportedException("Closing an application is not possible in Windows Phone and WinRT");
#endif

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
                RootFrame.GoBack();
            }
        }

        /// <summary>
        /// Navigates forward to the next page.
        /// </summary>
        public virtual void GoForward()
        {
            if (CanGoForward)
            {
                RootFrame.GoForward();
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

#if NETFX_CORE
            var type = Reflection.TypeCache.GetType(uri);
            RootFrame.Navigate(type, parameters);
#elif SILVERLIGHT
            string finalUri = string.Format("{0}{1}", uri, ToQueryString(parameters));
            Navigate(new Uri(finalUri, UriKind.RelativeOrAbsolute));
#else
            RootFrame.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute), parameters);
#endif
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

            // First try the registered uri (which should always override page registrations)
            lock (_registeredUris)
            {
                if (!_registeredUris.ContainsKey(viewModelTypeName))
                {
#if NETFX_CORE
                    var viewLocator = ServiceLocator.Default.ResolveType<IViewLocator>();
                    var url = viewLocator.ResolveView(viewModelType).AssemblyQualifiedName;
#else
                    var urlLocator = ServiceLocator.Default.ResolveType<IUrlLocator>();
                    var url = urlLocator.ResolveUrl(viewModelType);
#endif

                    _registeredUris.Add(viewModelTypeName, url);
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

#if NETFX_CORE
            var error = string.Format("Direct navigations to urls is not supported in WinRT, cannot navigate to '{0}'. Use Navigate(type) instead.", uri.ToString());
            Log.Error(error);
            throw new InvalidOperationException(error);
#else
            RootFrame.Navigate(uri);
#endif
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
                    throw new Exception(Catel.ResourceHelper.GetString(typeof(NavigationService), "Exceptions", "ViewModelAlreadyRegistered"));
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
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
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
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
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

        /// <summary>
        /// Returns the number of total back entries (which is the navigation history).
        /// </summary>
        public virtual int GetBackStackCount()
        {
#if NETFX_CORE
            throw new NotImplementedException();
#elif SILVERLIGHT
            throw new NotImplementedException();
#else
            return RootFrame.BackStack.Cast<object>().Count();
#endif
        }

        /// <summary>
        /// Removes the last back entry from the navigation history.
        /// </summary>
        public virtual void RemoveBackEntry()
        {
#if NETFX_CORE
            throw new NotImplementedException();
#elif SILVERLIGHT
            throw new NotImplementedException();
#else
            RootFrame.RemoveBackEntry();
#endif
        }

        /// <summary>
        /// Removes all the back entries from the navigation history.
        /// </summary>
        public virtual void RemoveAllBackEntries()
        {
#if NETFX_CORE
            throw new NotImplementedException();
#elif SILVERLIGHT
            throw new NotImplementedException();
#else
            while (RootFrame.RemoveBackEntry() != null)
            {   
            }
#endif
        }

#if SILVERLIGHT
        /// <summary>
        /// Converts a dictionary to query string parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>String containing the paramets as query string. <c>null</c> values will be removed.</returns>
        /// <remarks>
        /// This method uses the <see cref="Object.ToString"/> method to convert values to a parameter value. Make sure
        /// that the objects passed correctly support this.
        /// </remarks>
        private static string ToQueryString(Dictionary<string, object> parameters)
        {
            string url = string.Empty;

            foreach (var parameter in parameters)
            {
                if (parameter.Value != null)
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        url = "?";
                    }
                    else
                    {
                        url += "&";
                    }

                    url += string.Format("{0}={1}", HttpUtility.UrlEncode(parameter.Key), HttpUtility.UrlEncode(parameter.Value.ToString()));
                }
            }

            return url;
        }
#endif

#if NETFX_CORE
        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        private Frame GetApplicationRootFrame()
        {
            if (_rootFrame == null)
            {
                if (Window.Current != null)
                {
                    _rootFrame = Window.Current.Content as Frame;
                }
            }

            return _rootFrame as Frame;
        }
#elif WINDOWS_PHONE
        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        private Microsoft.Phone.Controls.PhoneApplicationFrame GetApplicationRootFrame()
        {
            if (_rootFrame == null)
            {
                _rootFrame = Application.Current.RootVisual as Microsoft.Phone.Controls.PhoneApplicationFrame;
            }

            return _rootFrame as Microsoft.Phone.Controls.PhoneApplicationFrame;
        }
#elif SILVERLIGHT
        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        private System.Windows.Controls.Frame GetApplicationRootFrame()
        {
            if (_rootFrame == null)
            {
                if (Application.Current != null)
                {
                    if (Application.Current.RootVisual != null)
                    {
                        _rootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is System.Windows.Controls.Frame) as System.Windows.Controls.Frame;
                    }
                }
            }

            return _rootFrame as System.Windows.Controls.Frame;
        }
#else
        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        private NavigationWindow GetApplicationRootFrame()
        {
            if (_rootFrame == null)
            {
                if (Application.Current != null)
                {
                    _rootFrame = Application.Current.MainWindow as NavigationWindow;
                }
            }

            return _rootFrame as NavigationWindow;
        }
#endif

        /// <summary>
        /// Ensures that the page type is correct for the target framework.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        /// <exception cref="ArgumentException">If <paramref name="pageType"/> is not of the right type.</exception>
        private static void EnsurePageTypeIsCorrect(Type pageType)
        {
#if NETFX_CORE
            Argument.IsOfType("pageType", pageType, typeof(Page));
#elif WINDOWS_PHONE
            Argument.IsOfType("pageType", pageType, typeof(PhoneApplicationPage));
#elif SILVERLIGHT
            Argument.IsOfType("pageType", pageType, typeof(System.Windows.Controls.Page));
#else
            Argument.IsOfType("pageType", pageType, typeof(System.Windows.Controls.Page));
#endif
        }
        #endregion
    }
}
