﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.Logging
{
    using System;
    using AndroidLog = global::Android.Util.Log;

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

            switch (logEvent)
            {
                case LogEvent.Debug:
                    System.Diagnostics.Trace.WriteLine(consoleMessage);
                    break;

                case LogEvent.Info:
                    System.Diagnostics.Trace.TraceInformation(consoleMessage);
                    break;

                case LogEvent.Warning:
                    System.Diagnostics.Trace.TraceWarning(consoleMessage);
                    break;

                case LogEvent.Error:
                    System.Diagnostics.Trace.TraceError(consoleMessage);
                    break;
            }
        }
    }
}

#endif