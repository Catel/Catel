// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogBatchEntry.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    /// <summary>
    /// Represents a log entry inside a batch.
    /// </summary>
    public class LogBatchEntry
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogBatchEntry" /> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        public LogBatchEntry(ILog log, string message, LogEvent logEvent, object extraData)
        {
            Log = log;
            Message = message;
            LogEvent = logEvent;
            ExtraData = extraData;
        }
        #endregion

        #region Properties
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
        #endregion
    }
}