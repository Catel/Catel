namespace Catel.Logging
{
    using Microsoft.Extensions.Logging;

    public static partial class ILoggerExtensions
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
        public static void LogDebugHeading1(this ILogger log, string messageFormat, params object[] args)
        {
            LogDebugHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogDebugHeading2(this ILogger log, string messageFormat, params object[] args)
        {
            LogDebugHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a debug message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogDebugHeading3(this ILogger log, string messageFormat, params object[] args)
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
        public static void LogDebugHeading(this ILogger log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogLevel.Debug, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInformationHeading1(this ILogger log, string messageFormat, params object[] args)
        {
            LogInformationHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInformationHeading2(this ILogger log, string messageFormat, params object[] args)
        {
            LogInformationHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInformationHeading3(this ILogger log, string messageFormat, params object[] args)
        {
            LogInformationHeading(log, Heading3, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a info message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="headingContent">Content of the heading.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogInformationHeading(this ILogger log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogLevel.Information, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningHeading1(this ILogger log, string messageFormat, params object[] args)
        {
            LogWarningHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningHeading2(this ILogger log, string messageFormat, params object[] args)
        {
            LogWarningHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a warning message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningHeading3(this ILogger log, string messageFormat, params object[] args)
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
        public static void LogWarningHeading(this ILogger log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogLevel.Warning, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorHeading1(this ILogger log, string messageFormat, params object[] args)
        {
            LogErrorHeading(log, Heading1, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorHeading2(this ILogger log, string messageFormat, params object[] args)
        {
            LogErrorHeading(log, Heading2, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading as a error message.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorHeading3(this ILogger log, string messageFormat, params object[] args)
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
        public static void LogErrorHeading(this ILogger log, string headingContent, string messageFormat, params object[] args)
        {
            LogHeading(log, LogLevel.Error, headingContent, messageFormat, args);
        }

        /// <summary>
        /// Logs a heading.
        /// </summary>
        /// <param name="logger">The log.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="headingContent">Content of the heading.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogHeading(this ILogger logger, LogLevel logLevel, string headingContent, string messageFormat, params object[] args)
        {
            logger.Log(logLevel, string.Empty);
            logger.Log(logLevel, messageFormat, args);
            logger.Log(logLevel, headingContent);
            logger.Log(logLevel, string.Empty);
        }
    }
}
