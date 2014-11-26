// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIElementExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



#if !XAMARIN

namespace Catel.Windows
{
#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Extensions for <see cref="FrameworkElement"/>.
    /// </summary>
    public static partial class FrameworkElementExtensions
    {
        /// <summary>
        /// Fixes the blurriness in WPF by setting both <c>SnapsToDevicePixels</c> and
        /// <c>UseLayoutRounding</c> to <c>true</c>.
        /// </summary>
        /// <param name="element">The UI element.</param>
        public static void FixBlurriness(this FrameworkElement element)
        {
            Argument.IsNotNull(() => element);

#if NET
            element.SnapsToDevicePixels = true;
#endif

            element.UseLayoutRounding = true;
        }
    }
}

#endif