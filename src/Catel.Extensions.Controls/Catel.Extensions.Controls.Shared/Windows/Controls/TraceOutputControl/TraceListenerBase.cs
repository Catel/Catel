// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceListenerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2011 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Diagnostics
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Class that implements a trace listener.
    /// </summary>
    public abstract class TraceListenerBase : TraceListener
    {
        #region Constants
        /// <summary>
        /// Trace message for the ItemsSource timing issue. This line should be ignored.
        /// </summary>
        private const string ItemsSourceTimingIssueTrace = "ContentAlignment; DataItem=null;";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListenerBase"/> class.
        /// </summary>
        /// <param name="name">The name of the <see cref="T:System.Diagnostics.TraceListener"/>.</param>
        protected TraceListenerBase(string name)
        {
            Name = name;
            ActiveTraceLevel = TraceLevel.Info;

            TraceSourceCollection = new List<TraceSource>();
            TraceSourceCollection.Add(PresentationTraceSources.DataBindingSource);
            TraceSourceCollection.Add(PresentationTraceSources.DependencyPropertySource);
            TraceSourceCollection.Add(PresentationTraceSources.MarkupSource);
            TraceSourceCollection.Add(PresentationTraceSources.ResourceDictionarySource);

            foreach (var traceSource in TraceSourceCollection)
            {
                traceSource.Listeners.Add(this);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the trace source collection.
        /// </summary>
        /// <value>The trace source collection.</value>
        private List<TraceSource> TraceSourceCollection { get; set; }

        /// <summary>
        /// Gets or sets the active trace type.
        /// </summary>
        public TraceLevel ActiveTraceLevel { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Writes trace information, a formatted array of objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType"/> values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (message.Contains(ItemsSourceTimingIssueTrace))
            {
            	return;
            }

            switch (eventType)
            {
                case TraceEventType.Error:
                    if ((ActiveTraceLevel == TraceLevel.Error) ||
                        (ActiveTraceLevel == TraceLevel.Warning) ||
                        (ActiveTraceLevel == TraceLevel.Info) ||
                        (ActiveTraceLevel == TraceLevel.Verbose))
                    {
                        OnTrace(message, eventType);
                    }

                    break;

                case TraceEventType.Warning:
                    if ((ActiveTraceLevel == TraceLevel.Warning) ||
                        (ActiveTraceLevel == TraceLevel.Info) ||
                        (ActiveTraceLevel == TraceLevel.Verbose))
                    {
                        OnTrace(message, eventType);
                    }

                    break;

                case TraceEventType.Information:
                    if ((ActiveTraceLevel == TraceLevel.Info) ||
                        (ActiveTraceLevel == TraceLevel.Verbose))
                    {
                        OnTrace(message, eventType);
                    }

                    break;

                // Everything else is verbose
                ////case TraceEventType.Verbose:
                default:
                    if (ActiveTraceLevel == TraceLevel.Verbose)
                    {
                        OnTrace(message, TraceEventType.Verbose);
                    }

                    break;
            }
        }

        /// <summary>
        /// Writes text to the output window.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public override void Write(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// Writes a line of text to the output window.
        /// </summary>
        /// <param name="message">Message to write.</param>
        public override void WriteLine(string message)
        {
            if (ActiveTraceLevel == TraceLevel.Verbose)
            {
                OnTrace(message, TraceEventType.Verbose);
            }
        }

        /// <summary>
        /// Called when a new trace has occurred.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="eventType">Type of the event.</param>
        protected abstract void OnTrace(string message, TraceEventType eventType);
        #endregion
    }
}

#endif