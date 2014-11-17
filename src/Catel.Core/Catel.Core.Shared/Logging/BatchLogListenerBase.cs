// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchLogListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base class for log listeners that can write in batches.
    /// </summary>
    public abstract class BatchLogListenerBase : LogListenerBase, IBatchLogListener
    {
        #region Fields
        private readonly object _lock = new object();
        private readonly object _flushLock = new object();
        private int _flushCount;

        private readonly Timer _timer;
        private List<LogBatchEntry> _logBatch = new List<LogBatchEntry>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchLogListenerBase" /> class.
        /// </summary>
        /// <param name="maxBatchCount">The maximum batch count.</param>
        public BatchLogListenerBase(int maxBatchCount = 100)
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
#pragma warning disable 4014
                    Flush();
#pragma warning restore 4014
                }
            }
        }

        private void OnTimerTick(object state)
        {
            lock (_lock)
            {
                if (_logBatch.Count > 0)
                {
#pragma warning disable 4014
                    Flush();
#pragma warning restore 4014
                }
            }
        }

        /// <summary>
        /// Flushes the current queue asynchronous.
        /// </summary>
        /// <returns>Task so it can be awaited.</returns>
        public async Task Flush()
        {
            List<LogBatchEntry> batchToSubmit;

            lock (_lock)
            {
                batchToSubmit = _logBatch;

                _logBatch = new List<LogBatchEntry>();
            }

            await Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    lock (_flushLock)
                    {
                        if (_flushCount == 0)
                        {
                            _flushCount++;
                            return;
                        }
                    }

                    ThreadHelper.Sleep(10);
                }
            });

            if (batchToSubmit.Count > 0)
            {
                await WriteBatch(batchToSubmit);
            }

            lock (_flushLock)
            {
                _flushCount--;
            }
        }

        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        /// <returns>Task so this can be done asynchronously.</returns>
        protected abstract Task WriteBatch(List<LogBatchEntry> batchEntries);
        #endregion
    }
}