// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEntry.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;

    /// <summary>
    /// Log entry class.
    /// </summary>
    public class LogEntry
    {
        private LogData _logData;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry" /> class.
        /// </summary>
        /// <param name="eventArgs">The event args.</param>
        public LogEntry(LogMessageEventArgs eventArgs)
            : this(eventArgs.Log, eventArgs.Message, eventArgs.LogEvent, eventArgs.ExtraData, eventArgs.LogData, eventArgs.Time)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry" /> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        public LogEntry(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            Time = time;
            Log = log;
            Message = message;
            LogEvent = logEvent;
            ExtraData = extraData;

            _logData = logData;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Gets the extra data.
        /// </summary>
        /// <value>
        /// The extra data.
        /// </value>
        public object ExtraData { get; private set; }

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public ILog Log { get; private set; }

        /// <summary>
        /// Gets the log event.
        /// </summary>
        /// <value>
        /// The log event.
        /// </value>
        public LogEvent LogEvent { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the log data attached to this log entry.
        /// </summary>
        /// <value>The data.</value>
        public LogData Data
        {
            get
            {
                if (_logData == null)
                {
                    _logData = new LogData();
                }

                return _logData;
            }
        }

        /// <summary>
        /// Returns the string value of this object.
        /// </summary>
        /// <returns>String value.</returns>
        public override string ToString()
        {
            return $"[{Time}] [{LogEvent}] {Message}";
        }
        #endregion
    }
}