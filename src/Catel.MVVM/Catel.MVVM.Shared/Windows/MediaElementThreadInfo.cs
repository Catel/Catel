// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaElementThreadInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows
{
    using System;
    using System.Threading;
    using System.Windows.Media;

    /// <summary>
    /// Media element thread info.
    /// </summary>
    public class MediaElementThreadInfo : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaElementThreadInfo"/> class.
        /// </summary>
        /// <param name="hostVisual">The host visual.</param>
        /// <param name="thread">The thread.</param>
        public MediaElementThreadInfo(HostVisual hostVisual, Thread thread)
        {
            Argument.IsNotNull("hostVisual", hostVisual);
            Argument.IsNotNull("thread", thread);

            HostVisual = hostVisual;
            Thread = thread;
        }

        /// <summary>
        /// Gets the thread that must be killed by the user.
        /// </summary>
        /// <value>The thread.</value>
        public Thread Thread { get; private set; }

        /// <summary>
        /// Gets the media element.
        /// </summary>
        /// <value>The media element.</value>
        public HostVisual HostVisual { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var thread = Thread;
            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }

            Thread = null;
        }
    }
}

#endif