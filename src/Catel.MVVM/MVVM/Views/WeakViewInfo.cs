namespace Catel.MVVM.Views
{
    using System;
    using Logging;
    using LoadedEventArgs = System.EventArgs;

    /// <summary>
    /// Class containing weak events for a <see cref="IView"/>. This way it is safe to subscribe
    /// to events of a <see cref="IView"/> without causing memory leaks.
    /// </summary>
    public class WeakViewInfo
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly WeakReference _view;

        private bool _isViewLoadState;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakViewInfo"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="isViewLoaded">if set to <c>true</c>, the view is already loaded.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public WeakViewInfo(IView view, bool isViewLoaded = false)
        {
            ArgumentNullException.ThrowIfNull(view);

            _view = new WeakReference(view);
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
            ArgumentNullException.ThrowIfNull(viewLoadState);

            _view = new WeakReference(viewLoadState);
            Initialize(viewLoadState, isViewLoaded);
        }

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
        public IView? View
        {
            get
            {
                IView? view = null;

                if (_isViewLoadState)
                {
                    var viewLoadState = _view.Target as IViewLoadState;
                    if (viewLoadState is not null)
                    {
                        view = viewLoadState.View;
                    }
                }
                else
                {
                    view = _view.Target as IView;
                }

                return view;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="View"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if the <see cref="View"/> is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Occurs when the view is loaded.
        /// </summary>
        public event EventHandler<EventArgs>? Loaded;

        /// <summary>
        /// Occurs when the view is unloaded.
        /// </summary>
        public event EventHandler<EventArgs>? Unloaded;

        private void Initialize(object viewObject, bool isViewLoaded)
        {
            IsLoaded = isViewLoaded;
            _isViewLoadState = viewObject is IViewLoadState;

            if (this.SubscribeToWeakGenericEvent<LoadedEventArgs>(viewObject, nameof(View.Loaded), OnViewLoadStateLoaded, false) is null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'view.Loaded', going to subscribe without weak events");

                ((IView) viewObject).Loaded += OnViewLoadStateLoaded;
            }

            if (this.SubscribeToWeakGenericEvent<LoadedEventArgs>(viewObject, nameof(View.Unloaded), OnViewLoadStateUnloaded, false) is null)
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
        public void OnViewLoaded(object? sender, EventArgs e)
        {
            OnLoaded();
        }

        /// <summary>
        /// Called when the view is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnViewUnloaded(object? sender, EventArgs e)
        {
            OnUnloaded();
        }

        /// <summary>
        /// Called when the view is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnViewLoadStateLoaded(object? sender, LoadedEventArgs e)
        {
            OnLoaded();
        }

        /// <summary>
        /// Called when the view is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public void OnViewLoadStateUnloaded(object? sender, LoadedEventArgs e)
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
            if (loaded is not null)
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
            if (unloaded is not null)
            {
                unloaded(this, EventArgs.Empty);
            }
        }
    }
}
