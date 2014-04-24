// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakViewInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;

#if NETFX_CORE
    using LoadedEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
    using LayoutUpdatedEventArgs = System.Object;
#else
    using LoadedEventArgs = System.EventArgs;
    using LayoutUpdatedEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// Class containing weak events for a <see cref="IView"/>. This way it is safe to subscribe
    /// to events of a <see cref="IView"/> without causing memory leaks.
    /// </summary>
    public class WeakViewInfo
    {
        #region Fields
        private readonly WeakReference _view;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakViewInfo"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public WeakViewInfo(IView view)
        {
            Argument.IsNotNull("view", view);

            _view = new WeakReference(view);

            this.SubscribeToWeakGenericEvent<LoadedEventArgs>(view, "Loaded", OnLoaded);
            this.SubscribeToWeakGenericEvent<LoadedEventArgs>(view, "Unloaded", OnUnloaded);

#if !NET && !XAMARIN
            this.SubscribeToWeakGenericEvent<LayoutUpdatedEventArgs>(view, "LayoutUpdated", OnLayoutUpdated);
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the link to the <see cref="View"/> is alive.
        /// </summary>
        /// <value><c>true</c> if the link is alive; otherwise, <c>false</c>.</value>
        public bool IsAlive
        {
            get { return _view.IsAlive; }
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        public IView View
        {
            get { return _view.Target as IView; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="View"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if the <see cref="View"/> is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        public event EventHandler<EventArgs> Loaded;

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        public event EventHandler<EventArgs> Unloaded;

#if !NET && !XAMARIN
        /// <summary>
        /// Occurs when the view layout is updated.
        /// </summary>
        public event EventHandler<EventArgs> LayoutUpdated;
#endif
        #endregion

        #region Methods
        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnLoaded(object sender, LoadedEventArgs e)
        {
            if (IsLoaded)
            {
                return;
            }

            IsLoaded = true;

            var loaded = Loaded;
            if (loaded != null)
            {
                loaded(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the view is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnUnloaded(object sender, LoadedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            IsLoaded = false;

            var unloaded = Unloaded;
            if (unloaded != null)
            {
                unloaded(this, EventArgs.Empty);
            }
        }
         
#if !NET && !XAMARIN
        /// <summary>
        /// Called when the view layout is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnLayoutUpdated(object sender, LayoutUpdatedEventArgs e)
        {
            var layoutUpdated = LayoutUpdated;
            if (layoutUpdated != null)
            {
                layoutUpdated(this, EventArgs.Empty);
            }
        }
#endif
        #endregion
    }
}