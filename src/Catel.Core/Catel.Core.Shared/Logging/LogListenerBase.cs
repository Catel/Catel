// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
                        _timeFormat = "hh:mm:ss:fff";
                        break;
                    case TimeDisplay.DateTime:
                        _timeFormat = "yyyy-MM-dd hh:mm:ss:fff";
                        break;
                    default:
                        _timeFormat = "hh:mm:ss:fff";
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
        void ILogListener.Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Write(log, message, logEvent, extraData, time);

            RaiseLogMessage(log, message, logEvent, extraData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Debug(ILog log, string message, object extraData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Debug(log, message, extraData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Info(ILog log, string message, object extraData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Info(log, message, extraData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Warning(ILog log, string message, object extraData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Warning(log, message, extraData, time);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        void ILogListener.Error(ILog log, string message, object extraData, DateTime time)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Error(log, message, extraData, time);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the <see cref="LogMessage" /> event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="time">The time.</param>
        protected void RaiseLogMessage(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            var handler = LogMessage;
            if (handler != null)
            {
                handler(this, new LogMessageEventArgs(log, message, extraData, logEvent, time));
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
        protected virtual string FormatLogEvent(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            string logMessage = string.Format("{0} => [{1}] [{2}] [{3}] {4}", ThreadHelper.GetCurrentThreadId(), time.ToString(_timeFormat), LogEventStrings[logEvent], log.TargetType.FullName, message);
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
        protected virtual void Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
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
        protected virtual void Debug(ILog log, string message, object extraData, DateTime time)
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
        protected virtual void Info(ILog log, string message, object extraData, DateTime time)
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
        protected virtual void Warning(ILog log, string message, object extraData, DateTime time)
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
        protected virtual void Error(ILog log, string message, object extraData, DateTime time)
        {
            // Empty by default
        }
        #endregion

        /// <summary>
        /// Occurs when a log message is written to one of the logs.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessage;
    }
}