// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLoadedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// EventArgs implementation for when a <see cref="IView"/> is loaded.
    /// </summary>
    public class ViewLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLoadedEventArgs"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public ViewLoadedEventArgs(IView view)
        {
            Argument.IsNotNull("view", view);

            View = view;
        }

        /// <summary>
        /// Gets the view that has just been loaded.
        /// </summary>
        /// <value>The view.</value>
        public IView View { get; private set; }
    }
}