// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebugLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE

namespace Catel.Logging
{
    public partial class DebugLogListener
    {
        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The extra data.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData)
        {
            string consoleMessage = FormatLogEvent(log, message, logEvent, extraData);

            System.Diagnostics.Debug.WriteLine(consoleMessage);
        }
    }
}

#endif