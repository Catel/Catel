// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakViewInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using Logging;

#if UWP
    using LoadedEventArgs = System.Object;
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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private WeakReference _view;

        private bool _isViewLoadState;
        #endregion


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakViewInfo"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="isViewLoaded">if set to <c>true</c>, the view is already loaded.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public WeakViewInfo(IView view, bool isViewLoaded = false)
        {
            Argument.IsNotNull("view", view);

            Initialize(view, isViewLoaded);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakViewInfo" /> class.
        /// </summary>
        /// <param name="viewLoadState">The view load state.</param>
        /// <param name="isViewLoaded">if set to <c>true</c>, the view is already loaded.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLoadState" /> is <c>null</c>.</exception>
        public WeakViewInfo(IViewLoadState viewLoadState, bool isViewLoaded = false)
        {
            Argument.IsNotNull("viewLoadState", viewLoadState);

            Initialize(viewLoadState, isViewLoaded);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the link to the <see cref="IView"/> is alive.
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
            get
            {
                if (_isViewLoadState)
                {
                    var viewLoadState = _view.Target as IViewLoadState;
                    if (viewLoadState != null)
                    {
                        return viewLoadState.View;
                    }

                    return null;
                }

                return _view.Target as IView;
            }
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
        #endregion

        #region Methods
        private void Initialize(object viewObject, bool isViewLoaded)
        {
            _view = new WeakReference(viewObject);
            _isViewLoadState = true;

            IsLoaded = isViewLoaded;
            _isViewLoadState = viewObject is IViewLoadState;

            if (this.SubscribeToWeakGenericEvent<LoadedEventArgs>(viewObject, "Loaded", OnViewLoadStateLoaded, false) == null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'view.Loaded', going to subscribe without weak events");

                ((IView) viewObject).Loaded += OnViewLoadStateLoaded;
            }

            if (this.SubscribeToWeakGenericEvent<LoadedEventArgs>(viewObject, "Unloaded", OnViewLoadStateUnloaded, false) == null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'view.Unloaded', going to subscribe without weak events");

                ((IView)viewObject).Unloaded += OnViewLoadStateUnloaded;
            }
        }

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnViewLoaded(object sender, EventArgs e)
        {
            OnLoaded();
        }

        /// <summary>
        /// Called when the view is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnViewUnloaded(object sender, EventArgs e)
        {
            OnUnloaded();
        }

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnViewLoadStateLoaded(object sender, LoadedEventArgs e)
        {
            OnLoaded();
        }

        /// <summary>
        /// Called when the view is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnViewLoadStateUnloaded(object sender, LoadedEventArgs e)
        {
            OnUnloaded();
        }

        private void OnLoaded()
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

        private void OnUnloaded()
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
        #endregion
    }
}
