// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewLoadedManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Manager that handles top =&gt; bottom loaded events for all views inside an application.
    /// <para>
    /// </para>
    /// The reason this class is built is that in non-WPF technologies, the visual tree is loaded from
    /// bottom =&gt; top. However, Catel heavily relies on the order to be top =&gt; bottom.
    /// </summary>
    public interface IViewLoadedManager
    {
        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        void AddView(IView view);

        /// <summary>
        /// Cleans up the dead links.
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Occurs when any of the subscribed framework elements are loaded.
        /// </summary>
        event EventHandler<ViewLoadedEventArgs> ViewLoaded;
    }
}