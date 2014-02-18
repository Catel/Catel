// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DebuggerWatcher.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Diagnostics
{
    using System;
    using System.Diagnostics;

#if NET || SL5
    using ThreadClass = System.Threading.Thread;
#else
    using System.Threading.Tasks;
    using ThreadClass = System.Threading.Tasks.Task;
#endif

    /// <summary>
    /// Watcher class that raises an event when the debugger is detached.
    /// </summary>
    public class DebuggerWatcher
    {
        private ThreadClass _thread;
        private bool _abortThread;

        /// <summary>
        /// Occurs when the debugger is detached.
        /// </summary>
        public event EventHandler<EventArgs> DebuggerDetached;

        /// <summary>
        /// Starts watching the debugger in a background thread.
        /// </summary>
        /// <returns><c>true</c> if the watcher has been started which means the debugger is attached, <c>false</c> otherwise.</returns>
        public bool Start()
        {
            if (_thread != null)
            {
                return true;
            }

            if (!Debugger.IsAttached)
            {
                return false;
            }

#if NET || SL5
            _thread = new ThreadClass(ThreadMethod);
            _thread.IsBackground = true;
#else
            _thread = new Task(ThreadMethod, TaskCreationOptions.LongRunning);
#endif

            _thread.Start();

            return true;
        }

        /// <summary>
        /// Stops watching the debugger in a background thread.
        /// </summary>
        public void Stop()
        {
            if (_thread != null)
            {
                _abortThread = true;
            }
        }

        private void ThreadMethod()
        {
            while (!_abortThread)
            {
                if (!Debugger.IsAttached)
                {
                    DebuggerDetached.SafeInvoke(this);
                    break;
                }

                ThreadHelper.Sleep(250);
            }

            _thread = null;
            _abortThread = false;
        }
    }
}