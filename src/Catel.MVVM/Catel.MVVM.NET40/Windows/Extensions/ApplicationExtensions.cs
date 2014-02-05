// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !WINDOWS_PHONE

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="System.Windows.Application"/> class.
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Gets the currently active window of the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>
        /// The active window of the application or null in case of none window is opened.
        /// </returns>
        public static System.Windows.Window GetActiveWindow(this System.Windows.Application application)
        {
            return application.Dispatcher.Invoke(new Func<System.Windows.Window>(() =>
            {
                return GetActiveWindowForApplication(application); 
                
            })) as System.Windows.Window;
        }

        private static System.Windows.Window GetActiveWindowForApplication(this System.Windows.Application application)
        {
            System.Windows.Window activeWindow = null;

            if (application != null && application.Windows.Count > 0)
            {
                var windowList = new List<System.Windows.Window>(application.Windows.Cast<System.Windows.Window>());
                activeWindow = windowList.FirstOrDefault(cur => cur.IsActive);
                if (activeWindow == null && windowList.Count == 1 && windowList[0].Topmost)
                {
                    activeWindow = windowList[0];
                }
            }

            return activeWindow;
        }
    }
}

#endif
