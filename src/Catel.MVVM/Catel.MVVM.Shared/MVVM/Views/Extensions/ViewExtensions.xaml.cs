// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Data;
    using Windows.Threading;
    using Windows;
    using IoC;
    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
#endif

    public static partial class ViewExtensions
    {
        private static readonly HashSet<Type> _autoDetectedViewtypes = new HashSet<Type>(); 

        /// <summary>
        /// Automatically detects view properties to subscribe to by searching for dependency properties
        /// decorated with the <see cref="ViewToViewModelAttribute"/>.
        /// </summary>
        /// <param name="viewType">The view type.</param>
        public static void AutoDetectViewPropertiesToSubscribe(this Type viewType)
        {
            Argument.IsNotNull("viewType", viewType);

            lock (_autoDetectedViewtypes)
            {
                if (_autoDetectedViewtypes.Contains(viewType))
                {
                    return;
                }

                var serviceLocator = ServiceLocator.Default;
                var viewPropertySelector = serviceLocator.ResolveType<IViewPropertySelector>();

                var dependencyProperties = Catel.Windows.Data.DependencyPropertyHelper.GetDependencyProperties(viewType);
                foreach (var dependencyProperty in dependencyProperties)
                {
                    var propertyInfo = viewType.GetPropertyEx(dependencyProperty.PropertyName);
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.IsDecoratedWithAttribute<ViewToViewModelAttribute>())
                        {
                            viewPropertySelector.AddPropertyToSubscribe(dependencyProperty.PropertyName, viewType);
                        }
                    }
                }

                _autoDetectedViewtypes.Add(viewType);
            }
        }

        /// <summary>
        /// Gets the parent of the specified element, both for Silverlight and WPF.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The parent <see cref="FrameworkElement"/> or <c>null</c> if there is no parent.</returns>
        public static FrameworkElement GetParent(this FrameworkElement element)
        {
            Argument.IsNotNull("element", element);

            return GetPossibleParents(element).FirstOrDefault();
        }

        /// <summary>
        /// Gets the possible parents of the specified element, both for Silverlight and WPF.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The possible parents <see cref="FrameworkElement"/> or <c>null</c> if there is no parent.</returns>
        public static FrameworkElement[] GetPossibleParents(this FrameworkElement element)
        {
            Argument.IsNotNull("element", element);

            var parents = new List<FrameworkElement>();

            var elementParent = element.Parent as FrameworkElement;
            if (elementParent != null)
            {
                parents.Add(elementParent);
            }

#if NET
            var templatedParent = element.TemplatedParent as FrameworkElement;
            if (templatedParent != null)
            {
                parents.Add(templatedParent);
            }
#endif

            var visualTreeParent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (visualTreeParent != null)
            {
                parents.Add(visualTreeParent);
            }

            return parents.ToArray();
        }

        /// <summary>
        /// Finds a parent by predicate. It first tries to find the parent via the <c>UserControl.Parent</c> property, and if that
        /// doesn't satisfy, it uses the <c>UserControl.TemplatedParent</c> property.
        /// </summary>
        /// <param name="view">The control.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// <see cref="DependencyObject"/> or <c>null</c> if no parent is found that matches the predicate.
        /// </returns>
        public static DependencyObject FindParentByPredicate(this IView view, Predicate<object> predicate)
        {
            return FindParentByPredicate((FrameworkElement)view, predicate, -1);
        }

        /// <summary>
        /// Finds a parent by predicate. It first tries to find the parent via the <c>UserControl.Parent</c> property, and if that
        /// doesn't satisfy, it uses the <c>UserControl.TemplatedParent</c> property.
        /// </summary>
        /// <param name="view">The control.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>
        /// <see cref="DependencyObject"/> or <c>null</c> if no parent is found that matches the predicate.
        /// </returns>
        public static DependencyObject FindParentByPredicate(this FrameworkElement view, Predicate<object> predicate, int maxDepth)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNull("predicate", predicate);

            object foundParent = null;

            var parents = GetPossibleParents(view);
            foreach (var parent in parents)
            {
                foundParent = parent.FindLogicalOrVisualAncestor(predicate, maxDepth);
                if (foundParent != null)
                {
                    break;
                }
            }

            return foundParent as DependencyObject;
        }

        static partial void FinalDispatch(IView view, Action action)
        {
            var dependencyObject = (DependencyObject)view;

            var dispatcher = dependencyObject.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }
    }
}

#endif