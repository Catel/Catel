// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Log listener that writes to the console.
    /// </summary>
    public class ConsoleLogListener : LogListenerBase
    {
#if NET
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
#endif

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData)
        {
            string consoleMessage = FormatLogEvent(log, message, logEvent, extraData);

#if NET


#endif

#if NETFX_CORE || PCL
            System.Diagnostics.Debug.WriteLine(consoleMessage);
#else
            Console.WriteLine(consoleMessage);
#endif
        }

#if NET
        private static void UpdateConsoleColors(LogEvent logEvent)
        {
            var colorSet = ColorSets[logEvent];

            Console.BackgroundColor = colorSet.Background;
            Console.ForegroundColor = colorSet.Foreground;
        }
#endif
    }
}