// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationRootService.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE || UWP

namespace Catel.Services
{
#if UWP
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
#else
    using System.Windows;
    using System.Windows.Controls;
    using Windows;
#endif

    public partial class NavigationRootService
    {
        private object _rootFrame;

        /// <summary>
        /// Gets the navigation root.
        /// </summary>
        /// <returns>System.Object.</returns>
        public virtual object GetNavigationRoot()
        {
            return GetApplicationRootFrame();
        }

#if UWP
        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        protected virtual Frame GetApplicationRootFrame()
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
#else
        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        protected virtual Frame GetApplicationRootFrame()
        {
            if (_rootFrame == null)
            {
                var application = Application.Current;
                if (application != null)
                {
                    var mainWindow = application.MainWindow;
                    if (mainWindow != null)
                    {
                        _rootFrame = mainWindow.FindVisualDescendant(e => e is Frame) as Frame;
                    }
                }
            }

            return _rootFrame as Frame;
        }
#endif
    }
}

#endif
