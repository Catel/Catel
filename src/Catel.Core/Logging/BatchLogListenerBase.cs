namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for log listeners that can write in batches.
    /// </summary>
    public abstract class BatchLogListenerBase : LogListenerBase, IBatchLogListener
    {
        private readonly object _lock = new object();

#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly Timer _timer;
#pragma warning restore IDISP006 // Implement IDisposable.
        private TimeSpan _timerInterval;
        private List<LogBatchEntry> _logBatch = new List<LogBatchEntry>();

        private bool _isFlushing;
        private bool _needsFlushing;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchLogListenerBase" /> class.
        /// </summary>
        /// <param name="maxBatchCount">The maximum batch count.</param>
        public BatchLogListenerBase(int maxBatchCount = 100)
            : this(TimeSpan.FromSeconds(5), maxBatchCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchLogListenerBase" /> class.
        /// </summary>
        /// <param name="interval">The interval between the auto-flushes.</param>
        /// <param name="maxBatchCount">The maximum batch count.</param>
        public BatchLogListenerBase(TimeSpan interval, int maxBatchCount = 100)
        {
            _timer = new Timer(OnTimerTick);

            MaximumBatchCount = maxBatchCount;
            Interval = interval;
        }

        /// <summary>
        /// Gets the maximum batch count.
        /// </summary>
        /// <value>
        /// The maximum batch count.
        /// </value>
        public int MaximumBatchCount { get; private set; }

        /// <summary>
        /// Gets the interval between the automatic flushes.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        protected TimeSpan Interval
        {
            get { return _timerInterval; }
            set
            {
                _timerInterval = value;
                _timer.Change(value, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the listener should be flushed, even when the batch is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the listener should be flushed, even when the batch is empty; otherwise, <c>false</c>.
        /// </value>
        internal bool FlushWhenBatchIsEmpty { get; set; }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object? extraData, LogData? logData, DateTime time)
        {
            var logEntry = new LogBatchEntry(log, message, logEvent, extraData, logData, time);

            lock (_lock)
            {
                _logBatch.Add(logEntry);

                if (_logBatch.Count >= MaximumBatchCount)
                {
#pragma warning disable 4014
                    FlushAsync();
#pragma warning restore 4014
                }
            }
        }

        private bool RequiresNewFlush()
        {
            lock (_lock)
            {
                return _needsFlushing;
            }
        }

        private void OnTimerTick(object? state)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            FlushAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Flushes the current queue asynchronous.
        /// </summary>
        /// <returns>Task so it can be awaited.</returns>
        public async Task FlushAsync()
        {
            List<LogBatchEntry>? batchToSubmit = null;

            lock (_lock)
            {
                _needsFlushing = false;

                if (_isFlushing)
                {
                    // Note: in an ideal world, we would create a _tcs and await that first, but that could make the await too long,
                    // so for now we will just return. We might improve this implementation in the future if need be.
                    _needsFlushing = true;
                    return;
                }

                if (_logBatch.Count > 0 || FlushWhenBatchIsEmpty)
                {
                    _isFlushing = true;

                    batchToSubmit = _logBatch;
                    _logBatch = new List<LogBatchEntry>();
                }
            }

            if (batchToSubmit is not null)
            {
                try
                {
                    await WriteBatchAsync(batchToSubmit);
                }
                finally
                {
                    lock (_lock)
                    {
                        _isFlushing = false;
                    }
                }

                if (RequiresNewFlush())
                {
                    // Note: don't await, this is actually another call which doesn't belong to the caller
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    FlushAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }

        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        /// <returns>Task so this can be done asynchronously.</returns>
        protected virtual Task WriteBatchAsync(List<LogBatchEntry> batchEntries)
        {
            return Task.CompletedTask;
        }
    }
}
