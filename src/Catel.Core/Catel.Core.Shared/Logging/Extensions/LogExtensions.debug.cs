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
        /// Writes an empty line as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void Debug(this ILog log)
        {
            Write(log, LogEvent.Debug, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The format argument 1</param>
        public static void Debug(this ILog log, LogEvent logEvent, string messageFormat, object s1)
        {
            Write(log, LogEvent.Debug, messageFormat, s1);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The format argument 1</param>
        /// <param name="s2">The format argument 2</param>
        public static void Debug(this ILog log, string messageFormat, object s1, object s2)
        {
            Write(log, LogEvent.Debug, messageFormat, s1, s2);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The format argument 1</param>
        /// <param name="s2">The format argument 2</param>
        /// <param name="s3">The format argument 3</param>
        public static void Debug(this ILog log, string messageFormat, object s1, object s2, object s3)
        {
            Write(log, LogEvent.Debug, messageFormat, s1, s2, s3);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The format argument 1</param>
        /// <param name="s2">The format argument 2</param>
        /// <param name="s3">The format argument 3</param>
        /// <param name="s4">The format argument 4</param>
        public static void Debug(this ILog log, string messageFormat, object s1, object s2, object s3, object s4)
        {
            Write(log, LogEvent.Debug, messageFormat, s1, s2, s3, s4);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The format argument 1</param>
        /// <param name="s2">The format argument 2</param>
        /// <param name="s3">The format argument 3</param>
        /// <param name="s4">The format argument 4</param>
        /// <param name="s5">The format argument 5</param>
        /// <param name="others">The othersm format arguments</param>
        public static void Debug(this ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others)
        {
            Write(log, LogEvent.Debug, messageFormat, s1,s2,s3,s4, s5, others);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Debug(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Debug, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Debug(this ILog log, Exception exception)
        {
            Write(log, LogEvent.Debug, exception, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Debug(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Debug, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as debug message with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        public static void DebugWithData(this ILog log, string message, object extraData = null)
        {
            log.WriteWithData(message, extraData, LogEvent.Debug);
        }

        /// <summary>
        /// Writes the specified message as debug message with log data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logData">The log data.</param>
        public static void DebugWithData(this ILog log, string message, LogData logData)
        {
            log.WriteWithData(message, logData, LogEvent.Debug);
        }

        /// <summary>
        /// Writes the specified message as debug message with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void DebugWithData(this ILog log, Exception exception, string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsDebugEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            log.DebugWithData(FormatException(exception, message), extraData);
        }
    }
}