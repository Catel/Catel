// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows.Data
{
    using System;
    using System.Diagnostics;
#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Information about a dependency property.
    /// </summary>
    [DebuggerDisplay("{PropertyName}")]
    public class DependencyPropertyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyPropertyInfo"/> class.
        /// </summary>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyProperty"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public DependencyPropertyInfo(DependencyProperty dependencyProperty, string propertyName)
        {
            Argument.IsNotNull("dependencyProperty", dependencyProperty);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            DependencyProperty = dependencyProperty;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the dependency property.
        /// </summary>
        /// <value>The dependency property.</value>
        public DependencyProperty DependencyProperty { get; private set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }
    }
}