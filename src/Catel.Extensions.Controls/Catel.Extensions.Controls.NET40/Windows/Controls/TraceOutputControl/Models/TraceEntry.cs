// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceEntry.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Class containing a trace entry as it will be used in the output control.
    /// </summary>
    public class TraceEntry
    {
        #region Constructors
        /// <summary>
        /// Initializes a new verbose empty trace entry.
        /// </summary>
        public TraceEntry()
            : this(TraceLevel.Verbose, string.Empty, DateTime.Now)
        {
        }

        /// <summary>
        /// Initializes a new trace entry for the current date/time.
        /// </summary>
        /// <param name="level"><see cref="TraceLevel"/> of the trace entry.</param>
        /// <param name="message">Message of the trace entry.</param>
        public TraceEntry(TraceLevel level, string message)
            : this(level, message, DateTime.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance that can be fully customized.
        /// </summary>
        /// <param name="level"><see cref="TraceLevel"/> of the trace entry.</param>
        /// <param name="message">Message of the trace entry.</param>
        /// <param name="time"><see cref="DateTime"/> when the entry was created.</param>
        public TraceEntry(TraceLevel level, string message, DateTime time)
        {
            Message = message;
            TraceLevel = level;
            Time = time;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Actual trace message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Trace level.
        /// </summary>
        public TraceLevel TraceLevel { get; private set; }

        /// <summary>
        /// Date/time of the trace message.
        /// </summary>
        public DateTime Time { get; private set; }
        #endregion
    }
}