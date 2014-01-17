// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFrameworkElementLoadedManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Manager that handles top =&gt; bottom loaded events for all framework elements inside an application.
    /// <para>
    /// </para>
    /// The reason this class is built is that in non-WPF technologies, the visual tree is loaded from
    /// bottom =&gt; top. However, Catel heavily relies on the order to be top =&gt; bottom.
    /// </summary>
    public interface IFrameworkElementLoadedManager
    {
        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="action">The action to execute when the framework element is loaded. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement" /> is <c>null</c>.</exception>
        void AddElement(FrameworkElement frameworkElement, Action action = null);

        /// <summary>
        /// Cleans up the dead links.
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Occurs when any of the subscribed framework elements are loaded.
        /// </summary>
        event EventHandler<FrameworkElementLoadedEventArgs> FrameworkElementLoaded;
    }
}