// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryLogService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using Logging;

    /// <summary>
    /// Rolling in memory log service.
    /// </summary>
    public class RollingInMemoryLogService : ServiceBase, IRollingInMemoryLogService
    {
        private readonly RollingInMemoryLogListener _rollingInMemoryLogListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="RollingInMemoryLogService" /> class.
        /// </summary>
        public RollingInMemoryLogService()
        {
            _rollingInMemoryLogListener = new RollingInMemoryLogListener();
            _rollingInMemoryLogListener.LogMessage += OnLogListenerLogMessage;

            LogManager.AddListener(_rollingInMemoryLogListener);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the maximum number of log entries to keep.
        /// <para />
        /// The default value is 250.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        public int MaximumNumberOfLogEntries
        {
            get { return _rollingInMemoryLogListener.MaximumNumberOfLogEntries; }
            set { _rollingInMemoryLogListener.MaximumNumberOfLogEntries = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of warning log entries to keep.
        /// <para />
        /// The default value is 50.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        public int MaximumNumberOfWarningLogEntries
        {
            get { return _rollingInMemoryLogListener.MaximumNumberOfWarningLogEntries; }
            set { _rollingInMemoryLogListener.MaximumNumberOfWarningLogEntries = value; }
        }

        /// <summary>
        /// Gets or sets the maximum number of error log entries to keep.
        /// <para />
        /// The default value is 50.
        /// </summary>
        /// <value>The maximum number of log entries.</value>
        public int MaximumNumberOfErrorLogEntries
        {
            get { return _rollingInMemoryLogListener.MaximumNumberOfErrorLogEntries; }
            set { _rollingInMemoryLogListener.MaximumNumberOfErrorLogEntries = value; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a log message is written.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessage;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        public IEnumerable<LogEntry> GetLogEntries()
        {
            return _rollingInMemoryLogListener.GetLogEntries();
        }

        /// <summary>
        /// Gets the warning log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        public IEnumerable<LogEntry> GetWarningLogEntries()
        {
            return _rollingInMemoryLogListener.GetWarningLogEntries();
        }

        /// <summary>
        /// Gets the error log entries.
        /// </summary>
        /// <returns>IEnumerable&lt;LogEntry&gt;.</returns>
        public IEnumerable<LogEntry> GetErrorLogEntries()
        {
            return _rollingInMemoryLogListener.GetErrorLogEntries();
        }

        private void OnLogListenerLogMessage(object sender, LogMessageEventArgs e)
        {
            LogMessage.SafeInvoke(this, e);
        }
        #endregion
    }
}