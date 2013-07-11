// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using Logging;
    using System;

    /// <summary>
    /// Extensions to the <see cref="ILog" /> interface.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Debug(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Debug, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Debug(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Debug, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Info(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Info, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Info(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Info, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Warning(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Warning, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Warning(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Warning, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Error(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Error, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Error(this ILog log, Exception exception, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Error, exception, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, params object[] args)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            Argument.IsNotNull("log", log);
            Argument.IsNotNull("messageFormat", messageFormat);

            log.WriteWithData(string.Format(messageFormat, args), null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="log" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat" /> is <c>null</c>.</exception>
        public static void Write(this ILog log, LogEvent logEvent, Exception exception, string messageFormat, params object[] args)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            Argument.IsNotNull("log", log);
            Argument.IsNotNull("exception", exception);
            Argument.IsNotNull("messageFormat", messageFormat);

            log.WriteWithData(exception, string.Format(messageFormat, args), null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// Log.ErrorAndThrowException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="log"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageFormat"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static void ErrorAndThrowException<TException>(this ILog log, string messageFormat, params object[] args)
            where TException : Exception
        {
            Argument.IsNotNull("log", log);
            Argument.IsNotNull("messageFormat", messageFormat);

            var message = string.Format(messageFormat, args);

            log.ErrorWithData(message);

            Exception exception;

            try
            {
                exception = (Exception)Activator.CreateInstance(typeof(TException), message);
            }
#if !NETFX_CORE && !PCL
            catch (MissingMethodException)
#else
            catch (Exception)
#endif
            {
                string error = string.Format("Exception type '{0}' does not have a constructor accepting a string", typeof(TException).Name);
                log.Error(error);
                throw new NotSupportedException(error);
            }

            throw exception;
        }
    }
}