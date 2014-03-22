// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.visualtree.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
    using Catel.Windows;

#if XAMARIN
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

    public static partial class ViewExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Methods
        /// <summary>
        /// Ensures that a visual tree exists for the view.
        /// </summary>
        /// <param name="view">The view.</param>
        public static void EnsureVisualTree(this IView view)
        {
            Argument.IsNotNull("view", view);

            // According to the documentation, no visual tree is garantueed in the Loaded event of the user control.
            // However, as a solution the documentation says you need to manually call ApplyTemplate, so let's do that.
            // For more info, see http://msdn.microsoft.com/en-us/library/ms596558(vs.95)
            var targetControl = view as Control;
            if (targetControl != null)
            {
                (targetControl).ApplyTemplate();
            }
        }

        /// <summary>
        /// Finds the parent view model container.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The IViewModelContainer or <c>null</c> if the container is not found.</returns>
        public static IViewModelContainer FindParentViewModelContainer(this IView view)
        {
            return FindParentByPredicate(view, o => o is IViewModelContainer) as IViewModelContainer;
        }

#if !XAMARIN
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
#endif
        #endregion
    }
}