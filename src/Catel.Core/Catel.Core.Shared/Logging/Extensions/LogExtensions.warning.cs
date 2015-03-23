// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensions.info.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System;

    public static partial class LogExtensions
    {
        /// <summary>
        /// Writes an empty line as warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void Warning(this ILog log)
        {
            Write(log, LogEvent.Warning, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Warning(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Warning, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Warning(this ILog log, Exception exception)
        {
            Write(log, LogEvent.Warning, exception, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Warning(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Warning, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as warning message with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void WarningWithData(this ILog log, Exception exception, string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsWarningEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            log.WarningWithData(FormatException(exception, message), extraData);
        }
    }
}