// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract base class that implements the <see cref="ILogListener"/> interface.
    /// </summary>
    public abstract class LogListenerBase : ILogListener
    {
        #region Constants
        /// <summary>
        /// The log event strings.
        /// </summary>
        protected static readonly Dictionary<LogEvent, string> LogEventStrings;
        #endregion

        #region Fields
        private TimeDisplay _timeDisplay;
        private string _timeFormat;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="LogListenerBase"/> class.
        /// </summary>
        static LogListenerBase()
        {
            LogEventStrings = new Dictionary<LogEvent, string>();
            foreach (var enumValue in Enum<LogEvent>.GetValues())
            {
                LogEventStrings[enumValue] = enumValue.ToString().ToUpper();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogListenerBase"/> class.
        /// </summary>
        /// <param name="ignoreCatelLogging">if set to <c>true</c>, the internal logging of Catel will be ignored.</param>
        protected LogListenerBase(bool ignoreCatelLogging = false)
        {
            IsDebugEnabled = true;
            IsInfoEnabled = true;
            IsWarningEnabled = true;
            IsErrorEnabled = true;

            IgnoreCatelLogging = ignoreCatelLogging;

            TimeDisplay = TimeDisplay.Time;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a log message is written to one of the logs.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessage;
        #endregion

        #region ILogListener Members
        /// <summary>
        /// Gets or sets a value indicating whether to ignore Catel logging.
        /// </summary>
        /// <value><c>true</c> if Catel logging should be ignored; otherwise, <c>false</c>.</value>
        public bool IgnoreCatelLogging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in debug messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this listener is interested in debug messages; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in info messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this listener is interested in info messages; otherwise, <c>false</c>.
        /// </value>
        public bool IsInfoEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in warning messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this listener is interested in warning messages; otherwise, <c>false</c>.
        /// </value>
        public bool IsWarningEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in error messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if this listener is interested in error messages; otherwise, <c>false</c>.
        /// </value>
        public bool IsErrorEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating what format of time to use.
        /// <para />
        /// This default value is <c>Time</c>.
        /// </summary>
        public TimeDisplay TimeDisplay
        {
            get { return _timeDisplay; }
            set
            {
                _timeDisplay = value;

                switch (_timeDisplay)
                {
                    case TimeDisplay.Time:
                        _timeFormat = "HH:mm:ss:fff";
                        break;

                    case TimeDisplay.DateTime:
                        _timeFormat = "yyyy-MM-dd HH:mm:ss:fff";
                        break;

                    default:
                        _timeFormat = "HH:mm:ss:fff";
                        break;
                }
            }
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Use overload with logData", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        void ILogListener.Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            ((ILogListener)this).Write(log, message, logEvent, extraData, null, time);
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            if (ShouldIgnoreLogMessage(log, message, logEvent, extraData, logData, time))
            {
                return;
            }

            Write(log, message, logEvent, extraData, time);
            Write(log, message, logEvent, extraData, logData, time);

            RaiseLogMessage(log, message, logEvent, extraData, logData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Use overload with logData", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        void ILogListener.Debug(ILog log, string message, object extraData, DateTime time)
        {
            ((ILogListener)this).Debug(log, message, extraData, null, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Debug(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            if (ShouldIgnoreLogMessage(log, message, LogEvent.Debug, extraData, logData, time))
            {
                return;
            }

            Debug(log, message, extraData, time);
            Debug(log, message, extraData, logData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Use overload with logData", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        void ILogListener.Info(ILog log, string message, object extraData, DateTime time)
        {
            ((ILogListener)this).Info(log, message, extraData, null, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Info(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            if (ShouldIgnoreLogMessage(log, message, LogEvent.Info, extraData, logData, time))
            {
                return;
            }

            Info(log, message, extraData, time);
            Info(log, message, extraData, logData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Use overload with logData", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        void ILogListener.Warning(ILog log, string message, object extraData, DateTime time)
        {
            ((ILogListener)this).Warning(log, message, extraData, null, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Warning(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            if (ShouldIgnoreLogMessage(log, message, LogEvent.Warning, extraData, logData, time))
            {
                return;
            }

            Warning(log, message, extraData, time);
            Warning(log, message, extraData, logData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Use overload with logData", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        void ILogListener.Error(ILog log, string message, object extraData, DateTime time)
        {
            ((ILogListener)this).Error(log, message, extraData, null, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The ti me.</param>
        void ILogListener.Error(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            if (ShouldIgnoreLogMessage(log, message, LogEvent.Error, extraData, logData, time))
            {
                return;
            }

            Error(log, message, extraData, time);
            Error(log, message, extraData, logData, time);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns whether the log message should be ignored
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="time">The time.</param>
        /// <returns><c>true</c> if the message should be ignored, <c>false</c> otherwise.</returns>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual bool ShouldIgnoreLogMessage(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            return false;
        }

        /// <summary>
        /// Returns whether the log message should be ignored
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        /// <returns><c>true</c> if the message should be ignored, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldIgnoreLogMessage(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            // Should be removed in v6
            if (ShouldIgnoreLogMessage(log, message, logEvent, extraData, time))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="LogMessage" /> event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected void RaiseLogMessage(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            RaiseLogMessage(log, message, logEvent, extraData, null, time);
        }

        /// <summary>
        /// Raises the <see cref="LogMessage" /> event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected void RaiseLogMessage(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogMessageEventArgs(log, message, extraData, logData, logEvent, time));
            }
        }

        /// <summary>
        /// Formats the log event to a message which can be written to a log persistence storage.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="time">The time.</param>
        /// <returns>The formatted log event.</returns>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            return FormatLogEvent(log, message, logEvent, extraData, null, time);
        }

        /// <summary>
        /// Formats the log event to a message which can be written to a log persistence storage.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        /// <returns>The formatted log event.</returns>
        protected virtual string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            var logMessage = string.Format("{0} => [{1}] [{2}] [{3}] {4}", time.ToString(_timeFormat), LogEventStrings[logEvent], log.TargetType.FullName, ThreadHelper.GetCurrentThreadId(), message);
            return logMessage;
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual void Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected virtual void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual void Debug(ILog log, string message, object extraData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected virtual void Debug(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual void Info(ILog log, string message, object extraData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected virtual void Info(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual void Warning(ILog log, string message, object extraData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected virtual void Warning(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        [ObsoleteEx(Replacement = "Overload with logData parameter", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual void Error(ILog log, string message, object extraData, DateTime time)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected virtual void Error(ILog log, string message, object extraData, LogData logData, DateTime time)
        {
            // Empty by default
        }
        #endregion
    }
}