// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewLoadState.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Interface containing the load state of a view. This interface can be implemented by
    /// a view, but also by other helper classes wrapping a view.
    /// </summary>
    public interface IViewLoadState
    {
        /// <summary>
        /// Gets the view object.
        /// </summary>
        IView View { get; }

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        event EventHandler<EventArgs> Unloaded;
    }
}