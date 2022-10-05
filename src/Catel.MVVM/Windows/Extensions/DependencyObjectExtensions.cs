namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Extension methods for the <see cref="DependencyObject"/> class.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Finds the logical or visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindLogicalOrVisualAncestor(this DependencyObject startElement, Predicate<object> condition, int maxDepth = -1)
        {
            // Try to be super fast, simple mode (just 1 level)

            // Try to find logical ancestor one level up
            var logicalAncestor = FindLogicalAncestor(startElement, condition, 1);
            if (logicalAncestor is not null)
            {
                return logicalAncestor;
            }

            // Try to find visual ancestor one level up
            var visualAncestor = FindVisualAncestor(startElement, condition, 1);
            if (visualAncestor is not null)
            {
                return visualAncestor;
            }

            // Go into "expensive mode" (search all levels)

            // Try to find logical ancestor at any level
            logicalAncestor = FindLogicalAncestor(startElement, condition, maxDepth);
            if (logicalAncestor is not null)
            {
                return logicalAncestor;
            }

            // Try to find visual ancestor at any level
            visualAncestor = FindVisualAncestor(startElement, condition, maxDepth);
            if (visualAncestor is not null)
            {
                return visualAncestor;
            }

            // If we didn't find anything, try visual parent and call this method (recursive)
            var visualParent = startElement.GetVisualParent();
            if (visualParent is not null)
            {
                var lastResortVisualAncestor = FindLogicalOrVisualAncestor(visualParent, condition, maxDepth > 0 ? maxDepth - 1 : -1);
                if (lastResortVisualAncestor is not null)
                {
                    return lastResortVisualAncestor;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the logical or visual ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindLogicalOrVisualAncestorByType<T>(this DependencyObject startElement)
        {
            return FindLogicalOrVisualAncestorByType<T>(startElement, -1);
        }

        /// <summary>
        /// Finds the logical or visual ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindLogicalOrVisualAncestorByType<T>(this DependencyObject startElement, int maxDepth)
        {
            return (T)FindLogicalOrVisualAncestor(startElement, o => o is T, maxDepth);
        }

        /// <summary>
        /// Finds the logical ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindLogicalAncestor(this DependencyObject startElement, Predicate<object> condition, int maxDepth = -1)
        {
            var obj = startElement;
            while ((obj is not null) && !condition(obj))
            {
                if (maxDepth == 0)
                {
                    return null;
                }

                if (maxDepth > 0)
                {
                    maxDepth--;
                }

                obj = obj.GetLogicalParent();
            }

            return obj;
        }

        /// <summary>
        /// Finds the visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindVisualAncestor(this DependencyObject startElement, Predicate<object> condition, int maxDepth = -1)
        {
            var obj = startElement;
            while ((obj is not null) && !condition(obj))
            {
                if (maxDepth == 0)
                {
                    return null;
                }
                else if (maxDepth > 0)
                {
                    maxDepth--;
                }

                obj = obj.GetVisualParent();
            }

            return obj;
        }

        /// <summary>
        /// Finds the logical ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindLogicalAncestorByType<T>(this DependencyObject startElement)
        {
            return FindLogicalAncestorByType<T>(startElement, -1);
        }

        /// <summary>
        /// Finds the logical ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindLogicalAncestorByType<T>(this DependencyObject startElement, int maxDepth)
        {
            return (T)FindLogicalAncestor(startElement, o => o is T, maxDepth);
        }

        /// <summary>
        /// Finds the visual ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindVisualAncestorByType<T>(this DependencyObject startElement)
        {
            return FindVisualAncestorByType<T>(startElement, -1);
        }

        /// <summary>
        /// Finds the visual ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindVisualAncestorByType<T>(this DependencyObject startElement, int maxDepth)
        {
            return (T)FindVisualAncestor(startElement, o => o is T, maxDepth);
        }

        /// <summary>
        /// Finds the logical root.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static DependencyObject FindLogicalRoot(this DependencyObject startElement)
        {
            var obj = startElement;
            while (startElement is not null)
            {
                obj = startElement;
                startElement = startElement.GetLogicalParent();
            }

            return obj;
        }

        /// <summary>
        /// Finds the visual root.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindVisualRoot(this DependencyObject startElement)
        {
            return FindVisualAncestor(startElement, delegate (object o)
            {
                var dependencyObject = o as DependencyObject;
                if (dependencyObject is null)
                {
                    return false;
                }

                return (dependencyObject.GetVisualParent() is null);
            });
        }

        /// <summary>
        /// Gets the logical parent of the specified dependency object.
        /// </summary>
        /// <param name="element">The element to retrieve the parent from.</param>
        /// <returns>The parent or <c>null</c> if the parent could not be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        public static DependencyObject GetLogicalParent(this DependencyObject element)
        {
            ArgumentNullException.ThrowIfNull(element);

            try
            {
                return LogicalTreeHelper.GetParent(element);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the logical parent of the specified dependency object.
        /// </summary>
        /// <param name="element">The element to retrieve the parent from.</param>
        /// <returns>The parent or <c>null</c> if the parent could not be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        public static DependencyObject GetVisualParent(this DependencyObject element)
        {
            ArgumentNullException.ThrowIfNull(element);

            try
            {
                return VisualTreeHelper.GetParent(element);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the visual descendant.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static DependencyObject FindVisualDescendant(this DependencyObject startElement, Predicate<object> condition)
        {
            if (startElement is not null)
            {
                if (condition(startElement))
                {
                    return startElement;
                }

                var startElementAsUserControl = startElement as UserControl;
                if (startElementAsUserControl is not null)
                {
                    return FindVisualDescendant(startElementAsUserControl.Content as DependencyObject, condition);
                }

                var startElementAsContentControl = startElement as ContentControl;
                if (startElementAsContentControl is not null)
                {
                    return FindVisualDescendant(startElementAsContentControl.Content as DependencyObject, condition);
                }

                var startElementAsBorder = startElement as Border;
                if (startElementAsBorder is not null)
                {
                    return FindVisualDescendant(startElementAsBorder.Child, condition);
                }

                var startElementAsDecorator = startElement as Decorator;
                if (startElementAsDecorator is not null)
                {
                    return FindVisualDescendant(startElementAsDecorator.Child, condition);
                }

                // If the element has children, loop the children
                var children = new List<DependencyObject>();

                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(startElement); i++)
                {
                    children.Add(VisualTreeHelper.GetChild(startElement, i));
                }

                // First, loop children itself
                foreach (var child in children)
                {
                    if (condition(child))
                    {
                        return child;
                    }
                }

                // Direct child is not what we are looking for, continue
                foreach (var child in children)
                {
                    var obj = FindVisualDescendant(child, condition);
                    if (obj is not null)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the visual descendant by name.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="name">The name of the element to search for.</param>
        /// <returns>object or <c>null</c> if the descendant is not found.</returns>
        public static DependencyObject FindVisualDescendantByName(this DependencyObject startElement, string name)
        {
            return FindVisualDescendant(startElement, o => (o is FrameworkElement) && string.Equals(((FrameworkElement)o).Name, name));
        }

        /// <summary>
        /// Finds the visual descendant by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the descendant is not found.</returns>
        public static T FindVisualDescendantByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindVisualDescendant(startElement, o => (o is T));
        }

        /// <summary>
        /// Gets the direct children from the visual tree.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>
        /// 	<see cref="IEnumerable{DependencyObject}"/> of all children.
        /// </returns>
        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject parent)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; ++i)
            {
                yield return VisualTreeHelper.GetChild(parent, i);
            }
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
        public static bool IsElementWithName(this DependencyObject dependencyObject, string name)
        {
            Argument.IsNotNull("dependencyObject", dependencyObject);
            Argument.IsNotNullOrWhitespace("name", name);

            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement is not null)
            {
                return string.Equals(frameworkElement.Name, name);
            }

            return false;
        }
    }
}
