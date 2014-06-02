// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLoadedManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#endif

#if !NET && !XAMARIN
    using System.Windows;
#endif

    /// <summary>
    /// Available view load state events.
    /// </summary>
    public enum ViewLoadStateEvent
    {
        /// <summary>
        /// The view is about to be loaded.
        /// </summary>
        Loading,

        /// <summary>
        /// The view has just been loaded.
        /// </summary>
        Loaded,

        /// <summary>
        /// The view is about to be unloaded.
        /// </summary>
        Unloading,

        /// <summary>
        /// The view has just been unloaded.
        /// </summary>
        Unloaded
    }

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
        #region Fields
        private readonly List<WeakViewInfo> _viewElements = new List<WeakViewInfo>();
        private readonly Stack<WeakViewInfo> _loadedStack = new Stack<WeakViewInfo>();

        private ViewLoadStateEvent _lastInvokedViewLoadStateEvent;

        private readonly Timer _cleanUpTimer;
        private readonly object _lock = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLoadManager"/> class.
        /// </summary>
        public ViewLoadManager()
        {
            _cleanUpTimer = new Timer(x => CleanUp(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }
        #endregion

        #region IViewLoadedManager Members
        /// <summary>
        /// Adds the view load state.
        /// </summary>
        /// <param name="viewLoadState">The view load state.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLoadState" /> is <c>null</c>.</exception>
        public void AddView(IViewLoadState viewLoadState)
        {
            Argument.IsNotNull("viewLoadState", viewLoadState);

            var elementInfo = new WeakViewInfo(viewLoadState);

            elementInfo.Loaded += OnViewLoaded;
            elementInfo.Unloaded += OnViewUnloaded;

            lock (_lock)
            {
                _viewElements.Add(elementInfo);
            }
        }

        /// <summary>
        /// Cleans up the dead links.
        /// </summary>
        public void CleanUp()
        {
            lock (_lock)
            {
                for (int i = 0; i < _viewElements.Count; i++)
                {
                    var viewInfo = _viewElements[i];
                    if (!viewInfo.IsAlive)
                    {
                        viewInfo.Loaded -= OnViewLoaded;
                        viewInfo.Unloaded -= OnViewUnloaded;

#if NETFX_CORE || SILVERLIGHT
                        // Note: always unsubscribe LayoutUpdated
                        viewInfo.LayoutUpdated -= OnViewLayoutUpdated;
#endif

                        _viewElements.RemoveAt(i--);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when any of the subscribed views are about to be loaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> ViewLoading;

        /// <summary>
        /// Occurs when any of the subscribed views are loaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> ViewLoaded;

        /// <summary>
        /// Occurs when any of the subscribed views are about to be unloaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> ViewUnloading;

        /// <summary>
        /// Occurs when any of the subscribed views are unloaded.
        /// </summary>
        public event EventHandler<ViewLoadEventArgs> ViewUnloaded;
        #endregion

        #region Methods
        private void OnViewLoaded(object sender, EventArgs e)
        {
            var viewInfo = (WeakViewInfo)sender;

#if NETFX_CORE || SILVERLIGHT
            viewInfo.LayoutUpdated += OnViewLayoutUpdated;
#endif

            // Loaded is always called first on the inner child, add it to the stack
            lock (_loadedStack)
            {
                var view = viewInfo.View;

                _loadedStack.Push(viewInfo);

#if NETFX_CORE || SILVERLIGHT
                view.Dispatch(() => ((FrameworkElement)view).InvalidateMeasure());
#else
                // In WPF, handle view as loaded immediately
                HandleViewLoaded(view);
#endif
            }
        }

        private void OnViewUnloaded(object sender, EventArgs e)
        {
            var viewInfo = (WeakViewInfo)sender;

            // Loaded is always called first on the inner child, add it to the stack
            lock (_loadedStack)
            {
                var view = viewInfo.View;

                _loadedStack.Push(viewInfo);

                // Yes, invoke events right after each other
                InvokeViewLoadEvent(view, ViewLoadStateEvent.Unloading);
                InvokeViewLoadEvent(view, ViewLoadStateEvent.Unloaded);
            }
        }

#if NETFX_CORE || SILVERLIGHT
        private void OnViewLayoutUpdated(object sender, EventArgs e)
        {
            var viewInfo = (WeakViewInfo)sender;

            HandleViewLoaded(viewInfo.View);
        }
#endif

        private void HandleViewLoaded(IView view)
        {
            lock (_lock)
            {
                if (_loadedStack.Count > 0)
                {
                    var lastLoadedViewInfo = _loadedStack.Peek();
                    if (ReferenceEquals(view, lastLoadedViewInfo.View))
                    {
                        // We have reach the top control, now iterate the stack
                        while (_loadedStack.Count > 0)
                        {
                            var innerViewInfo = _loadedStack.Pop();
                            var innerView = innerViewInfo.View;

#if NETFX_CORE || SILVERLIGHT
                            innerViewInfo.LayoutUpdated -= OnViewLayoutUpdated;
#endif

                            // Yes, invoke events right after each other
                            InvokeViewLoadEvent(innerView, ViewLoadStateEvent.Loading);
                            InvokeViewLoadEvent(innerView, ViewLoadStateEvent.Loaded);
                        }
                    }
                }
            }
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

            if (view == null)
            {
                return;
            }

            EventHandler<ViewLoadEventArgs> handler = null;

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

            if (handler != null)
            {
                handler(this, new ViewLoadEventArgs(view));
            }

            _lastInvokedViewLoadStateEvent = viewLoadStateEvent;
        }
        #endregion
    }
}