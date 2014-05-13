// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using Windows.Threading;
    using Windows;

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
        /// <summary>
        /// Gets the parent of the specified element, both for Silverlight and WPF.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The parent <see cref="FrameworkElement"/> or <c>null</c> if there is no parent.</returns>
        public static FrameworkElement GetParent(this FrameworkElement element)
        {
            Argument.IsNotNull("element", element);

#if NET
            return (element.Parent ?? element.TemplatedParent) as FrameworkElement;
#else
            return element.Parent as FrameworkElement;
#endif
        }

        /// <summary>
        /// Finds a parent by predicate. It first tries to find the parent via the <c>UserControl.Parent</c> property, and if that
        /// doesn't satisfy, it uses the <c>UserControl.TemplatedParent</c> property.
        /// </summary>
        /// <param name="view">The control.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// 	<see cref="DependencyObject"/> or <c>null</c> if no parent is found that matches the predicate.
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
        /// 	<see cref="DependencyObject"/> or <c>null</c> if no parent is found that matches the predicate.
        /// </returns>
        public static DependencyObject FindParentByPredicate(this FrameworkElement view, Predicate<object> predicate, int maxDepth)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNull("predicate", predicate);

            object foundParent = null;

            var parents = new List<DependencyObject>();
            if (view.Parent != null)
            {
                parents.Add(view.Parent);
            }
#if NET
            if (view.TemplatedParent != null)
            {
                parents.Add(view.TemplatedParent);
            }
#endif

            var visualTreeParent = VisualTreeHelper.GetParent(view);
            if (visualTreeParent != null)
            {
                parents.Add(visualTreeParent);
            }

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