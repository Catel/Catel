namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using Logging;

    /// <summary>
    /// In memory log listener that keeps track of the latest log messages.
    /// </summary>
    public class RollingInMemoryLogListener : LogListenerBase
    {
        private readonly object _lock = new object();

        private readonly List<LogEntry> _lastLogEntries = new List<LogEntry>();
        private readonly List<LogEntry> _lastWarningLogEntries = new List<LogEntry>();
        private readonly List<LogEntry> _lastErrorLogEntries = new List<LogEntry>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingInMemoryLogListener"/> class.
        /// </summary>
        public RollingInMemoryLogListener()
        {
            MaximumNumberOfLogEntries = 250;
            MaximumNumberOfWarningLogEntries = 50;
            MaximumNumberOfErrorLogEntries = 50;
        }

        /// <summary>
        /// Gets or sets the maximum number of log entries to keep.
        /// <para />
        /// The default value is 250.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        public int MaximumNumberOfLogEntries { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of warning log entries to keep.
        /// <para />
        /// The default value is 50.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        public int MaximumNumberOfWarningLogEntries { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of error log entries to keep.
        /// <para />
        /// The default value is 50.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        public int MaximumNumberOfErrorLogEntries { get; set; }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            base.Write(log, message, logEvent, extraData, logData, time);

            var logEntry = new LogEntry(log, message, logEvent, extraData, null, time);

            AddLogEvent(_lastLogEntries, logEntry, MaximumNumberOfLogEntries);

            switch (logEvent)
            {
                case LogEvent.Warning:
                    AddLogEvent(_lastWarningLogEntries, logEntry, MaximumNumberOfWarningLogEntries);
                    break;

                case LogEvent.Error:
                    AddLogEvent(_lastErrorLogEntries, logEntry, MaximumNumberOfErrorLogEntries);
                    break;
            }
        }

        private void AddLogEvent(List<LogEntry> collection, LogEntry logEntry, int maximumEntries)
        {
            lock (_lock)
            {
                collection.Add(logEntry);

                while (collection.Count > maximumEntries)
                {
                    collection.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Gets the log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        public IEnumerable<LogEntry> GetLogEntries()
        {
            lock (_lock)
            {
                return _lastLogEntries.ToArray();
            }
        }

        /// <summary>
        /// Gets the warning log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        public IEnumerable<LogEntry> GetWarningLogEntries()
        {
            lock (_lock)
            {
                return _lastWarningLogEntries.ToArray();
            }
        }

        /// <summary>
        /// Gets the error log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        public IEnumerable<LogEntry> GetErrorLogEntries()
        {
            lock (_lock)
            {
                return _lastErrorLogEntries.ToArray();
            }
        }
    }
}
