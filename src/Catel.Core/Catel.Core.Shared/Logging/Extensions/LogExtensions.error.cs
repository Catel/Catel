// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensions.info.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;

    public static partial class LogExtensions
    {
        /// <summary>
        /// Writes an empty line as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void Error(this ILog log)
        {
            Write(log, LogEvent.Error, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Error(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Error, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Error(this ILog log, Exception exception)
        {
            Write(log, LogEvent.Error, exception, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Error(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Error, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        public static void ErrorWithData(this ILog log, string message, object extraData = null)
        {
            log.WriteWithData(message, extraData, LogEvent.Error);
        }

        /// <summary>
        /// Writes the specified message as error message with log data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logData">The log data.</param>
        public static void ErrorWithData(this ILog log, string message, LogData logData)
        {
            log.WriteWithData(message, logData, LogEvent.Error);
        }

        /// <summary>
        /// Writes the specified message as error message with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void ErrorWithData(this ILog log, Exception exception, string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            log.ErrorWithData(FormatException(exception, message), extraData);
        }
    }
}