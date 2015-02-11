﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogBatchEntry.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System;

    /// <summary>
    /// Represents a log entry inside a batch.
    /// </summary>
    public class LogBatchEntry : LogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogBatchEntry" /> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="time">The time.</param>
        public LogBatchEntry(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
            : base(log, message, logEvent, extraData, time)
        {
        }
    }
}