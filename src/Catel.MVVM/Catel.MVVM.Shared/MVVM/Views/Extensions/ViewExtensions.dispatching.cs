// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Extension methods for views.
    /// </summary>
    public static partial class ViewExtensions
    {
        /// <summary>
        /// Runs the specified action on the view dispatcher.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static void Dispatch(this IView view, Action action)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNull("action", action);

            FinalDispatch(view, action);
        }

        static partial void FinalDispatch(IView view, Action action);
    }
}