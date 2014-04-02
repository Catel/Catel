// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INavigationView.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;
    using Catel.MVVM.Navigation;

    /// <summary>
    /// Interface defining functionality for a view with navigation.
    /// </summary>
    public interface INavigationView : IView
    {
        /// <summary>
        /// Gets or sets a value indicating whether navigating away from the view should save the view model.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if navigating away should save the view model; otherwise, <c>false</c>.
        /// </value>
        bool NavigatingAwaySavesViewModel { get; set; }

        ///// <summary>
        ///// Occurs when the app has navigated to this view.
        ///// </summary>
        //event EventHandler<NavigatedEventArgs> NavigatedTo;

        ///// <summary>
        ///// Occurs when the app is about to navigate away from this view.
        ///// </summary>
        //event EventHandler<NavigatingEventArgs> NavigatingAway;

        ///// <summary>
        ///// Occurs when the app has navigated away from this view.
        ///// </summary>
        //event EventHandler<NavigatedEventArgs> NavigatedAway;
    }
}