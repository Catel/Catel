namespace Catel.Services
{
    using System.Windows;
    using System.Windows.Controls;
    using Windows;

    public partial class NavigationRootService
    {
        private object? _rootFrame;

        /// <summary>
        /// Gets the navigation root.
        /// </summary>
        /// <returns>System.Object.</returns>
        public virtual object? GetNavigationRoot()
        {
            return GetApplicationRootFrame();
        }

        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        protected virtual Frame? GetApplicationRootFrame()
        {
            if (_rootFrame is null)
            {
                var application = Application.Current;
                if (application is not null)
                {
                    var mainWindow = application.MainWindow;
                    if (mainWindow is not null)
                    {
                        _rootFrame = mainWindow.FindVisualDescendant(e => e is Frame) as Frame;
                    }
                }
            }

            return _rootFrame as Frame;
        }
    }
}
