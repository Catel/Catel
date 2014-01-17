// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Logging
{
    using System;

    /// <summary>
    /// Log listener that writes to the console.
    /// </summary>
    public class ConsoleLogListener : LogListenerBase
    {
        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData)
        {
            string consoleMessage = string.Format("{0} => [{1}] {2}", DateTime.Now.ToString("hh:mm:ss:fff"),
                logEvent.ToString().ToUpper(), message);

#if NETFX_CORE || PCL
            System.Diagnostics.Debug.WriteLine(consoleMessage);
#else
            Console.WriteLine(consoleMessage);
#endif
        }
    }
}