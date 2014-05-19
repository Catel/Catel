// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5 || WINDOWS_PHONE || NETFX_CORE

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Windows;
    using Logging;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
#elif WINDOWS_PHONE
    using System.Windows.Navigation;
    using System.Net;
    using Microsoft.Phone.Controls;
#elif SILVERLIGHT
    using System.Windows.Navigation;
    using System.Windows.Browser;
#else
    using System.Windows.Navigation;
#endif

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService
    {
        #region Fields
        private static object _rootFrame;

#if NET || SL5
        private bool _appClosingByMainWindow;
        private bool _appClosedFromService;
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether it is possible to navigate back.
        /// </summary>
        /// <value>
        /// <c>true</c> if it is possible to navigate back; otherwise, <c>false</c>.
        /// </value>
        public override bool CanGoBack
        {
            get { return RootFrame.CanGoBack; }
        }

        /// <summary>
        /// Gets a value indicating whether it is possible to navigate forward.
        /// </summary>
        /// <value>
        /// <c>true</c> if it is possible to navigate backforward otherwise, <c>false</c>.
        /// </value>
        public override bool CanGoForward
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

        #region Methods
        partial void Initialize()
        {
#if NET || SL5
            var mainWindow = CatelEnvironment.MainWindow;
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
        }

        partial void CloseMainWindow()
        {
#if NET || SL5
            _appClosedFromService = true;

            var mainWindow = CatelEnvironment.MainWindow;
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
            Log.Error("Closing an application is not possible in '{0}'", Platforms.CurrentPlatform);
            throw new NotSupportedInPlatformException("Closing an application is not possible");
#endif
        }

        partial void NavigateBack()
        {
            if (CanGoBack)
            {
                RootFrame.GoBack();
            }
        }

        partial void NavigateForward()
        {
            if (CanGoForward)
            {
                RootFrame.GoForward();
            }
        }

        partial void NavigateToUri(Uri uri)
        {
#if NETFX_CORE
            var error = string.Format("Direct navigations to urls is not supported in '{0}', cannot navigate to '{1}'. Use Navigate(type) instead.", Platforms.CurrentPlatform, uri.ToString());
            Log.Error(error);
            throw new NotSupportedInPlatformException(error);
#else
            RootFrame.Navigate(uri);
#endif
        }

        partial void NavigateWithParameters(string uri, Dictionary<string, object> parameters)
        {
#if NETFX_CORE
            var type = Reflection.TypeCache.GetType(uri);
            RootFrame.Navigate(type, parameters);
#elif SILVERLIGHT || WINDOWS_PHONE
            string finalUri = string.Format("{0}{1}", uri, ToQueryString(parameters));
            Navigate(new Uri(finalUri, UriKind.RelativeOrAbsolute));
#else
            RootFrame.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute), parameters);
#endif
        }

        /// <summary>
        /// Resolves the navigation target.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The target to navigate to.</returns>
        protected override string ResolveNavigationTarget(Type viewModelType)
        {
            string navigationTarget = null;
            var dependencyResolver = this.GetDependencyResolver();

#if NETFX_CORE
            var viewLocator = dependencyResolver.Resolve<IViewLocator>();
            navigationTarget = viewLocator.ResolveView(viewModelType).AssemblyQualifiedName;
#else
            var urlLocator = dependencyResolver.Resolve<IUrlLocator>();
            navigationTarget = urlLocator.ResolveUrl(viewModelType);
#endif

            return navigationTarget;
        }

        /// <summary>
        /// Returns the number of total back entries (which is the navigation history).
        /// </summary>
        /// <returns>System.Int32.</returns>
        public override int GetBackStackCount()
        {
#if NETFX_CORE
            return RootFrame.BackStackDepth;
#elif WINDOWS_PHONE
            return RootFrame.BackStack.Cast<object>().Count();
#elif SILVERLIGHT
            throw new NotSupportedInPlatformException();
#else
            return RootFrame.BackStack.Cast<object>().Count();
#endif
        }

        /// <summary>
        /// Removes the last back entry from the navigation history.
        /// </summary>
        public override void RemoveBackEntry()
        {
#if NETFX_CORE && WINDOWS_PHONE
            var lastItem = RootFrame.BackStack.LastOrDefault();
            if (lastItem != null)
            {
                RootFrame.BackStack.Remove(lastItem);
            }
#elif NETFX_CORE
            throw new MustBeImplementedException();
#elif WINDOWS_PHONE
            RootFrame.RemoveBackEntry();
#elif SILVERLIGHT
            throw new NotSupportedInPlatformException();
#else
            RootFrame.RemoveBackEntry();
#endif
        }

        /// <summary>
        /// Removes all the back entries from the navigation history.
        /// </summary>
        public override void RemoveAllBackEntries()
        {
#if NETFX_CORE && WINDOWS_PHONE
            RootFrame.BackStack.Clear();
#elif NETFX_CORE 
            throw new MustBeImplementedException();
#elif WINDOWS_PHONE
            while (RootFrame.RemoveBackEntry() != null)
            {
            }
#elif SILVERLIGHT
            throw new NotSupportedInPlatformException();
#else
            while (RootFrame.RemoveBackEntry() != null)
            {
            }
#endif
        }

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
        #endregion
    }
}

#endif