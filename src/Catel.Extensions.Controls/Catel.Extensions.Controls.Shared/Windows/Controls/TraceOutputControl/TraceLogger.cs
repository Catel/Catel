// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceLogger.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using Logging;

    /// <summary>
    /// Output trace listener.
    /// </summary>
    public class TraceLogger : TraceListenerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Constructor & destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLogger"/> class.
        /// </summary>
        public TraceLogger()
            : base("Output Trace Listener")
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when a new trace has occurred.
        /// </summary>
        /// <param name="message">The message that is traced.</param>
        /// <param name="eventType">Type of the event.</param>
        protected override void OnTrace(string message, TraceEventType eventType)
        {
            var logEvent = LogEvent.Debug;

            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    logEvent = LogEvent.Error;
                    break;

                case TraceEventType.Warning:
                    logEvent = LogEvent.Warning;
                    break;

                case TraceEventType.Information:
                    logEvent = LogEvent.Info;
                    break;

                case TraceEventType.Verbose:
                    logEvent = LogEvent.Debug;
                    break;

                default:
                    logEvent = LogEvent.Debug;
                    break;
            }

            Log.WriteWithData(message, null, logEvent);
        }
        #endregion
    }
}

#endif