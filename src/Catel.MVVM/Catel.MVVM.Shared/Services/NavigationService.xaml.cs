// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
    using RootFrameType = global::Windows.UI.Xaml.Controls.Frame;
#elif WINDOWS_PHONE
    using System.Windows.Navigation;
    using System.Net;
    using Microsoft.Phone.Controls;
    using RootFrameType = Microsoft.Phone.Controls.PhoneApplicationFrame;
#elif SILVERLIGHT
    using System.Windows.Navigation;
    using System.Windows.Browser;
    using RootFrameType = System.Windows.Controls.Frame;
#else
    using System.Windows.Navigation;
    using RootFrameType = System.Windows.Navigation.NavigationWindow;
#endif

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService
    {
        #region Fields
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
        #endregion

        #region Methods
        private RootFrameType RootFrame
        {
            get { return _navigationRootService.GetNavigationRoot() as RootFrameType; }
        }

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
#if NET
                var app = Application.Current;
                app.Shutdown();
#else
                mainWindow.Close();
#endif
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
            var error = $"Direct navigations to urls is not supported in '{Platforms.CurrentPlatform}', cannot navigate to '{uri}'. Use Navigate(type) instead.";
            Log.Error(error);
            throw new NotSupportedInPlatformException(error);
#else
            RootFrame.Navigate(uri);
#endif
        }

        partial void NavigateWithParameters(string uri, Dictionary<string, object> parameters)
        {
            Log.Debug($"Navigating to '{uri}'");

#if NETFX_CORE
            var type = Reflection.TypeCache.GetType(uri);
            var result = RootFrame.Navigate(type, parameters);
            if (result)
            {
                Log.Debug($"Navigated to '{uri}'");
            }
            else
            {
                Log.Error($"Failed to navigate to '{uri}'");
            }
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
#if NETFX_CORE && !WIN80
            var lastItem = RootFrame.BackStack.LastOrDefault();
            if (lastItem != null)
            {
                RootFrame.BackStack.Remove(lastItem);
            }
#elif NETFX_CORE // WIN80
            throw new NotSupportedInPlatformException();
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