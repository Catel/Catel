// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EtwLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !NET40 && !PCL && !SILVERLIGHT

namespace Catel.Logging.Listeners
{
    using System;
    using System.Diagnostics.Tracing;

    /// <summary>
    /// Log listener for ETW (Event tracing for Windows).
    /// </summary>
    public class EtwLogListener : LogListenerBase
    {
        private readonly CatelEventSource _eventSource = CatelEventSource.Default;

        /// <summary>
        /// Called when any message is written to the log.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="message">The message.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="extraData">The additional data.</param>
        /// <param name="time">The time.</param>
        protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, System.DateTime time)
        {
            base.Write(log, message, logEvent, extraData, time);

            switch (logEvent)
            {
                case LogEvent.Debug:
                    _eventSource.Debug(message);
                    break;

                case LogEvent.Info:
                    _eventSource.Info(message);
                    break;

                case LogEvent.Warning:
                    _eventSource.Warning(message);
                    break;

                case LogEvent.Error:
                    _eventSource.Error(message);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("logEvent");
            }
        }
    }

    [EventSource(Name = "Catel")]
    internal sealed class CatelEventSource : EventSource
    {
        public static readonly CatelEventSource Default = new CatelEventSource();

        [Event(1, Level = EventLevel.Verbose)]
        public void Debug(string message)
        {
            WriteEvent(1, message, Name);
        }

        [Event(2, Level = EventLevel.Informational)]
        public void Info(string message)
        {
            WriteEvent(2, message, Name);
        }

        [Event(3, Level = EventLevel.Warning)]
        public void Warning(string message)
        {
            WriteEvent(3, message, Name);
        }

        [Event(4, Level = EventLevel.Error)]
        public void Error(string message)
        {
            WriteEvent(4, message, Name);
        }
    }
}

#endif