// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



#if NET

namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using Reflection;

    /// <summary>
    /// Log listener which writes all data to the system event log.
    /// </summary>
    public class EventLogListener : BatchLogListenerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogListener"/> class.
        /// </summary>
        public EventLogListener()
        {
            var assembly = AssemblyHelper.GetEntryAssembly();

            Source = assembly.Title();
            LogName = "Application";
            MachineName = Dns.GetHostName();

            var sourceData = new EventSourceCreationData(Source, LogName)
            {
                MachineName = MachineName
            };

            if (!EventLog.SourceExists(Source, MachineName))
            {
                EventLog.CreateEventSource(sourceData);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the source name to register and use when writing to the event log.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets the name of the computer on which to write events.
        /// </summary>
        public string MachineName { get; private set; }

        /// <summary>
        /// Gets the name of the event log to which the source writes entries.
        /// </summary>
        public string LogName { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Formats the log event to a message which can be written to a log persistence storage.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="time">The time.</param>
        /// <returns>The formatted log event.</returns>
        protected override string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            var logMessage = string.Format("[{0}] {1}", log.TargetType.FullName, message);
            return logMessage;
        }

        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        /// <returns>Task so this can be done asynchronously.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="batchEntries"/> is <c>null</c>.</exception>
        protected override async Task WriteBatch(List<LogBatchEntry> batchEntries)
        {
            Argument.IsNotNull("batchEntries", batchEntries);

            try
            {
                foreach (var batchEntry in batchEntries)
                {
                    var type = ChooseEventLogEntryType(batchEntry.LogEvent);

                    var message = FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData, batchEntry.Time);

                    var entry = batchEntry;
                    await Task.Factory.StartNew(() => EventLog.WriteEntry(Source, message, type, (int) entry.LogEvent));
                }
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        private static EventLogEntryType ChooseEventLogEntryType(LogEvent logEvent)
        {
            switch (logEvent)
            {
                case LogEvent.Debug:
                case LogEvent.Info:
                    return EventLogEntryType.Information;

                case LogEvent.Warning:
                    return EventLogEntryType.Warning;

                case LogEvent.Error:
                    return EventLogEntryType.Error;

                default:
                    return EventLogEntryType.Information;
            }
        }
        #endregion
    }
}

#endif