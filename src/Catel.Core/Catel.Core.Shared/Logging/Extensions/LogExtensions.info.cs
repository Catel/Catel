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
        /// Writes an empty line as info message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void Info(this ILog log)
        {
            Write(log, LogEvent.Info, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Info(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Info, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Info(this ILog log, Exception exception)
        {
            Write(log, LogEvent.Info, exception, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public static void Info(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Info, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as info message with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void InfoWithData(this ILog log, Exception exception, string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsInfoEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            log.InfoWithData(FormatException(exception, message), extraData);
        }
    }
}