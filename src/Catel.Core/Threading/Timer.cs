// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#define USE_INTERNAL_TIMER

namespace Catel.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The timeout class.
    /// </summary>
    public static class Timeout
    {
        /// <summary>
        /// A constant used to specify an infinite waiting period, for threading methods that accept an Int32 parameter.
        /// </summary>
        public const int Infinite = -1;

        /// <summary>
        /// A constant used to specify an infinite waiting period, for threading methods that accept an TimeSpan parameter.
        /// </summary>
        public static readonly TimeSpan InfiniteTimeSpan = TimeSpan.FromMilliseconds(-1);
    }

    /// <summary>
    /// Timer callback delegate.
    /// </summary>
    /// <param name="state">The state.</param>
    public delegate void TimerCallback(object state);

    /// <summary>
    /// Timer for WinRT since WinRT only provides the DispatcherTimer which cannot be used outside the UI thread.
    /// </summary>
    public class Timer : IDisposable
    {
        #region Fields
        private readonly TimerCallback _timerCallback;
        private readonly object _timerState;

        private readonly object _lock = new object();
        private System.Threading.CancellationTokenSource _cancellationTokenSource;

#if USE_INTERNAL_TIMER
        private System.Threading.Timer _timer;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer()
            : this(Timeout.Infinite)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="interval">The interval in milliseconds.</param>
        public Timer(int interval)
            : this(null, null, TimeSpan.FromMilliseconds(interval), TimeSpan.FromMilliseconds(interval))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public Timer(TimerCallback callback)
            : this(callback, null, Timeout.Infinite, Timeout.Infinite)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <param name="dueTime">The due time.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback" /> is <c>null</c>.</exception>
        public Timer(TimerCallback callback, object state, int dueTime, int interval)
            : this(callback, state, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(interval))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <param name="dueTime">The due time.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback" /> is <c>null</c>.</exception>
        public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan interval)
        {
            _timerCallback = callback;
            _timerState = state;

            Interval = (int)interval.TotalMilliseconds;

            Change(dueTime, interval);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval.</value>
        public int Interval { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the interval changes.
        /// </summary>
        public event EventHandler<EventArgs> Changed;
 
        /// <summary>
        /// Occurs when the interval elapses.
        /// </summary>
        public event EventHandler<EventArgs> Elapsed;
        #endregion

        #region Methods

        /// <summary>
        /// Changes the specified interval.
        /// </summary>
        /// <param name="dueTime">The due time.</param>
        /// <param name="interval">The interval.</param>
        public void Change(int dueTime, int interval)
        {
            Change(TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(interval));
        }

        /// <summary>
        /// Changes the specified interval.
        /// </summary>
        /// <param name="dueTime">The due time.</param>
        /// <param name="interval">The interval.</param>
        public void Change(TimeSpan dueTime, TimeSpan interval)
        {
            SetUpTimer(dueTime, interval);

            var cancellationToken = GetCancellationToken();

            if (dueTime < TimeSpan.Zero)
            {
                // Never invoke initial one
            }
            else if (dueTime == TimeSpan.Zero)
            {
                // Invoke immediately
                TimerElapsed();

                Start(cancellationToken);
            }
            else
            {
                // Invoke after due time
#if USE_INTERNAL_TIMER
                lock (_lock)
                {
                    if (_timer != null)
                    {
                        _timer.Change(dueTime, Timeout.InfiniteTimeSpan);
                    }
                }
#else
                var delayTask = TaskShim.Delay(dueTime, cancellationToken);
                delayTask.ContinueWith(ContinueTimer, cancellationToken, cancellationToken);
#endif
            }

            // Note: don't use SafeInvoke, this event could be raised a *lot*
            var changed = Changed;
            if (changed != null)
            {
                changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets up the timer.
        /// </summary>
        /// <param name="dueTime"></param>
        /// <param name="interval"></param>
        private void SetUpTimer(TimeSpan dueTime, TimeSpan interval)
        {
            Stop();

            var cancellationToken = GetCancellationToken();

#if USE_INTERNAL_TIMER
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                    _timer = null;
                }

                _timer = new System.Threading.Timer(OnTimerTick, cancellationToken, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
#endif

            Interval = (int)interval.TotalMilliseconds;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        private void Start(CancellationToken cancellationToken)
        {
            if (Interval <= 0)
            {
                // Never start a timer
                return;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

#if USE_INTERNAL_TIMER
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Change(Interval, Timeout.Infinite);
                }
            }
#else
            var delayTask = TaskShim.Delay(Interval, cancellationToken);
            delayTask.ContinueWith(ContinueTimer, cancellationToken, cancellationToken);
#endif
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        private void Stop()
        {
            lock (_lock)
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
            }

#if USE_INTERNAL_TIMER
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer = null;
                }
            }
#endif
        }

        private CancellationToken GetCancellationToken()
        {
            lock (_lock)
            {
                if (_cancellationTokenSource == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                }

                return _cancellationTokenSource.Token;
            }
        }

        /// <summary>
        /// Continues the timer.
        /// </summary>
        /// <param name="t">The task.</param>
        /// <param name="state">The state which must be the cancellation token.</param>
        private void ContinueTimer(Task t, object state)
        {
            var cancellationToken = (CancellationToken)state;
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            TimerElapsed();

            if (!cancellationToken.IsCancellationRequested)
            {
                Start(cancellationToken);
            }
        }

        /// <summary>
        /// Called when the interval elapses.
        /// </summary>
        private void TimerElapsed()
        {
            var elapsed = Elapsed;
            if (elapsed != null)
            {
                elapsed(this, EventArgs.Empty);
            }

            if (_timerCallback != null)
            {
                _timerCallback(_timerState);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

#if USE_INTERNAL_TIMER
        private void OnTimerTick(object state)
        {
            ContinueTimer(null, state);
        }
#endif
#endregion
    }
}