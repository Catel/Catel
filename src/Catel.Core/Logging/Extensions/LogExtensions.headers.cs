namespace Catel.Logging
{
    public static partial class LogExtensions
    {
        private const string Heading1 = "=======================================================================";
        private const string Heading2 = "-----------------------------------------------------------------------";
        private const string Heading3 = "";

        /// <summary>
        /// Logs a heading as a debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogDebugHeading1(this ILog log, string messageFormat, params object[] args)
        {
            LogDebugHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogDebugHeading2(this ILog log, string messageFormat, params object[] args)
        {
            LogDebugHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogDebugHeading3(this ILog log, string messageFormat, params object[] args)
        {
            LogDebugHeading(log, Heading3, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="headingContent">Content of the heading.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogDebugHeading(this ILog log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogEvent.Debug, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInfoHeading1(this ILog log, string messageFormat, params object[] args)
        {
            LogInfoHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInfoHeading2(this ILog log, string messageFormat, params object[] args)
        {
            LogInfoHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInfoHeading3(this ILog log, string messageFormat, params object[] args)
        {
            LogInfoHeading(log, Heading3, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="headingContent">Content of the heading.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInfoHeading(this ILog log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogEvent.Info, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningHeading1(this ILog log, string messageFormat, params object[] args)
        {
            LogWarningHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningHeading2(this ILog log, string messageFormat, params object[] args)
        {
            LogWarningHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningHeading3(this ILog log, string messageFormat, params object[] args)
        {
            LogWarningHeading(log, Heading3, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="headingContent">Content of the heading.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningHeading(this ILog log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogEvent.Warning, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorHeading1(this ILog log, string messageFormat, params object[] args)
        {
            LogErrorHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorHeading2(this ILog log, string messageFormat, params object[] args)
        {
            LogErrorHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorHeading3(this ILog log, string messageFormat, params object[] args)
        {
            LogErrorHeading(log, Heading3, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="headingContent">Content of the heading.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorHeading(this ILog log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogEvent.Error, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="headingContent">Content of the heading.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogHeading(this ILog log, LogEvent logEvent, string headingContent, string messageFormat, params object[] args)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            log.Write(logEvent, string.Empty);
            log.Write(logEvent, messageFormat, args);
            log.Write(logEvent, headingContent);
            log.Write(logEvent, string.Empty);
        }
    }
}
