#if NET || NETCORE || UWP

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Windows;
    using Logging;

#if UWP
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
#if NET || NETCORE
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
        /// <summary>
        /// Gets the root frame.
        /// </summary>
        protected RootFrameType RootFrame
        {
            get
            {
                // Note: don't cache, it might change dynamically
                var rootFrame = NavigationRootService.GetNavigationRoot() as RootFrameType;
                return rootFrame;
            }
        }

        partial void Initialize()
        {
#if NET || NETCORE
            var mainWindow = CatelEnvironment.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Closing += async (sender, e) =>
                {
                    // TODO: Always cancel and await somehow

                    if (!_appClosedFromService)
                    {
                        _appClosingByMainWindow = true;

                        if (!await CloseApplicationAsync())
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

        private async Task CloseMainWindowAsync()
        {
#if NET || NETCORE
            _appClosedFromService = true;

            var mainWindow = CatelEnvironment.MainWindow;
            if (mainWindow is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("No main window found (not running SL out of browser? Cannot close application without a window.");
            }

            if (!_appClosingByMainWindow)
            {
#if NET || NETCORE
                var app = Application.Current;
                app.Shutdown();
#else
                mainWindow.Close();
#endif
            }
#else
            Log.Error("Closing an application is not possible in '{0}'", Enum<SupportedPlatforms>.ToString(Platforms.CurrentPlatform));
            throw new NotSupportedInPlatformException("Closing an application is not possible");
#endif
        }

        private async Task NavigateBackAsync()
        {
            if (CanGoBack)
            {
                RootFrame.GoBack();
            }
        }

        private async Task NavigateForwardAsync()
        {
            if (CanGoForward)
            {
                RootFrame.GoForward();
            }
        }

        private async Task NavigateToUriAsync(Uri uri)
        {
#if NETFX_CORE
            throw Log.ErrorAndCreateException<NotSupportedInPlatformException>($"Direct navigations to urls is not supported in '{Enum<SupportedPlatforms>.ToString(Platforms.CurrentPlatform)}', cannot navigate to '{uri}'. Use Navigate(type) instead.");
#else
            RootFrame.Navigate(uri);
#endif
        }

        private async Task NavigateWithParametersAsync(string uri, Dictionary<string, object> parameters)
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
