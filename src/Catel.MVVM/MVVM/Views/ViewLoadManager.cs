namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Catel.Logging;

    /// <summary>
    /// Manager that handles top =&gt; bottom loaded events for all views inside an application.
    /// <para>
    /// </para>
    /// The reason this class is built is that in non-WPF technologies, the visual tree is loaded from
    /// bottom =&gt; top. However, Catel heavily relies on the order to be top =&gt; bottom.
    /// <para />
    /// This manager subscribes to both the <c>Loaded</c> and <c>LayoutUpdated</c>
    /// events. This is because in a nested scenario this will happen:
    /// <para />
    /// <code>
    /// <![CDATA[
    /// - UserControl 1
    ///   |- UserControl 2
    ///      |- UserControl 3
    /// ]]>
    /// </code>
    /// Will be executed in the following order:
    /// <para />
    /// <list type="number">
    ///   <item><description>Loaded (UC 3).</description></item>
    ///   <item><description>Loaded (UC 2).</description></item>
    ///   <item><description>Loaded (UC 1).</description></item>
    ///   <item><description>LayoutUpdated (UC 1).</description></item>
    ///   <item><description>LayoutUpdated (UC 2).</description></item>
    ///   <item><description>LayoutUpdated (UC 3).</description></item>
    /// </list>
    /// </summary>
    public class ViewLoadManager : IViewLoadManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly List<WeakViewInfo> _views = new List<WeakViewInfo>();

        private ViewLoadStateEvent _lastInvokedViewLoadStateEvent;

#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly Timer _cleanUpTimer;
#pragma warning restore IDISP006 // Implement IDisposable.

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLoadManager"/> class.
        /// </summary>
        public ViewLoadManager()
        {
            _cleanUpTimer = new Timer(x => CleanUp(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Occurs when any of the subscribed views are about to be loaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs>? ViewLoading;

        /// <summary>
        /// Occurs when any of the subscribed views are loaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs>? ViewLoaded;

        /// <summary>
        /// Occurs when any of the subscribed views are about to be unloaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs>? ViewUnloading;

        /// <summary>
        /// Occurs when any of the subscribed views are unloaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs>? ViewUnloaded;

        /// <summary>
        /// Adds the view load state.
        /// </summary>
        /// <param name="viewLoadState">The view load state.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLoadState" /> is <c>null</c>.</exception>
        public void AddView(IViewLoadState viewLoadState)
        {
            ArgumentNullException.ThrowIfNull(viewLoadState);
            ArgumentNullException.ThrowIfNull(viewLoadState.View);

            var viewInfo = new WeakViewInfo(viewLoadState.View);
            viewInfo.Loaded += OnViewInfoLoaded;
            viewInfo.Unloaded += OnViewInfoUnloaded;

            _views.Add(viewInfo);
        }

        private void OnViewInfoLoaded(object? sender, EventArgs e)
        {
            var weakViewInfo = sender as WeakViewInfo;
            if (weakViewInfo is null || weakViewInfo.View is null)
            {
                throw Log.ErrorAndCreateException<CatelException>($"Received event from WeakViewInfo without valid sender, cannot handle view events correctly");
            }

            // Just forward
            RaiseLoaded(weakViewInfo.View);
        }

        private void OnViewInfoUnloaded(object? sender, EventArgs e)
        {
            var weakViewInfo = sender as WeakViewInfo;
            if (weakViewInfo is null || weakViewInfo.View is null)
            {
                throw Log.ErrorAndCreateException<CatelException>($"Received event from WeakViewInfo without valid sender, cannot handle view events correctly");
            }

            // Just forward
            RaiseUnloaded(weakViewInfo.View);
        }

        /// <summary>
        /// Cleans up the dead links.
        /// </summary>
        public void CleanUp()
        {
            for (var i = 0; i < _views.Count; i++)
            {
                var view = _views[i];
                if (!view.IsAlive)
                {
                    view.Loaded -= OnViewInfoLoaded;
                    view.Unloaded -= OnViewInfoUnloaded;

                    _views.RemoveAt(i--);
                }
            }
        }

        private void RaiseLoaded(IView view)
        {
            // Yes, invoke events right after each other
            InvokeViewLoadEvent(view, ViewLoadStateEvent.Loading);
            InvokeViewLoadEvent(view, ViewLoadStateEvent.Loaded);
        }

        private void RaiseUnloaded(IView view)
        {
            // Yes, invoke events right after each other
            InvokeViewLoadEvent(view, ViewLoadStateEvent.Unloading);
            InvokeViewLoadEvent(view, ViewLoadStateEvent.Unloaded);
        }

        /// <summary>
        /// Invokes the specific view load event and makes sure that it isn't double invoked.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewLoadStateEvent">The view load state event.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">viewLoadStateEvent</exception>
        protected void InvokeViewLoadEvent(IView view, ViewLoadStateEvent viewLoadStateEvent)
        {
            if (_lastInvokedViewLoadStateEvent == viewLoadStateEvent)
            {
                return;
            }

            if (view is null)
            {
                return;
            }

            EventHandler<ViewLoadEventArgs>? handler;

            switch (viewLoadStateEvent)
            {
                case ViewLoadStateEvent.Loading:
                    handler = ViewLoading;
                    break;

                case ViewLoadStateEvent.Loaded:
                    handler = ViewLoaded;
                    break;

                case ViewLoadStateEvent.Unloading:
                    handler = ViewUnloading;
                    break;

                case ViewLoadStateEvent.Unloaded:
                    handler = ViewUnloaded;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("viewLoadStateEvent");
            }

            if (handler is not null)
            {
                handler(this, new ViewLoadEventArgs(view));
            }

            _lastInvokedViewLoadStateEvent = viewLoadStateEvent;
        }
    }
}
