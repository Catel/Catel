// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationRootService.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5 || WINDOWS_PHONE || NETFX_CORE

namespace Catel.Services
{
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
#elif WINDOWS_PHONE
    using System.Windows;
    using System.Windows.Navigation;
    using System.Net;
    using Microsoft.Phone.Controls;
#elif SILVERLIGHT
    using Catel.Windows;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Browser;
#else
    using System.Windows;
    using System.Windows.Navigation;
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

#if NETFX_CORE
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
#elif WINDOWS_PHONE
        /// <summary>
        /// Gets the application root frame.
        /// </summary>
        protected virtual Microsoft.Phone.Controls.PhoneApplicationFrame GetApplicationRootFrame()
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
        protected virtual System.Windows.Controls.Frame GetApplicationRootFrame()
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
        protected virtual NavigationWindow GetApplicationRootFrame()
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
    }
}

#endif