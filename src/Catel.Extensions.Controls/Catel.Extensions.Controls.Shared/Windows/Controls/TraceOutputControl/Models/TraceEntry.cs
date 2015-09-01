// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceEntry.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System;

    using Logging;

    /// <summary>
    /// Class containing a log entry as it will be used in the output control.
    /// </summary>
    [ObsoleteEx(ReplacementTypeOrMember = "Orc.Controls, see https://github.com/wildgums/orc.controls", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public class TraceEntry
    {
        #region Constructors
        /// <summary>
        /// Initializes a new log entry for the current date/time.
        /// </summary>
        /// <param name="entry">The inner log entry.</param>
        public TraceEntry(LogEntry entry)
            : this(entry, DateTime.Now)
        {
        }

        /// <summary>
        /// Initializes a new instance that can be fully customized.
        /// </summary>
        /// <param name="entry">The inner log entry.</param>
        /// <param name="time"><see cref="DateTime"/> when the entry was created.</param>
        public TraceEntry(LogEntry entry, DateTime time)
        {
            Message = entry.Message;
            LogEvent = entry.LogEvent;
            Time = time;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Actual message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Log event.
        /// </summary>
        public LogEvent LogEvent { get; private set; }

        /// <summary>
        /// Date/time of the log message.
        /// </summary>
        public DateTime Time { get; private set; }
        #endregion
    }
}

#endif