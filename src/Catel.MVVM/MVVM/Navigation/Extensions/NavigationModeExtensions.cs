// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationModeExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS
namespace Catel.MVVM.Navigation
{
    using System;

#if NETFX_CORE
    using FrameworkNavigationMode = global::Windows.UI.Xaml.Navigation.NavigationMode;
#else
    using FrameworkNavigationMode = System.Windows.Navigation.NavigationMode;
#endif

    /// <summary>
    /// Extension methods for navigation mode.
    /// </summary>
    public static class NavigationModeExtensions
    {
        /// <summary>
        /// Converts the specified navigation mode.
        /// </summary>
        /// <param name="navigationMode">The navigation mode.</param>
        /// <returns>NavigationMode.</returns>
        public static NavigationMode Convert(this FrameworkNavigationMode navigationMode)
        {
            switch (navigationMode)
            {
                case FrameworkNavigationMode.New:
                    return NavigationMode.New;
                    
                case FrameworkNavigationMode.Back:
                    return NavigationMode.Back;

                case FrameworkNavigationMode.Forward:
                    return NavigationMode.Forward;

                case FrameworkNavigationMode.Refresh:
                    return NavigationMode.Refresh;

                default:
                    throw new ArgumentOutOfRangeException("navigationMode");
            }
        }
    }
}
#endif