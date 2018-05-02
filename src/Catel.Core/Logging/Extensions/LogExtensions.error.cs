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
        /// Writes an empty line as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void Error(this ILog log)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            Write(log, LogEvent.Error, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting arguments.</param>
        public static void Error(this ILog log, string messageFormat, object s1)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            Write(log, LogEvent.Error, messageFormat, s1);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The .</param>
        /// <param name="s2"></param>
        public static void Error(this ILog log, string messageFormat, object s1, object s2)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            Write(log, LogEvent.Error, messageFormat, s1, s2);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        public static void Error(this ILog log, string messageFormat, object s1, object s2, object s3)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            Write(log, LogEvent.Error, messageFormat, s1, s2, s3);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        /// <param name="s4">The formatting argument 4.</param>
        public static void Error(this ILog log, string messageFormat, object s1, object s2, object s3, object s4)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            Write(log, LogEvent.Error, messageFormat, s1, s2, s3, s4);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        /// <param name="s4">The formatting argument 4.</param>
        /// <param name="s5">The formatting argument 5.</param>
        /// <param name="others">The formatting arguments.</param>
        public static void Error(this ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            Write(log, LogEvent.Error, messageFormat, s1, s2, s3, s4, s5, others);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Error(this ILog log, string messageFormat, params object[] args)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

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
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

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
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

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
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

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
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

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
            log.ErrorWithData(FormatException(exception, message), extraData);
        }
    }
}