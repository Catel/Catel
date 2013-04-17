// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogicalTreeHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else

#endif

    /// <summary>
    /// Internal "replacement" for the <c>LogicalTreeHelper</c> as it can be found in WPF.
    /// </summary>
    public static class LogicalTreeHelper
    {
        /// <summary>
        /// Finds a logical node in the tree of the specified <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name of the control to find.</param>
        /// <returns>Child as <see cref="DependencyObject"/> or <c>null</c> if the child cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyObject"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public static DependencyObject FindLogicalNode(DependencyObject dependencyObject, string name)
        {
            Argument.IsNotNull("dependencyObject", dependencyObject);
            Argument.IsNotNullOrWhitespace("name", name);

            // Check if the object is the node itself
            if (IsElementWithName(dependencyObject, name))
            {
                return dependencyObject;
            }

            // Search all child nodes
            var children = new List<DependencyObject>(dependencyObject.GetVisualChildren());
            foreach (DependencyObject child in children)
            {
                if (IsElementWithName(child, name))
                {
                    return child;
                }
            }

            // Since we didn't find anything, check the childs of all childs
            return children.Select(child => FindLogicalNode(child, name)).FirstOrDefault(foundChild => foundChild != null);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DependencyObject"/> has the specified name.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name that the name of the <see cref="DependencyObject"/> should match.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="DependencyObject"/> has the specified name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyObject"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public static bool IsElementWithName(DependencyObject dependencyObject, string name)
        {
            Argument.IsNotNull("dependencyObject", dependencyObject);
            Argument.IsNotNullOrWhitespace("name", name);

            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement != null)
            {
                return (string.Compare((frameworkElement).Name, name) == 0);
            }

            return false;
        }
    }
}
