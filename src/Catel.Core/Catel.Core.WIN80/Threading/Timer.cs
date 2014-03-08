// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Timer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if !WIN81
namespace System.Threading
{
    using Tasks;
    using Catel;

    /// <summary>
    /// Timer callback delegate.
    /// </summary>
    /// <param name="state">
    /// The state.
    /// </param>
    public delegate void TimerCallback(object state);

    /// <summary>
    /// Timer for WinRT since WinRT only provides the DispatcherTimer which cannot be used outside the UI thread.
    /// </summary>
    public class Timer : IDisposable
    {
        #region Fields
        private bool _isTimerRunning;
        private readonly TimerCallback _timerCallback;
        private readonly object _timerState;

        private bool _isKillingTimer;
        private bool _hasKilledTimer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        public Timer()
            : this(100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer"/> class.
        /// </summary>
        /// <param name="interval">
        /// The interval in milliseconds.
        /// </param>
        public Timer(int interval)
        {
            Interval = interval;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <param name="dueTime">The due time.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback" /> is <c>null</c>.</exception>
        public Timer(TimerCallback callback, object state, int dueTime, int interval)
            : this(callback, state, TimeSpan.FromMilliseconds(dueTime), TimeSpan.FromMilliseconds(interval)) { }

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
            Argument.IsNotNull("callback", callback);

            _timerCallback = callback;
            _timerState = state;

            Change(dueTime, interval);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>The interval. The default is 100 milliseconds.</value>
        public int Interval { get; set; }
        #endregion

        #region Events
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
        public async void Change(TimeSpan dueTime, TimeSpan interval)
        {
            if (_isTimerRunning)
            {
                _isKillingTimer = true;

                while (!_hasKilledTimer)
                {
                    await Task.Delay(10);
                }

                _isKillingTimer = false;
                _hasKilledTimer = false;
                
            }

            Interval = (int)interval.TotalMilliseconds;

#pragma warning disable 4014
            Task.Run(() =>
#pragma warning restore 4014
            {
                Task.Delay(dueTime);
                Start();
            });
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        private async void Start()
        {
            _isTimerRunning = true;

            while (_isTimerRunning)
            {
                if (_isKillingTimer)
                {
                    _hasKilledTimer = true;
                    _isTimerRunning = false;
                    break;
                }

                TimerElapsed();

                await Task.Delay(Interval);
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        private void Stop()
        {
            _isTimerRunning = false;
        }

        /// <summary>
        /// Called when the interval elapses.
        /// </summary>
        private void TimerElapsed()
        {
            Elapsed.SafeInvoke(this);

            if (_timerCallback != null)
            {
                _timerCallback(_timerState);
            }
        }
        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}
#endif