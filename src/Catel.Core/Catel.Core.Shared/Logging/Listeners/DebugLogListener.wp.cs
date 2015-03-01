// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && !NETFX_CORE

namespace Catel.Logging
{
    using System;

    public partial class DebugLogListener
    {
        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
        {
            string consoleMessage = FormatLogEvent(log, message, logEvent, extraData, time);

            System.Diagnostics.Debug.WriteLine(consoleMessage);
        }
    }
}

#endif