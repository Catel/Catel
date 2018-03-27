﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETFX_CORE

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
#else
    using System.Windows.Navigation;
    using RootFrameType = System.Windows.Controls.Frame;
#endif

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService
    {
        #region Fields
#if NET
        private bool _appClosingByMainWindow;
        private bool _appClosedFromService;
#else
        private long _canGoBackPropertyChangedCallbackToken;
        private long _canGoForwardPropertyChangedCallbackToken;
#endif
        private RootFrameType _frame;
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
        /// <summary>
        /// Gets the root frame.
        /// </summary>
        protected RootFrameType RootFrame
        {
            get
            {
                // Note: don't cache, it might change dynamically
                var rootFrame = NavigationRootService.GetNavigationRoot() as RootFrameType;

                if (!Equals(_frame, rootFrame))
                {
                    if (_frame != null)
                    {
#if NET
                        var canGoBackPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(RootFrameType.CanGoBackProperty, typeof(RootFrameType));
                        canGoBackPropertyDescriptor.RemoveValueChanged(_frame, CanGoBackChangedHandler);
                        var canGoForwardPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(RootFrameType.CanGoBackProperty, typeof(RootFrameType));
                        canGoForwardPropertyDescriptor.RemoveValueChanged(_frame, CanGoForwardChangedHandler);
#else
                        _frame.UnregisterPropertyChangedCallback(RootFrameType.CanGoBackProperty, _canGoBackPropertyChangedCallbackToken);
                        _frame.UnregisterPropertyChangedCallback(RootFrameType.CanGoForwardProperty, _canGoForwardPropertyChangedCallbackToken);
#endif
                        _frame.Navigating -= NavigatingEventHandler;
                        _frame.NavigationFailed -= NavigationFailedEventHandler;
                        _frame.Navigated -= NavigatedEventHandler;
                    }

                    _frame = rootFrame;

                    if (_frame != null)
                    {
#if NET
                        var propertyDescriptor = DependencyPropertyDescriptor.FromProperty(RootFrameType.CanGoBackProperty, typeof(RootFrameType));
                        propertyDescriptor.AddValueChanged(_frame, CanGoBackChangedHandler);
                        var canGoForwardPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(RootFrameType.CanGoBackProperty, typeof(RootFrameType));
                        canGoForwardPropertyDescriptor.AddValueChanged(_frame, CanGoForwardChangedHandler);
#else
                        _canGoBackPropertyChangedCallbackToken = _frame.RegisterPropertyChangedCallback(RootFrameType.CanGoBackProperty, (_, __) => OnCanGoBackChanged());
                        _canGoForwardPropertyChangedCallbackToken = _frame.RegisterPropertyChangedCallback(RootFrameType.CanGoForwardProperty, (_, __) => OnCanGoForwardChanged());
#endif
                        _frame.Navigating += NavigatingEventHandler;
                        _frame.NavigationFailed += NavigationFailedEventHandler;
                        _frame.Navigated += NavigatedEventHandler;
                    }
                }

                return rootFrame;

#if NET
                void CanGoBackChangedHandler(object sender, EventArgs e)
                {
                    OnCanGoBackChanged();
                }
                void CanGoForwardChangedHandler(object sender, EventArgs e)
                {
                    OnCanGoForwardChanged();
                }
#endif
                void NavigatingEventHandler(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
                {
                    OnNavigating();
                }
                void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs navigationFailedEventArgs)
                {
                    OnNavigationFailed();
                }
                void NavigatedEventHandler(object sender, NavigationEventArgs navigationEventArgs)
                {
                    OnNavigated();
                }
            }
        }

        partial void Initialize()
        {
#if NET
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
#if NET
            _appClosedFromService = true;

            var mainWindow = CatelEnvironment.MainWindow;
            if (mainWindow == null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("No main window found (not running SL out of browser? Cannot close application without a window.");
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
            throw Log.ErrorAndCreateException<NotSupportedInPlatformException>($"Direct navigations to urls is not supported in '{Platforms.CurrentPlatform}', cannot navigate to '{uri}'. Use Navigate(type) instead.");
#else
            RootFrame.Navigate(uri);
#endif
        }

        partial void NavigateWithParameters(string uri, Dictionary<string, object> parameters)
        {
            Log.Debug($"Navigating to '{uri}'");

#if NETFX_CORE
            var type = Reflection.TypeCache.GetType(uri, allowInitialization: false);
            var result = RootFrame.Navigate(type, parameters);
            if (result)
            {
                Log.Debug($"Navigated to '{uri}'");
            }
            else
            {
                Log.Error($"Failed to navigate to '{uri}'");
            }
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
#else
            return RootFrame.BackStack.Cast<object>().Count();
#endif
        }

        /// <summary>
        /// Removes the last back entry from the navigation history.
        /// </summary>
        public override void RemoveBackEntry()
        {
            Log.Debug("Removing last back entry");

#if NETFX_CORE
            var lastItem = RootFrame.BackStack.LastOrDefault();
            if (lastItem != null)
            {
                RootFrame.BackStack.Remove(lastItem);
            }
#else
            RootFrame.RemoveBackEntry();
#endif
        }

        /// <summary>
        /// Removes all the back entries from the navigation history.
        /// </summary>
        public override void RemoveAllBackEntries()
        {
            Log.Debug("Clearing all back entries");

#if NETFX_CORE
            RootFrame.BackStack.Clear();
#else
            while (RootFrame.RemoveBackEntry() != null)
            {
            }
#endif
        }

        #endregion
    }
}

#endif