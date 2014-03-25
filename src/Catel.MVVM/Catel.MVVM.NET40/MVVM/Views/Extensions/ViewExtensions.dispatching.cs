// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;

#if !XAMARIN
    using Catel.Windows.Threading;
#endif

#if XAMARIN

#elif NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

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

#if !XAMARIN
            var dependencyObject = (DependencyObject)view;
            dependencyObject.Dispatcher.Invoke(action);
#else
            throw new MustBeImplementedException();
#endif
        }
    }
}