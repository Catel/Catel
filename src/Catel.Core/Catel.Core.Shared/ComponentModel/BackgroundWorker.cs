// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundWorker.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace System.ComponentModel
{
    using Threading.Tasks;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Implementation of a BackgroundWorker using the async/await keywords.
    /// </summary>
    /// <remarks>
    /// This class originally comes from http://www.lhotka.net/cslacvs/viewvc.cgi/core/trunk/Source/Csla.WinRT/Threading/BackgroundWorkerBCL.cs.
    /// </remarks>
    public class BackgroundWorker : DependencyObject
    {
        private readonly CoreDispatcher _dispatcher;

        /// <summary>
        /// Creates a new instance of the type.
        /// </summary>
        public BackgroundWorker()
        {
#if NETFX_CORE
            _dispatcher = Dispatcher;
#else
            _dispatcher = get_Dispatcher();
#endif
        }

        /// <summary>
        /// Requests that the background task
        /// cancel its operation.
        /// </summary>
        public void CancelAsync()
        {
            if (!WorkerSupportsCancellation)
            {
                throw new NotSupportedException();
            }

            CancellationPending = true;
        }

        /// <summary>
        /// Gets a value indicating whether a cancel
        /// request is pending.
        /// </summary>
        public bool CancellationPending { get; private set; }

        /// <summary>
        /// Event raised on the UI thread to indicate that
        /// progress has changed.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Call from the worker thread to report on progress.
        /// </summary>
        /// <param name="percentProgress">Percent complete.</param>
        public void ReportProgress(int percentProgress)
        {
            ReportProgress(percentProgress, null);
        }

        /// <summary>
        /// Call from the worker thread to report on progress.
        /// </summary>
        /// <param name="percentProgress">Percent complete.</param>
        /// <param name="userState">User state value.</param>
#if NETFX_CORE
        public async void ReportProgress(int percentProgress, object userState)
#else
        public async void ReportProgress(int percentProgress, object userState)
#endif
        {
            if (ProgressChanged != null)
            {
#if NETFX_CORE
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ProgressChanged(this, new ProgressChangedEventArgs(percentProgress, userState)));
#else
                _dispatcher.Invoke(CoreDispatcherPriority.Normal,
                    (sender, args) => { ProgressChanged(this, new ProgressChangedEventArgs(percentProgress, userState)); }, this, null);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the worker 
        /// reports progress.
        /// </summary>
        public bool WorkerReportsProgress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the worker 
        /// supports cancellation.
        /// </summary>
        public bool WorkerSupportsCancellation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the worker 
        /// is currently executing.
        /// </summary>
        public bool IsBusy { get; set; }

        /// <summary>
        /// Event raised on a background thread when work is to be
        /// performed. The code handling this event should implement
        /// the background task.
        /// </summary>
        public event DoWorkEventHandler DoWork;

        /// <summary>
        /// Event raised on the UI thread when work is complete.
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Raise the RunWorkerCompleted event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            if (RunWorkerCompleted != null)
            {
                RunWorkerCompleted(this, e);
            }
        }

        /// <summary>
        /// Starts the background task by raising the DoWork event.
        /// </summary>
        public void RunWorkerAsync()
        {
            RunWorkerAsync(null);
        }

        /// <summary>
        /// Starts the background task by raising the DoWork event.
        /// </summary>
        /// <param name="userState">User state value.</param>
        public async void RunWorkerAsync(object userState)
        {
            if (DoWork != null)
            {
                CancellationPending = false;
                IsBusy = true;
                try
                {
                    var args = new DoWorkEventArgs {Argument = userState};
                    await Task.Run(() => { DoWork(this, args); });
                    IsBusy = false;
                    OnRunWorkerCompleted(new RunWorkerCompletedEventArgs {Result = args.Result});
                }
                catch (Exception ex)
                {
                    IsBusy = false;
                    OnRunWorkerCompleted(new RunWorkerCompletedEventArgs {Error = ex});
                }
            }
        }
    }

    /// <summary>
    /// DoWork method definition.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void DoWorkEventHandler(object sender, DoWorkEventArgs e);

    /// <summary>
    /// Event arguments passed to the DoWork event/method.
    /// </summary>
    public class DoWorkEventArgs : EventArgs
    {
        /// <summary>
        /// Creates an instance of the type.
        /// </summary>
        public DoWorkEventArgs()
        {
        }

        /// <summary>
        /// Creates an instance of the type.
        /// </summary>
        /// <param name="argument">Argument passed to DoWork handler.</param>
        public DoWorkEventArgs(object argument)
        {
            Argument = argument;
        }

        /// <summary>
        /// Gets or sets the argument value passed into
        /// the DoWork handler.
        /// </summary>
        public object Argument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// operation was cancelled prior to completion.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets or sets a value containing the result
        /// of the operation.
        /// </summary>
        public object Result { get; set; }
    }
}

#endif