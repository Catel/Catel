// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System.Diagnostics;

    using Diagnostics;
    using Logging;

    /// <summary>
    /// The output log listener.
    /// </summary>
    public class OutputLogListener : LogListenerBase
    {
        /// <summary>
        /// The trace listener.
        /// </summary>
        private readonly TraceLogger _traceLogger;

        /// <summary>
        /// The output log listener.
        /// </summary>
        public OutputLogListener()
        {
            _traceLogger = new TraceLogger();
            _traceLogger.ActiveTraceLevel = TraceLevel.Verbose;

            Trace.Listeners.Add(_traceLogger);
        }
    }
}

#endif