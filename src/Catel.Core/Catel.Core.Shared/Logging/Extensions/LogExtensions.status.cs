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
            if (!LogManager.LogInfo.IsStatusEnabled)
            {
                return;
            }

            Write(log, LogEvent.Status, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting arguments.</param>
        public static void Status(this ILog log, string messageFormat, object s1)
        {
            if (!LogManager.LogInfo.IsStatusEnabled)
            {
                return;
            }

            Write(log, LogEvent.Status, messageFormat, s1);
        }

        /// <summary>
        /// Writes the specified message as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        public static void Status(this ILog log, string messageFormat, object s1, object s2)
        {
            if (!LogManager.LogInfo.IsStatusEnabled)
            {
                return;
            }

            Write(log, LogEvent.Status, messageFormat, s1, s2);
        }

        /// <summary>
        /// Writes the specified message as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        public static void Status(this ILog log, string messageFormat, object s1, object s2, object s3)
        {
            if (!LogManager.LogInfo.IsStatusEnabled)
            {
                return;
            }

            Write(log, LogEvent.Status, messageFormat, s1, s2, s3);
        }

        /// <summary>
        /// Writes the specified message as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        /// <param name="s4">The formatting argument 4.</param>
        public static void Status(this ILog log, string messageFormat, object s1, object s2, object s3, object s4)
        {
            if (!LogManager.LogInfo.IsStatusEnabled)
            {
                return;
            }

            Write(log, LogEvent.Status, messageFormat, s1, s2, s3, s4);
        }

        /// <summary>
        /// Writes the specified message as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="s1">The formatting argument 1.</param>
        /// <param name="s2">The formatting argument 2.</param>
        /// <param name="s3">The formatting argument 3.</param>
        /// <param name="s4">The formatting argument 4.</param>
        /// <param name="s5">The formatting argument 5.</param>
        /// <param name="others">The formatting arguments.</param>
        public static void Status(this ILog log, string messageFormat, object s1, object s2, object s3, object s4, object s5, params object[] others)
        {
            if (!LogManager.LogInfo.IsStatusEnabled)
            {
                return;
            }

            Write(log, LogEvent.Status, messageFormat, s1, s2, s3, s4, s5, others);
        }


        /// <summary>
        /// Writes the specified message as status message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Status(this ILog log, string messageFormat, params object[] args)
        {
            if (!LogManager.LogInfo.IsStatusEnabled)
            {
                return;
            }

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