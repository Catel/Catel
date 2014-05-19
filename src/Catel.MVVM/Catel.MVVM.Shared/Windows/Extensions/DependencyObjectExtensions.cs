// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyObjectExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
#if NETFX_CORE
    using global::Windows.UI;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    using Catel.Windows.Data;

#endif

    /// <summary>
    /// Extension methods for the <see cref="DependencyObject"/> class.
    /// </summary>
    public static class DependencyObjectExtensions
    {
#if SILVERLIGHT
        /// <summary>
        /// Gets the binding expression for the specified dependency property.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <returns>
        /// The <see cref="BindingExpression"/> or <c>null</c> if the property is not bound.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyObject"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyProperty"/> is <c>null</c>.</exception>
        public static BindingExpression GetBindingExpression(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            Argument.IsNotNull("dependencyObject", dependencyObject);
            Argument.IsNotNull("dependencyProperty", dependencyProperty);

            return dependencyObject.ReadLocalValue(dependencyProperty) as BindingExpression;
        }
#endif

        /// <summary>
        /// Returns the ancestory object of a <see cref="DependencyObject"/>.
        /// </summary>
        /// <typeparam name="T">Ancestor object.</typeparam>
        /// <param name="visualObject">Visual object to get the ancestor object for.</param>
        /// <returns><see cref="DependencyObject"/> or null if no ancestor object is found.</returns>
        /// <remarks>
        ///		If visualObject was of type T it was returned as ancestor, this is changed.
        ///		GetAncestorObject wil not return supplied parameter anymore.
        /// </remarks>
        public static T GetAncestorObject<T>(this DependencyObject visualObject)
        {
            object ancestorObject = null;

            if (visualObject != null)
            {
                var parentVisualObject = visualObject.GetVisualParent();

                if (parentVisualObject is T)
                {
                    ancestorObject = parentVisualObject;
                }
                else
                {
                    ancestorObject = GetAncestorObject<T>(parentVisualObject);
                }
            }

            return (T)ancestorObject;
        }

        /// <summary>
        /// Finds the logical or visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindLogicalOrVisualAncestor(this DependencyObject startElement, Predicate<object> condition, int maxDepth = -1)
        {
#if NET
            // Try to find logical ancestor one level up
            object logicalAncestor = FindLogicalAncestor(startElement, condition, 1);
            if (logicalAncestor != null)
            {
                return logicalAncestor;
            }
#endif

            // Try to find visual ancestor one level up
            object visualAncestor = FindVisualAncestor(startElement, condition, 1);
            if (visualAncestor != null)
            {
                return visualAncestor;
            }

            // If we didn't find anything, try visual parent and call this method (recursive)
            var visualParent = startElement.GetVisualParent();
            if (visualParent != null)
            {
                object lastResortVisualAncestor = FindLogicalOrVisualAncestor(visualParent, condition);
                if (lastResortVisualAncestor != null)
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
            return (T)FindLogicalOrVisualAncestor(startElement, o => (o is T));
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
            DependencyObject obj = startElement;
            while ((obj != null) && !condition(obj))
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
        /// Finds the logical ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindLogicalAncestorByType<T>(this DependencyObject startElement)
        {
            return (T)FindLogicalAncestor(startElement, o => (o is T));
        }

        /// <summary>
        /// Finds the logical root.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static DependencyObject FindLogicalRoot(this DependencyObject startElement)
        {
            DependencyObject obj = null;
            while (startElement != null)
            {
                obj = startElement;
                startElement = startElement.GetLogicalParent();
            }

            return obj;
        }

        /// <summary>
        /// Gets the logical parent of the specified dependency object.
        /// </summary>
        /// <param name="element">The element to retrieve the parent from.</param>
        /// <returns>The parent or <c>null</c> if the parent could not be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        public static DependencyObject GetLogicalParent(this DependencyObject element)
        {
            Argument.IsNotNull("element", element);

#if NET
            return LogicalTreeHelper.GetParent(element);
#else
            return VisualTreeHelper.GetParent(element);
#endif
        }

        /// <summary>
        /// Gets the logical parent of the specified dependency object.
        /// </summary>
        /// <param name="element">The element to retrieve the parent from.</param>
        /// <returns>The parent or <c>null</c> if the parent could not be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        public static DependencyObject GetVisualParent(this DependencyObject element)
        {
            Argument.IsNotNull("element", element);

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
        /// Finds the visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindVisualAncestor(this DependencyObject startElement, Predicate<object> condition, int maxDepth = -1)
        {
            DependencyObject obj = startElement;
            while ((obj != null) && !condition(obj))
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
        /// Finds the visual ancestor by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindVisualAncestorByType<T>(this DependencyObject startElement)
        {
            return (T)FindVisualAncestor(startElement, o => (o is T));
        }

        /// <summary>
        /// Finds the visual descendant.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static DependencyObject FindVisualDescendant(this DependencyObject startElement, Predicate<object> condition)
        {
            if (startElement != null)
            {
                if (condition(startElement))
                {
                    return startElement;
                }

#if !WINDOWS_PHONE
                var startElementAsUserControl = startElement as UserControl;
                if (startElementAsUserControl != null)
                {
                    return FindVisualDescendant(startElementAsUserControl.Content as DependencyObject, condition);
                }
#endif

                var startElementAsContentControl = startElement as ContentControl;
                if (startElementAsContentControl != null)
                {
                    return FindVisualDescendant(startElementAsContentControl.Content as DependencyObject, condition);
                }

                var startElementAsBorder = startElement as Border;
                if (startElementAsBorder != null)
                {
                    return FindVisualDescendant(startElementAsBorder.Child as DependencyObject, condition);
                }

#if NET
                var startElementAsDecorator = startElement as Decorator;
                if (startElementAsDecorator != null)
                {
                    return FindVisualDescendant(startElementAsDecorator.Child as DependencyObject, condition);
                }
#endif

                // If the element has children, loop the children
                var children = new List<DependencyObject>();

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startElement); i++)
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
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the visual descendant by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static T FindVisualDescendantByType<T>(this DependencyObject startElement)
            where T : DependencyObject
        {
            return (T)FindVisualDescendant(startElement, o => (o is T));
        }

        /// <summary>
        /// Finds the visual root.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindVisualRoot(this DependencyObject startElement)
        {
            return FindVisualAncestor(startElement, delegate(object o)
            {
                var dependencyObject = o as DependencyObject;
                if (dependencyObject == null)
                {
                    return false;
                }

                return (dependencyObject.GetVisualParent() == null);
            });
        }

        /// <summary>
        /// Gets the visual children.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>
        /// 	<see cref="IEnumerable{DependencyObject}"/> of all children.
        /// </returns>
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; ++i)
            {
                yield return VisualTreeHelper.GetChild(parent, i);
            }
        }

        /// <summary>
        /// Finds a logical node in the tree of the specified <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name of the control to find.</param>
        /// <returns>Child as <see cref="DependencyObject"/> or <c>null</c> if the child cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyObject"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public static DependencyObject FindLogicalNode(this DependencyObject dependencyObject, string name)
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
        public static bool IsElementWithName(this DependencyObject dependencyObject, string name)
        {
            Argument.IsNotNull("dependencyObject", dependencyObject);
            Argument.IsNotNullOrWhitespace("name", name);

            var frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement != null)
            {
                return string.Equals((frameworkElement).Name, name);
            }

            return false;
        }

#if NET
        /// <summary>
        /// Updates all the bindings of the specified <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="recursive">if set to <c>true</c>, the bindings will be updated recursively using the <see cref="VisualTreeHelper"/>.</param>
        public static void UpdateAllBindings(this FrameworkElement element, bool recursive = false)
        {
            var bindingExpressions = GetBindingExpressions(element, recursive);
            foreach (var bindingExpression in bindingExpressions)
            {
                bindingExpression.UpdateSource();
            }
        }

        /// <summary>
        /// Gets all the binding expressions of the specified <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="recursive">if set to <c>true</c>, the bindings will be searched recursively using the <see cref="VisualTreeHelper"/>.</param>
        /// <returns>
        /// 	<see cref="IEnumerable{BindingExpression}"/> containing all bindings.
        /// </returns>
        /// <remarks>
        /// This code is originally found at http://stackoverflow.com/questions/3586870/retrieve-all-data-bindings-from-wpf-window.
        /// </remarks>
        public static IEnumerable<BindingExpression> GetBindingExpressions(this FrameworkElement element, bool recursive = false)
        {
            var bindings = new List<BindingExpression>();
            var dpList = new List<DependencyPropertyInfo>();

            //dpList.AddRange(GetDependencyProperties(element));
            //dpList.AddRange(GetAttachedProperties(element));

            dpList.AddRange(element.GetDependencyProperties());

            foreach (var dp in dpList)
            {
                var bindingExpression = BindingOperations.GetBindingExpression(element, dp.DependencyProperty);
                if (bindingExpression != null)
                {
                    bindings.Add(bindingExpression);
                }
            }

            if (recursive)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(element);
                if (childrenCount > 0)
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                        if (child != null)
                        {
                            bindings.AddRange(GetBindingExpressions(child, true));
                        }
                    }
                }
            }

            return bindings;
        }
#endif
    }
}

#endif