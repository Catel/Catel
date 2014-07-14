// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchLogListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Base class for log listeners that can write in batches.
    /// </summary>
    public abstract class BatchLogListenerBase : LogListenerBase, IBatchLogListener
    {
        #region Fields
        private readonly object _lock = new object();

        private readonly Timer _timer;
        private List<LogBatchEntry> _logBatch = new List<LogBatchEntry>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchLogListenerBase" /> class.
        /// </summary>
        /// <param name="maxBatchCount">The maximum batch count.</param>
        public BatchLogListenerBase(int maxBatchCount = 50)
        {
            MaximumBatchCount = maxBatchCount;

            var interval = TimeSpan.FromSeconds(5);
            _timer = new Timer(OnTimerTick, null, interval, interval);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the maximum batch count.
        /// </summary>
        /// <value>
        /// The maximum batch count.
        /// </value>
        public int MaximumBatchCount { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            lock (_lock)
            {
                _logBatch.Add(new LogBatchEntry(log, message, logEvent, extraData, time));

                if (_logBatch.Count >= MaximumBatchCount)
                {
                    Flush();
                }
            }
        }

        private void OnTimerTick(object state)
        {
            lock (_lock)
            {
                if (_logBatch.Count > 0)
                {
                    Flush();
                }
            }
        }

        /// <summary>
        /// Flushes the current queue.
        /// </summary>
        public void Flush()
        {
            List<LogBatchEntry> batchToSubmit;

            lock (_lock)
            {
                batchToSubmit = _logBatch;

                _logBatch = new List<LogBatchEntry>();
            }

            if (batchToSubmit.Count > 0)
            {
                WriteBatch(batchToSubmit);
            }
        }

        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        protected abstract void WriteBatch(List<LogBatchEntry> batchEntries);
        #endregion
    }
}