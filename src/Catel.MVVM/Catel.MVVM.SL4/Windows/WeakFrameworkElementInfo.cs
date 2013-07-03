// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakFrameworkElementInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows
{
    using System;

#if NETFX_CORE
    using LoadingEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
    using LayoutUpdatedEventArgs = System.Object;
    using global::Windows.UI.Xaml;
#else
    using LoadingEventArgs = System.EventArgs;
    using LayoutUpdatedEventArgs = System.EventArgs;
    using System.Windows;
#endif

    /// <summary>
    /// Class containing weak events for a <see cref="FrameworkElement"/>. This way it is safe to subscribe
    /// to events of a <see cref="FrameworkElement"/> without causing memory leaks.
    /// </summary>
    public class WeakFrameworkElementInfo
    {
        #region Fields
        private readonly WeakReference _frameworkElement;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakFrameworkElementInfo" /> class.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement" /> is <c>null</c>.</exception>
        public WeakFrameworkElementInfo(FrameworkElement frameworkElement, Action action)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            _frameworkElement = new WeakReference(frameworkElement);
            Action = action;

            this.SubscribeToWeakGenericEvent<LoadingEventArgs>(frameworkElement, "Loaded", OnLoaded);
            this.SubscribeToWeakGenericEvent<LoadingEventArgs>(frameworkElement, "Unloaded", OnUnloaded);
            this.SubscribeToWeakGenericEvent<LayoutUpdatedEventArgs>(frameworkElement, "LayoutUpdated", OnLayoutUpdated);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the link to the <see cref="FrameworkElement"/> is alive.
        /// </summary>
        /// <value><c>true</c> if the link is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get { return _frameworkElement.IsAlive; }
        }

        /// <summary>
        /// Gets the framework element.
        /// </summary>
        /// <value>The framework element.</value>
        public FrameworkElement FrameworkElement
        {
            get { return _frameworkElement.Target as FrameworkElement; }
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public Action Action { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="FrameworkElement"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if the <see cref="FrameworkElement"/> is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the framework element is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnLoaded(object sender, LoadingEventArgs e)
        {
            if (IsLoaded)
            {
                return;
            }

            IsLoaded = true;

            var loaded = Loaded;
            if (loaded != null)
            {
                loaded(this, e);
            }
        }

        /// <summary>
        /// Called when the framework element is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUnloaded(object sender, LoadingEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            IsLoaded = false;

            var unloaded = Unloaded;
            if (unloaded != null)
            {
                unloaded(this, e);
            }
        }

        /// <summary>
        /// Called when the framework element layout is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnLayoutUpdated(object sender, LayoutUpdatedEventArgs e)
        {
            var layoutUpdated = LayoutUpdated;
            if (layoutUpdated != null)
            {
                layoutUpdated(this, e);
            }
        }
        #endregion

        /// <summary>
        /// Occurs when the framework element is loaded.
        /// </summary>
        public event EventHandler<LoadingEventArgs> Loaded;

        /// <summary>
        /// Occurs when the framework element is unloaded.
        /// </summary>
        public event EventHandler<LoadingEventArgs> Unloaded;

        /// <summary>
        /// Occurs when the framework element layout is updated.
        /// </summary>
        public event EventHandler<LayoutUpdatedEventArgs> LayoutUpdated;
    }
}