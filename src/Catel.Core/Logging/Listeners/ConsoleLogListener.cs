namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Log listener that writes to the console.
    /// </summary>
    public class ConsoleLogListener : LogListenerBase
    {
        private class ConsoleColorSet
        {
            public ConsoleColorSet(ConsoleColor background, ConsoleColor foreground)
            {
                Background = background;
                Foreground = foreground;
            }

            public ConsoleColor Background { get; private set; }
            public ConsoleColor Foreground { get; private set; }
        }

        private static readonly Dictionary<LogEvent, ConsoleColorSet> ColorSets = new Dictionary<LogEvent, ConsoleColorSet>();

        static ConsoleLogListener()
        {
            ColorSets[LogEvent.Debug] = new ConsoleColorSet(ConsoleColor.Black, ConsoleColor.Gray);
            ColorSets[LogEvent.Info] = new ConsoleColorSet(ConsoleColor.Black, ConsoleColor.White);
            ColorSets[LogEvent.Warning] = new ConsoleColorSet(ConsoleColor.Black, ConsoleColor.Yellow);
            ColorSets[LogEvent.Error] = new ConsoleColorSet(ConsoleColor.Black, ConsoleColor.Red);
        }

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object? extraData, LogData? logData, DateTime time)
        {
            var consoleMessage = FormatLogEvent(log, message, logEvent, extraData, logData, time);

            var oldConsoleBackgroundColor = Console.BackgroundColor;
            var oldConsoleForegroundColor = Console.ForegroundColor;

            UpdateConsoleColors(logEvent);

            Console.WriteLine(consoleMessage);
            Console.BackgroundColor = oldConsoleBackgroundColor;
            Console.ForegroundColor = oldConsoleForegroundColor;
        }

        private static void UpdateConsoleColors(LogEvent logEvent)
        {
            var colorSet = ColorSets[logEvent];

            Console.BackgroundColor = colorSet.Background;
            Console.ForegroundColor = colorSet.Foreground;
        }
    }
}
