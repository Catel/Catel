namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Logging;
    using RootFrameType = System.Windows.Controls.Frame;

    /// <summary>
    /// Service to navigate inside applications.
    /// </summary>
    public partial class NavigationService
    {
        private bool _appClosingByMainWindow;
        private bool _appClosedFromService;

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
        protected RootFrameType RootFrame
        {
            get
            {
                // Note: don't cache, it might change dynamically
                var rootFrame = NavigationRootService.GetNavigationRoot() as RootFrameType;
                if (rootFrame is null)
                {
                    throw Log.ErrorAndCreateException<CatelException>("No root frame is available for the navigation service");
                }

                return rootFrame;
            }
        }

        partial void Initialize()
        {
            var mainWindow = CatelEnvironment.MainWindow;
            if (mainWindow is not null)
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
        }

        private async Task CloseMainWindowAsync()
        {
            _appClosedFromService = true;

            var mainWindow = CatelEnvironment.MainWindow;
            if (mainWindow is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("No main window found (not running SL out of browser? Cannot close application without a window.");
            }

            if (!_appClosingByMainWindow)
            {
                var app = Application.Current;
                app.Shutdown();
            }
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
            RootFrame.Navigate(uri);
        }

        private async Task NavigateWithParametersAsync(string uri, Dictionary<string, object>? parameters)
        {
            Log.Debug($"Navigating to '{uri}'");

            RootFrame.Navigate(new Uri(uri, UriKind.RelativeOrAbsolute), parameters);
        }

        /// <summary>
        /// Resolves the navigation target.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The target to navigate to.</returns>
        protected override string? ResolveNavigationTarget(Type viewModelType)
        {
            var navigationTarget = UrlLocator.ResolveUrl(viewModelType);
            return navigationTarget;
        }

        /// <summary>
        /// Returns the number of total back entries (which is the navigation history).
        /// </summary>
        /// <returns>System.Int32.</returns>
        public override int GetBackStackCount()
        {
            return RootFrame.BackStack.Cast<object>().Count();
        }

        /// <summary>
        /// Removes the last back entry from the navigation history.
        /// </summary>
        public override void RemoveBackEntry()
        {
            Log.Debug("Removing last back entry");

            RootFrame.RemoveBackEntry();
        }

        /// <summary>
        /// Removes all the back entries from the navigation history.
        /// </summary>
        public override void RemoveAllBackEntries()
        {
            Log.Debug("Clearing all back entries");

            while (RootFrame.RemoveBackEntry() is not null)
            {
            }
        }
    }
}
