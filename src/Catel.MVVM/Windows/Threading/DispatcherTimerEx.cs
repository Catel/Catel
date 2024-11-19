namespace Catel.Windows.Threading
{
    using System;
    using System.Timers;
    using Catel.Services;

    /// <summary>
    /// Dispatcher timer that uses the <see cref="IDispatcherService"/> to dispatch its calls so it can be
    /// used inside unit tests.
    /// </summary>
    public class DispatcherTimerEx
    {
        protected readonly IDispatcherService _dispatcherService;

#pragma warning disable IDISP006 // Implement IDisposable
        private readonly System.Timers.Timer _timer = new System.Timers.Timer();
#pragma warning restore IDISP006 // Implement IDisposable

        private readonly object _lockObject = new object();
        private bool _isSubscribed;

        public DispatcherTimerEx(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;

            OnlyBeginInvokeIfRequired = true;
        }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        public virtual TimeSpan Interval
        {
            get => TimeSpan.FromMilliseconds(_timer.Interval);
            set => _timer.Interval = value.TotalMilliseconds;
        }

        /// <summary>
        /// Gets or sets whether the timer is enabled.
        /// </summary>
        public virtual bool IsEnabled
        {
            get => _timer.Enabled;
            set => _timer.Enabled = value;
        }

        /// <summary>
        /// Gets or sets a user-defined data object.
        /// </summary>
        public virtual object? Tag { get; set; }

        /// <summary>
        /// If set the <c>true</c>, the timer will pass <c>true</c> to <see cref="IDispatcherService.BeginInvoke(Action, bool)"/>.
        /// </summary>
        public virtual bool OnlyBeginInvokeIfRequired { get; set; }

        /// <summary>
        /// Occurs when the timer ticks.
        /// </summary>
        public event EventHandler<EventArgs>? Tick = default;

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public virtual void Start()
        {
            lock (_lockObject)
            {
                if (!_isSubscribed)
                {
                    _timer.Elapsed += OnTimerElapsed;
                    _isSubscribed = true;
                }

                _timer.Start();
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public virtual void Stop()
        {
            lock (_timer)
            {
                _timer.Stop();

                _timer.Elapsed += OnTimerElapsed;
                _isSubscribed = false;
            }
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            RaiseEvent();
        }

        protected virtual void RaiseEvent()
        {
            var handler = Tick;
            if (handler is not null)
            {
                _dispatcherService.BeginInvoke(() => handler(this, EventArgs.Empty), OnlyBeginInvokeIfRequired);
            }
        }
    }
}
