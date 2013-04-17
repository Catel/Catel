// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogListenerBase.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    /// <summary>
    /// Abstract base class that implements the <see cref="ILogListener"/> interface.
    /// </summary>
    public abstract class LogListenerBase : ILogListener
    {
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
        }

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
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        void ILogListener.Write(ILog log, string message, LogEvent logEvent, object extraData)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Write(log, message, logEvent, extraData);
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        protected virtual void Write(ILog log, string message, LogEvent logEvent, object extraData)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void ILogListener.Debug(ILog log, string message, object extraData)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Debug(log, message, extraData);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        protected virtual void Debug(ILog log, string message, object extraData)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void ILogListener.Info(ILog log, string message, object extraData)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Info(log, message, extraData);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        protected virtual void Info(ILog log, string message, object extraData)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void ILogListener.Warning(ILog log, string message, object extraData)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Warning(log, message, extraData);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        protected virtual void Warning(ILog log, string message, object extraData)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void ILogListener.Error(ILog log, string message, object extraData)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Error(log, message, extraData);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        protected virtual void Error(ILog log, string message, object extraData)
        {
            // Empty by default
        }

        #region Obsolete members
        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        [ObsoleteEx(Replacement = "Write(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void ILogListener.Write(ILog log, string message, LogEvent logEvent)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }
            
            Write(log, message, logEvent);
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        [ObsoleteEx(Replacement = "Write(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        public virtual void Write(ILog log, string message, LogEvent logEvent)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Debug(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void ILogListener.Debug(ILog log, string message)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Debug(log, message);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Debug(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        public virtual void Debug(ILog log, string message)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Info(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void ILogListener.Info(ILog log, string message)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Info(log, message);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Info"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Info(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        public virtual void Info(ILog log, string message)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Warning(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void ILogListener.Warning(ILog log, string message)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Warning(log, message);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Warning(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        public virtual void Warning(ILog log, string message)
        {
            // Empty by default
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Error(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void ILogListener.Error(ILog log, string message)
        {
            if (IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            Error(log, message);
        }

        /// <summary>
        /// Called when a <see cref="LogEvent.Error"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Error(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        public virtual void Error(ILog log, string message)
        {
            // Empty by default
        }
        #endregion
    }
}
