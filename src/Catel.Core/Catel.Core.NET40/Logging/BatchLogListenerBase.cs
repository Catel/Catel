﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchLogListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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

            _timer = new Timer(OnTimerTick, null, 5000, 5000);
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
        /// Formats the log event to a message which can be written to a log persistence storage.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <returns>The formatted log event.</returns>
        protected virtual string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData)
        {
            string logMessage = string.Format("{0} => [{1}] {2}", DateTime.Now.ToString("hh:mm:ss:fff"), logEvent.ToString().ToUpper(), message);
            return logMessage;
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData)
        {
            lock (_lock)
            {
                _logBatch.Add(new LogBatchEntry(log, message, logEvent, extraData));

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