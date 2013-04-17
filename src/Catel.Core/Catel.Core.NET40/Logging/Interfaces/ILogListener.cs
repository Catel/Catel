// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogListener.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    /// <summary>
    /// Interface allowing external subscribers for the logging.
    /// </summary>
    public interface ILogListener
    {
        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in debug messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this listener is interested in debug messages; otherwise, <c>false</c>.
        /// </value>
        bool IsDebugEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in info messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this listener is interested in info messages; otherwise, <c>false</c>.
        /// </value>
        bool IsInfoEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in warning messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this listener is interested in warning messages; otherwise, <c>false</c>.
        /// </value>
        bool IsWarningEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this listener is interested in error messages.
        /// <para />
        /// This default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this listener is interested in error messages; otherwise, <c>false</c>.
        /// </value>
        bool IsErrorEnabled { get; set; }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        [ObsoleteEx(Replacement = "Write(ILog, string, LogEvent, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void Write(ILog log, string message, LogEvent logEvent);

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        void Write(ILog log, string message, LogEvent logEvent, object extraData);

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Debug(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void Debug(ILog log, string message);

        /// <summary>
        /// Called when a <see cref="LogEvent.Debug" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void Debug(ILog log, string message, object extraData);

        /// <summary>
        /// Called when a <see cref="LogEvent.Info"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Info(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void Info(ILog log, string message);

        /// <summary>
        /// Called when a <see cref="LogEvent.Info" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void Info(ILog log, string message, object extraData);

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Warning(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void Warning(ILog log, string message);

        /// <summary>
        /// Called when a <see cref="LogEvent.Warning" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void Warning(ILog log, string message, object extraData);

        /// <summary>
        /// Called when a <see cref="LogEvent.Error"/> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        [ObsoleteEx(Replacement = "Error(ILog, string, object)", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0")]
        void Error(ILog log, string message);

        /// <summary>
        /// Called when a <see cref="LogEvent.Error" /> message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The additional data.</param>
        void Error(ILog log, string message, object extraData);
    }
}