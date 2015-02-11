// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewLoadedManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

#if SILVERLIGHT
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
    /// <remarks>
    /// To get the best performance, this class will only execute logic on silverlight systems. All other systems correctly
    /// support the loaded event.
    /// </remarks>
    public class ViewLoadManager : IViewLoadManager
    {
        #region Fields
#if SILVERLIGHT
        private readonly List<ViewStack> _viewStacks = new List<ViewStack>();
        private readonly Dictionary<FrameworkElement, UninitializedViewInfo> _uninitializedViews = new Dictionary<FrameworkElement, UninitializedViewInfo>();
#else
        private readonly List<WeakViewInfo> _views = new List<WeakViewInfo>();
#endif

        private ViewLoadStateEvent _lastInvokedViewLoadStateEvent;

        private readonly Timer _cleanUpTimer;
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

        #region Events
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
        /// <summary>
        /// Adds the view load state.
        /// </summary>
        /// <param name="viewLoadState">The view load state.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLoadState" /> is <c>null</c>.</exception>
        public void AddView(IViewLoadState viewLoadState)
        {
            Argument.IsNotNull("viewLoadState", viewLoadState);

#if SILVERLIGHT
            var frameworkElement = viewLoadState.View as FrameworkElement;
            if (frameworkElement != null)
            {
                _uninitializedViews[frameworkElement] = new UninitializedViewInfo(viewLoadState);

                frameworkElement.Loaded += OnFrameworkElementLoaded;
            }
#else
            var viewInfo = new WeakViewInfo(viewLoadState.View);
            viewInfo.Loaded += OnViewInfoLoaded;
            viewInfo.Unloaded += OnViewInfoUnloaded;

            _views.Add(viewInfo);
#endif
        }

#if SILVERLIGHT
        private void OnFrameworkElementLoaded(object sender, EventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;

            frameworkElement.Loaded -= OnFrameworkElementLoaded;

            var uninitializedViewInfo = _uninitializedViews[frameworkElement];

            AddViewAfterLoaded(uninitializedViewInfo);

            _uninitializedViews.Remove(frameworkElement);
        }

        private void AddViewAfterLoaded(UninitializedViewInfo uninitializedViewInfo)
        {
            var isTopViewStack = true;

            var viewLoadState = uninitializedViewInfo.ViewLoadState;
            var view = viewLoadState.View;
            ViewStack viewStack = null;

            var parent = view.FindParentByPredicate(x => x is IView) as FrameworkElement;
            if (parent != null)
            {
                if (_uninitializedViews.ContainsKey(parent))
                {
                    // We have a different uninitialized view that is the parent
                    var uninitializedParent = _uninitializedViews[parent];
                    uninitializedParent.ViewStack.AddChild(uninitializedViewInfo.ViewStack, uninitializedParent.ViewStack);

                    isTopViewStack = false;
                }
                else
                {
                    // We are now listed to be added to the visual tree
                    foreach (var existingViewStack in _viewStacks)
                    {
                        if (existingViewStack.ContainsView((IView)parent))
                        {
                            var viewAsFrameworkElement = view as FrameworkElement;
                            if (viewAsFrameworkElement != null && _uninitializedViews.ContainsKey(viewAsFrameworkElement))
                            {
                                // This happens when we are called from OnFrameworkElementLoaded but out parent wasn't updated yet
                                existingViewStack.AddChild(_uninitializedViews[viewAsFrameworkElement].ViewStack, existingViewStack);
                            }
                            else
                            {
                                existingViewStack.AddChild(view, existingViewStack);
                            }
                            

                            viewStack = existingViewStack;
                            isTopViewStack = false;
                            break;
                        }
                    }
                }
            }

            if (isTopViewStack)
            {
                var topViewStack = uninitializedViewInfo.ViewStack;

                topViewStack.ViewStackLoaded += OnViewStackLoaded;
                topViewStack.ViewStackUnloaded += OnViewStackUnloaded;

                _viewStacks.Add(topViewStack);

                viewStack = topViewStack;
            }

            if (viewStack != null)
            {
                viewStack.NotifyThatParentIsReadyToAcceptLoadedMessages();
            }
        }

        private void OnViewStackLoaded(object sender, ViewStackPartEventArgs e)
        {
            RaiseLoaded(e.View);
        }

        private void OnViewStackUnloaded(object sender, ViewStackPartEventArgs e)
        {
            RaiseUnloaded(e.View);
        }
#else
        private void OnViewInfoLoaded(object sender, EventArgs e)
        {
            // Just forward
            RaiseLoaded(((WeakViewInfo)sender).View);
        }

        private void OnViewInfoUnloaded(object sender, EventArgs e)
        {
            // Just forward
            RaiseUnloaded(((WeakViewInfo)sender).View);
        }
#endif

        /// <summary>
        /// Cleans up the dead links.
        /// </summary>
        public void CleanUp()
        {
#if SILVERLIGHT
            for (int i = 0; i < _viewStacks.Count; i++)
            {
                var viewStack = _viewStacks[i];
                if (viewStack.IsOutdated)
                {
                    viewStack.ViewStackLoaded -= OnViewStackLoaded;
                    viewStack.ViewStackUnloaded -= OnViewStackUnloaded;

                    viewStack.Dispose();

                    _viewStacks.RemoveAt(i--);
                }
                else
                {
                    viewStack.CheckForOutdatedChildren();
                }
            }
#else
            for (int i = 0; i < _views.Count; i++)
            {
                var view = _views[i];
                if (!view.IsAlive)
                {
                    view.Loaded -= OnViewInfoLoaded;
                    view.Unloaded -= OnViewInfoUnloaded;

                    _views.RemoveAt(i--);
                }
            }
#endif
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

            if (view == null)
            {
                return;
            }

            EventHandler<ViewLoadEventArgs> handler;

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

#if SILVERLIGHT
        private class UninitializedViewInfo
        {
            public UninitializedViewInfo(IViewLoadState viewLoadState)
            {
                ViewLoadState = viewLoadState;

                ViewStack = new ViewStack(viewLoadState.View, false);
            }

            public IViewLoadState ViewLoadState { get; private set; }

            public ViewStack ViewStack { get; private set; }
        }
#endif
    }
}