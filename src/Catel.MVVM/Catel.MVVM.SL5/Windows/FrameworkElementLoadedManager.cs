// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameworkElementLoadedManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;

#if NETFX_CORE
    using LoadingEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
    using LayoutUpdatedEventArgs = System.Object;
    using Catel.Windows.Threading;
    using global::Windows.UI.Xaml;
#else
    using LoadingEventArgs = System.EventArgs;
    using LayoutUpdatedEventArgs = System.EventArgs;
    using System.Windows;
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Manager that handles top =&gt; bottom loaded events for all framework elements inside an application.
    /// <para>
    /// </para>
    /// The reason this class is built is that in non-WPF technologies, the visual tree is loaded from
    /// bottom =&gt; top. However, Catel heavily relies on the order to be top =&gt; bottom.
    /// <para />
    /// This manager subscribes to both the <see cref="FrameworkElement.Loaded"/> and <see cref="FrameworkElement.LayoutUpdated"/> 
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
    /// <para />
    /// This manager will call the <see cref="FrameworkElementLoaded"/> when the first <see cref="FrameworkElement.LayoutUpdated"/> is 
    /// called on the right time on the right element in the correct expected order.
    /// </summary>
    public class FrameworkElementLoadedManager : IFrameworkElementLoadedManager
    {
        #region Fields
        private readonly List<WeakFrameworkElementInfo> _frameworkElements = new List<WeakFrameworkElementInfo>();

        private readonly Stack<WeakFrameworkElementInfo> _loadedStack = new Stack<WeakFrameworkElementInfo>();

        private readonly object _lock = new object();

        private readonly DispatcherTimer _timer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElementLoadedManager"/> class.
        /// <para />
        /// This constructor automatically cleans up every minute.
        /// </summary>
        public FrameworkElementLoadedManager()
            : this(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElementLoadedManager"/> class.
        /// </summary>
        /// <param name="automaticallyCleanUpEveryMinute">If set to <c>true</c>, this manager will automatically clean up every minute.</param>
        public FrameworkElementLoadedManager(bool automaticallyCleanUpEveryMinute)
        {
            if (automaticallyCleanUpEveryMinute)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 1, 0);
                _timer.Tick += (sender, e) => OnTimerTick();
                _timer.Start();
            }
        }
        #endregion

        #region IFrameworkElementLoadedManager Members
        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="frameworkElement">
        /// The framework element.
        /// </param>
        /// <param name="action">
        /// The action to execute when the framework element is loaded. Can be <c>null</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="frameworkElement"/> is <c>null</c>.
        /// </exception>
        public void AddElement(FrameworkElement frameworkElement, Action action = null)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            var elementInfo = new WeakFrameworkElementInfo(frameworkElement, action);

            elementInfo.Loaded += OnFrameworkElementLoaded;
            elementInfo.Unloaded += OnFrameworkElementUnloaded;

            lock (_lock)
            {
                _frameworkElements.Add(elementInfo);
            }
        }

        /// <summary>
        /// Cleans up the dead links.
        /// </summary>
        public void CleanUp()
        {
            lock (_lock)
            {
                for (int i = 0; i < _frameworkElements.Count; i++)
                {
                    var elementInfo = _frameworkElements[i];
                    if (!elementInfo.IsAlive)
                    {
                        elementInfo.Action = null;

                        elementInfo.Loaded -= OnFrameworkElementLoaded;
                        elementInfo.Unloaded -= OnFrameworkElementUnloaded;

                        // Note: always unsubscribe LayoutUpdated
                        elementInfo.LayoutUpdated -= OnFrameworkElementLayoutUpdated;

                        _frameworkElements.RemoveAt(i--);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when any of the subscribed framework elements are loaded.
        /// </summary>
        public event EventHandler<FrameworkElementLoadedEventArgs> FrameworkElementLoaded;
        #endregion

        #region Methods
        private void OnFrameworkElementLoaded(object sender, LoadingEventArgs e)
        {
            var elementInfo = (WeakFrameworkElementInfo)sender;
            elementInfo.LayoutUpdated += OnFrameworkElementLayoutUpdated;

            // Loaded is always called first on the inner child, add it to the stack
            lock (_loadedStack)
            {
                _loadedStack.Push(elementInfo);

                var frameworkElement = elementInfo.FrameworkElement;
                frameworkElement.Dispatcher.BeginInvoke(() => frameworkElement.InvalidateMeasure());
            }
        }

        private void OnFrameworkElementUnloaded(object sender, LoadingEventArgs e)
        {
            // Not interesting for now...
        }

        private void OnFrameworkElementLayoutUpdated(object sender, LayoutUpdatedEventArgs e)
        {
            var elementInfo = (WeakFrameworkElementInfo)sender;

            lock (_lock)
            {
                if (_loadedStack.Count > 0)
                {
                    var lastLoadedElementInfo = _loadedStack.Peek();
                    if (ReferenceEquals(elementInfo.FrameworkElement, lastLoadedElementInfo.FrameworkElement))
                    {
                        // We have reach the top control, now iterate the stack
                        while (_loadedStack.Count > 0)
                        {
                            var innerElementInfo = _loadedStack.Pop();
                            innerElementInfo.LayoutUpdated -= OnFrameworkElementLayoutUpdated;

                            if (innerElementInfo.Action != null)
                            {
                                innerElementInfo.Action();
                            }

                            var frameworkElementLoaded = FrameworkElementLoaded;
                            if (frameworkElementLoaded != null)
                            {
                                frameworkElementLoaded(this, new FrameworkElementLoadedEventArgs(innerElementInfo.FrameworkElement));
                            }
                        }
                    }
                }
            }
        }

        private void OnTimerTick()
        {
            CleanUp();
        }
        #endregion
    }
}