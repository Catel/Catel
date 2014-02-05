// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundWorker.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace System.ComponentModel
{
    using System.Threading;

    /// <summary>
    /// Executes an operation on a separate thread.
    /// </summary>
    /// <remarks>
    /// This code originally comes from https://pclcontrib.codeplex.com/SourceControl/latest#Source/Portable.ComponentModel.Async/ComponentModel/BackgroundWorker.cs.
    /// </remarks>
    public partial class BackgroundWorker
    {
        #region Fields
        private bool _cancellationPending;

        private AsyncOperation _currentOperation;

        private bool _workerReportsProgress;

        private bool _workerSupportsCancellation;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundWorker"/> class.
        /// </summary>
        public BackgroundWorker()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the application has requested cancellation of a background operation.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the application has requested cancellation of a background operation; otherwise, <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool CancellationPending
        {
            get
            {
                return _cancellationPending;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="BackgroundWorker"/> is running an asynchronous operation.
        /// </summary>
        /// <value>
        /// <see langword="true"/>, if the <see cref="BackgroundWorker"/> is running an asynchronous operation; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsBusy
        {
            get
            {
                return _currentOperation != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BackgroundWorker"/> can report progress updates.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="BackgroundWorker"/> supports progress updates; otherwise <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool WorkerReportsProgress
        {
            get
            {
                return _workerReportsProgress;
            }
            set
            {
                _workerReportsProgress = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BackgroundWorker"/> supports asynchronous cancellation.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="BackgroundWorker"/> supports cancellation; otherwise <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public bool WorkerSupportsCancellation
        {
            get
            {
                return _workerSupportsCancellation;
            }
            set
            {
                _workerSupportsCancellation = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="percentProgress">
        /// The percentage, from 0 to 100, of the background operation that is complete. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="WorkerReportsProgress"/> property is <see langword="false"/>.
        /// </exception>
        public void ReportProgress(int percentProgress)
        {
            ReportProgress(percentProgress, null);
        }

        /// <summary>
        /// Raises the <see cref="ProgressChanged"/> event.
        /// </summary>
        /// <param name="percentProgress">
        /// The percentage, from 0 to 100, of the background operation that is complete. 
        /// </param>
        /// <param name="userState">
        /// The state object passed to <see cref="RunWorkerAsync()"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="WorkerReportsProgress"/> property is <see langword="false"/>.
        /// </exception>
        public void ReportProgress(int percentProgress, object userState)
        {
            if (!WorkerReportsProgress)
            {
                throw new InvalidOperationException();
            }

            ProgressChangedEventArgs e = new ProgressChangedEventArgs(percentProgress, userState);

            if (_currentOperation == null)
            {
                // No operation, just execute it directly
                OnProgressChanged(e);
            }
            else
            {
                // Operation in progress, post it to the synchronization context
                _currentOperation.Post(_ => OnProgressChanged(e), null);
            }
        }

        /// <summary>
        /// Starts execution of a background operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsBusy"/> is <see langword="true"/>.
        /// </exception>
        public void RunWorkerAsync()
        {
            RunWorkerAsync(null);
        }

        /// <summary>
        /// Starts execution of a background operation.
        /// </summary>
        /// <param name="argument">
        /// A parameter for use by the background operation to be executed in the <see cref="DoWork"/> event handler. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsBusy"/> is <see langword="true"/>.
        /// </exception>
        public void RunWorkerAsync(object argument)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException();
            }

            _cancellationPending = false;
            _currentOperation = CreateAsyncOperation();

            QueueOnBackgroundThread(DoWorkOnBackgroundThread, argument);
        }

        /// <summary>
        /// Requests cancellation of a pending background operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="WorkerSupportsCancellation"/> property is <see langword="false"/>.
        /// </exception>
        public void CancelAsync()
        {
            if (!WorkerSupportsCancellation)
            {
                throw new InvalidOperationException();
            }

            _cancellationPending = true;
        }

        /// <summary>
        /// Raises the <see cref="DoWork"/> event. 
        /// </summary>
        /// <param name="e">
        /// An <see cref="DoWorkEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnDoWork(DoWorkEventArgs e)
        {
            var handler = DoWork;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ProgressChanged"/> event. 
        /// </summary>
        /// <param name="e">
        /// An <see cref="ProgressChangedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            var handler = ProgressChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RunWorkerCompleted"/> event. 
        /// </summary>
        /// <param name="e">
        /// An <see cref="RunWorkerCompletedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            var handler = RunWorkerCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Creates a new async operation.
        /// </summary>
        /// <returns>
        /// An <see cref="AsyncOperation"/>.
        /// </returns>
        protected virtual AsyncOperation CreateAsyncOperation()
        {
            return AsyncOperationManager.CreateOperation(null);
        }

        internal virtual void QueueOnBackgroundThread(Action<object> action, object argument)
        {
            // The BackgroundWorker in .NET uses Delegate.BeginInvoke, however, given that 
            // this isn't supported on Compact-based platforms such as Windows Phone and Xbox,
            // we use the ThreadPool instead.
            ThreadPool.QueueUserWorkItem((state) => action(state), argument);
        }

        private void RaiseOnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            _cancellationPending = false;
            _currentOperation = null;
            OnRunWorkerCompleted(e);
        }

        private void DoWorkOnBackgroundThread(object argument)
        {
            // Called on a background thread
            DoWorkEventArgs e = new DoWorkEventArgs(argument);
            Exception error = null;

            try
            {
                OnDoWork(e);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            CompleteWorkOnBackgroundThread(error, e.Cancel ? null : e.Result, e.Cancel);
        }

        private void CompleteWorkOnBackgroundThread(Exception error, object result, bool cancelled)
        {
            // Called on a background thread
            RunWorkerCompletedEventArgs e = new RunWorkerCompletedEventArgs(result, error, cancelled);

            _currentOperation.PostOperationCompleted(_ => RaiseOnRunWorkerCompleted(e), (object)null);
        }
        #endregion

        /// <summary>
        /// Occurs when <see cref="RunWorkerAsync()"/> is called.
        /// </summary>
        public event DoWorkEventHandler DoWork;

        /// <summary>
        /// Occurs when <see cref="ReportProgress(Int32)"/> is called.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;
    }
}