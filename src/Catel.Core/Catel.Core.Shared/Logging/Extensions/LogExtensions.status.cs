// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensions.status.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    public static partial class LogExtensions
    {
        /// <summary>
        /// Writes an empty line as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void Status(this ILog log)
        {
            Write(log, LogEvent.Status, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Status(this ILog log, string messageFormat, params object[] args)
        {
            Write(log, LogEvent.Status, messageFormat, args);
        }

        /// <summary>
        /// Writes an empty line as debug and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void DebugAndStatus(this ILog log)
        {
            Debug(log);
            Status(log);
        }

        /// <summary>
        /// Writes the specified message as debug and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void DebugAndStatus(this ILog log, string messageFormat, params object[] args)
        {
            Debug(log, messageFormat, args);
            Status(log, messageFormat, args);
        }

        /// <summary>
        /// Writes an empty line as info and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void InfoAndStatus(this ILog log)
        {
            Info(log);
            Status(log);
        }

        /// <summary>
        /// Writes the specified message as info and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void InfoAndStatus(this ILog log, string messageFormat, params object[] args)
        {
            Info(log, messageFormat, args);
            Status(log, messageFormat, args);
        }

        /// <summary>
        /// Writes an empty line as warning and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void WarningAndStatus(this ILog log)
        {
            Warning(log);
            Status(log);
        }

        /// <summary>
        /// Writes the specified message as warning and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void WarningAndStatus(this ILog log, string messageFormat, params object[] args)
        {
            Warning(log, messageFormat, args);
            Status(log, messageFormat, args);
        }

        /// <summary>
        /// Writes an empty line as error and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void ErrorAndStatus(this ILog log)
        {
            Error(log);
            Status(log);
        }

        /// <summary>
        /// Writes the specified message as error and status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void ErrorAndStatus(this ILog log, string messageFormat, params object[] args)
        {
            Error(log, messageFormat, args);
            Status(log, messageFormat, args);
        }
    }
}