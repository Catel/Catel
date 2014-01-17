// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputTraceListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Diagnostics
{
    using System.Diagnostics;

    /// <summary>
    /// Output trace listener.
    /// </summary>
    public class OutputTraceListener : TraceListenerBase
    {
        #region Constructor & destructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputTraceListener"/> class.
        /// </summary>
        public OutputTraceListener()
            : base("Output Trace Listener")
        {
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Delegate for the WrittenLine event.
        /// </summary>
        /// <param name="message">The message that is traced.</param>
        /// <param name="eventType">Type of the event.</param>
        public delegate void WrittenLineDelegate(string message, TraceEventType eventType);
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a new line is written to this trace listener.
        /// </summary>
        public event WrittenLineDelegate WrittenLine = null;
        #endregion

        #region Methods
        /// <summary>
        /// Called when a new trace has occurred.
        /// </summary>
        /// <param name="message">The message that is traced.</param>
        /// <param name="eventType">Type of the event.</param>
        protected override void OnTrace(string message, TraceEventType eventType)
        {
            var writtenLine = WrittenLine;
            if (writtenLine != null)
            {
                writtenLine(message, eventType);
            }
        }
        #endregion
    }
}
