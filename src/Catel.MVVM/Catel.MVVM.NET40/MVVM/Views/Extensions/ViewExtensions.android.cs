// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using global::Android.App;
    using global::Android.OS;
    using global::Android.Views;

    public static partial class ViewExtensions
    {
        private static readonly Handler _handler = new Handler(Looper.MainLooper);

        /// <summary>
        /// Finds a parent by predicate. It first tries to find the parent via the <c>UserControl.Parent</c> property, and if that
        /// doesn't satisfy, it uses the <c>UserControl.TemplatedParent</c> property.
        /// </summary>
        /// <param name="view">The control.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// 	<see cref="IView"/> or <c>null</c> if no parent is found that matches the predicate.
        /// </returns>
        public static IView FindParentByPredicate(this IView view, Predicate<object> predicate)
        {
            return FindParentByPredicate(view, predicate, -1);
        }

        /// <summary>
        /// Finds a parent by predicate. It first tries to find the parent via the <c>UserControl.Parent</c> property, and if that
        /// doesn't satisfy, it uses the <c>UserControl.TemplatedParent</c> property.
        /// </summary>
        /// <param name="view">The control.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>
        /// 	<see cref="View"/> or <c>null</c> if no parent is found that matches the predicate.
        /// </returns>
        public static IView FindParentByPredicate(this IView view, Predicate<object> predicate, int maxDepth)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNull("predicate", predicate);

            object foundParent = null;

            var parents = new List<IView>();
            if (view.Parent != null)
            {
                var parentView = view.Parent as IView;
                if (parentView != null)
                {
                    parents.Add(parentView);
                }
            }

            foreach (var parent in parents)
            {
                foundParent = parent.FindLogicalOrVisualAncestor(predicate, maxDepth);
                if (foundParent != null)
                {
                    break;
                }
            }

            return foundParent as IView;
        }

        /// <summary>
        /// Finds the logical or visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindLogicalOrVisualAncestor(this IView startElement, Predicate<object> condition, int maxDepth = -1)
        {
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
        /// Finds the visual ancestor according to the predicate.
        /// </summary>
        /// <param name="startElement">The start element.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="maxDepth">The maximum number of levels to go up when searching for the parent. If smaller than 0, no maximum is used.</param>
        /// <returns>object or <c>null</c> if the ancestor is not found.</returns>
        public static object FindVisualAncestor(this IView startElement, Predicate<object> condition, int maxDepth = -1)
        {
            var obj = startElement;
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
        /// Gets the logical parent of the specified dependency object.
        /// </summary>
        /// <param name="element">The element to retrieve the parent from.</param>
        /// <returns>The parent or <c>null</c> if the parent could not be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        public static IView GetVisualParent(this IView element)
        {
            Argument.IsNotNull("element", element);

            try
            {
                return element.Parent as IView;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static partial void FinalDispatch(IView view, Action action)
        {
            _handler.Post(action);
        }
    }
}

#endif