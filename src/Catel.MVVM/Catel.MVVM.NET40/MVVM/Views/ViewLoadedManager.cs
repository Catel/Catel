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
    public class ViewLoadedManager : IViewLoadedManager
    {
        #region Fields
        private readonly List<WeakViewInfo> _viewElements = new List<WeakViewInfo>();

        private readonly Stack<WeakViewInfo> _loadedStack = new Stack<WeakViewInfo>();

        private readonly object _lock = new object();

        private readonly Timer _timer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLoadedManager"/> class.
        /// <para />
        /// This constructor automatically cleans up every minute.
        /// </summary>
        public ViewLoadedManager()
            : this(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLoadedManager"/> class.
        /// </summary>
        /// <param name="automaticallyCleanUpEveryMinute">If set to <c>true</c>, this manager will automatically clean up every minute.</param>
        public ViewLoadedManager(bool automaticallyCleanUpEveryMinute)
        {
            if (automaticallyCleanUpEveryMinute)
            {
                _timer = new Timer(OnTimerTick, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            }
        }
        #endregion

        #region IViewLoadedManager Members
        /// <summary>
        /// Adds the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="action">The action to execute when the framework element is loaded. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
        public void AddView(IView view, Action action = null)
        {
            Argument.IsNotNull("view", view);

            var elementInfo = new WeakViewInfo(view, action);

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
                        viewInfo.Action = null;

                        viewInfo.Loaded -= OnViewLoaded;
                        viewInfo.Unloaded -= OnViewUnloaded;

#if !NET && !XAMARIN
                        // Note: always unsubscribe LayoutUpdated
                        viewInfo.LayoutUpdated -= OnViewLayoutUpdated;
#endif

                        _viewElements.RemoveAt(i--);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when any of the subscribed views are loaded.
        /// </summary>
        public event EventHandler<EventArgs> ViewLoaded;
        #endregion

        #region Methods
        private void OnViewLoaded(object sender, EventArgs e)
        {
            var viewInfo = (WeakViewInfo)sender;

#if !NET && !XAMARIN
            viewInfo.LayoutUpdated += OnViewLayoutUpdated;
#endif

            // Loaded is always called first on the inner child, add it to the stack
            lock (_loadedStack)
            {
                var view = viewInfo.View;

                _loadedStack.Push(viewInfo);

#if !NET && !XAMARIN
                view.Dispatch(() => ((FrameworkElement)view).InvalidateMeasure());
#else
                // In WPF, handle view as loaded immediately
                HandleViewLoaded(view);
#endif
            }
        }

        private void OnViewUnloaded(object sender, EventArgs e)
        {
            // Not interesting for now...
        }

        private void OnViewLayoutUpdated(object sender, EventArgs e)
        {
            var viewInfo = (WeakViewInfo)sender;

            HandleViewLoaded(viewInfo.View);
        }

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
#if !NET && !XAMARIN
                            innerViewInfo.LayoutUpdated -= OnViewLayoutUpdated;
#endif

                            if (innerViewInfo.Action != null)
                            {
                                innerViewInfo.Action();
                            }

                            RaiseViewLoaded(innerViewInfo.View);
                        }
                    }
                }
            }
        }

        private void RaiseViewLoaded(IView view)
        {
            var viewLoaded = ViewLoaded;
            if (viewLoaded != null)
            {
                viewLoaded(this, new ViewLoadedEventArgs(view));
            }
        }

        private void OnTimerTick(object state)
        {
            CleanUp();
        }
        #endregion
    }
}