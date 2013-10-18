// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameworkElementLoadedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// EventArgs implementation for when a <see cref="FrameworkElement"/> is loaded.
    /// </summary>
    public class FrameworkElementLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElementLoadedEventArgs"/> class.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        public FrameworkElementLoadedEventArgs(FrameworkElement frameworkElement)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            FrameworkElement = frameworkElement;
        }

        /// <summary>
        /// Gets the framework element that has just been loaded.
        /// </summary>
        /// <value>The framework element.</value>
        public FrameworkElement FrameworkElement { get; private set; }
    }
}