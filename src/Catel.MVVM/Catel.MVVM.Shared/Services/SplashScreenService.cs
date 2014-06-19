// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;

    using Catel.ExceptionHandling;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.MVVM.Tasks;

    /// <summary>
    /// The splash screen service.
    /// </summary>
    /// <returns>
    /// This service depends on services <see cref="IPleaseWaitService"/> and <see cref="IMessageService"/>.
    /// </returns>
    public sealed class SplashScreenService : ViewModelServiceBase, ISplashScreenService
    {
        #region Constants
        /// <summary>
        /// The execution is in progress error message.
        /// </summary>
        private const string ExecutionIsInProgressErrorMessage = "If the batch is already committed and the execution is in progress";

        /// <summary>
        /// The error message pattern.
        /// </summary>
        private const string TaskExecutionErrorMessagePattern = "An error occur during the execution of task '{0}'.\nDo you want to retry?";

        /// <summary>
        /// The at least one task should be registered error message.
        /// </summary>
        private const string AtLeastOneTaskShouldBeRegisteredErrorMessage = "At least one task should be registered";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IDispatcherService _dispatcherService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IExceptionService _exceptionService;

        /// <summary>
        /// The lock.
        /// </summary>
        private readonly object _syncObj = new object();

        /// <summary>
        /// The tasks queue.
        /// </summary>
        private readonly Queue<ITask> _tasks = new Queue<ITask>();

        /// <summary>
        /// The completed callback.
        /// </summary>
        private Action _completedCallback;

        /// <summary>
        /// The _progress notifyable view model.
        /// </summary>
        private IProgressNotifyableViewModel _progressNotifyableViewModel;

        /// <summary>
        /// The thread.
        /// </summary>
        private Thread _thread;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenService" /> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="viewModelFactory">The view model factory.</param>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="pleaseWaitService">The please wait serivce</param>
        /// <param name="exceptionService">The exception service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelFactory" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uiVisualizerService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="pleaseWaitService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionService" /> is <c>null</c>.</exception>
        public SplashScreenService(IDispatcherService dispatcherService, IViewModelFactory viewModelFactory,
            IUIVisualizerService uiVisualizerService, IPleaseWaitService pleaseWaitService, IExceptionService exceptionService)
        {
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => viewModelFactory);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => exceptionService);

            _dispatcherService = dispatcherService;
            _viewModelFactory = viewModelFactory;
            _uiVisualizerService = uiVisualizerService;
            _pleaseWaitService = pleaseWaitService;
            _exceptionService = exceptionService;

            CloseViewModelOnTerminated = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether is committing.
        /// </summary>
        private bool IsCommitting { get; set; }
        #endregion

        #region ISplashScreenService Members
        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets and sets a value indicating whether the service will close the view model when done.
        /// </summary>
        /// <value><c>true</c> if [close view model on terminated]; otherwise, <c>false</c>.</value>
        /// <remarks>The default value is <c>true</c>.</remarks>
        public bool CloseViewModelOnTerminated { get; set; }

        /// <summary>
        /// Execute in batch mode the enqueued tasks.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="viewModel">The view model instance.</param>
        /// <param name="show">Indicates whether the view model will be shown. If the view model is <c>null</c> then this argument will be used with its default <c>true</c>.</param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        public void Commit<TViewModel>(TViewModel viewModel = default(TViewModel), bool show = true) where TViewModel : IProgressNotifyableViewModel
        {
            if (!ReferenceEquals(viewModel, default(TViewModel)))
            {
                CommitUsingViewModel(viewModel, show);
            }
            else
            {
                Commit(typeof(TViewModel));
            }
        }

        /// <summary>
        /// Enqueue a task to be executed as batch.
        /// </summary>
        /// <param name="task">The task to enqueue.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="task" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress.</exception>
        public void Enqueue(ITask task)
        {
            Argument.IsNotNull(() => task);

            lock (_syncObj)
            {
                if (IsCommitting || IsRunning)
                {
                    throw new InvalidOperationException(ExecutionIsInProgressErrorMessage);
                }

                Log.Debug("Enqueueing task '{0}'. ", task.Name);

                _tasks.Enqueue(task);
            }
        }

        /// <summary>
        /// The commit.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="completedCallback">The completed callback.</param>
        /// <param name="viewModel">The view model instance.</param>
        /// <param name="show">Indicates whether the view model will be shown. If the view model is <c>null</c> then this argument will be ignored.</param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        public void CommitAsync<TViewModel>(Action completedCallback = null, TViewModel viewModel = default(TViewModel), bool show = true) where TViewModel : IProgressNotifyableViewModel
        {
            if (!ReferenceEquals(viewModel, default(TViewModel)))
            {
                CommitUsingViewModel(viewModel, show, true, completedCallback);
            }
            else
            {
                CommitAsync(completedCallback, typeof(TViewModel));
            }
        }

        /// <summary>
        /// Execute in batch mode the enqueued tasks asynchronously.
        /// </summary>
        /// <param name="completedCallback">The completed callback.</param>
        /// <param name="viewModelType">The vie model type.</param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="viewModelType" /> is not of type <see cref="IProgressNotifyableViewModel" />.</exception>
        public void CommitAsync(Action completedCallback = null, Type viewModelType = null)
        {
            CommitUsingViewModel(TryCreateProgressNotifyableViewModelFrom(viewModelType), true, true, completedCallback);
        }

        /// <summary>
        /// Execute in batch mode the enqueued tasks.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="viewModelType" /> is not of type <see cref="IProgressNotifyableViewModel" />.</exception>
        public void Commit(Type viewModelType = null)
        {
            CommitUsingViewModel(TryCreateProgressNotifyableViewModelFrom(viewModelType));
        }
      
        #endregion

        #region Methods
     
        /// <summary>
        /// The execute.
        /// </summary>
        private void Execute()
        {
            IsRunning = true;

            lock (_syncObj)
            {
                IsCommitting = false;
            }

            try
            {
                bool aborted = false;
                int progress = 0;
                int total = _tasks.Count;
                var processedTasks = new Stack<ITask>();
                while (!aborted && _tasks.Count > 0)
                {
                    ITask task = _tasks.Peek();
                    try
                    {
                        Log.Debug("Executing task '{0}'. ", task.Name);
                        if (_progressNotifyableViewModel != null)
                        {
                            _progressNotifyableViewModel.UpdateStatus(progress++, total, task);
                        }
                        else
                        {
                            // TODO: Display smooth detailed progress using the PleasWaitService
// ReSharper disable AccessToModifiedClosure
                            _dispatcherService.Invoke(() => _pleaseWaitService.UpdateStatus(progress++, total, task.Name));
// ReSharper restore AccessToModifiedClosure
                        }

                        if (task.AutomaticallyDispatch)
                        {
                            _dispatcherService.Invoke(task.Execute);
                        }
                        else
                        {
                            task.Execute();
                        }

                        processedTasks.Push(_tasks.Dequeue());
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);

                        if (!_exceptionService.HandleException(ex))
                        {
                            processedTasks.Push(_tasks.Dequeue());
                            aborted = true;
                        }
                        else
                        {
                            processedTasks.Push(_tasks.Dequeue());
                        }
                    }
                }

                if (aborted)
                {
                    progress = processedTasks.Count;
                    while (processedTasks.Count > 0)
                    {
                        ITask task = processedTasks.Pop();
                        Log.Debug("Rolling back task '{0}'. ", task.Name);

                        try
                        {
                            task.Rollback();
                        }
                        catch (Exception e)
                        {
                            Log.Warning("Rollback of task '{0}' failed", task.Name);
                            Log.Error(e);
                        }
                        finally
                        {
                            if (_progressNotifyableViewModel != null)
                            {
                                _progressNotifyableViewModel.UpdateStatus(--progress, total, task);
                            }
                            else
                            {
                                _dispatcherService.Invoke(() => _pleaseWaitService.UpdateStatus(--progress, total, string.Format("Rollback '{0}'", task.Name)));
                            }
                        }
                    }
                }
            }
            finally
            {
                if (_pleaseWaitService != null)
                {
                    _dispatcherService.Invoke(() => _pleaseWaitService.Hide());
                }

                IsRunning = false;

                if (_progressNotifyableViewModel != null && CloseViewModelOnTerminated)
                {
                    _dispatcherService.Invoke(() => _progressNotifyableViewModel.CloseViewModel(null));
                }

                if (_completedCallback != null)
                {
                    _dispatcherService.Invoke(() => _completedCallback.Invoke());
                }
            }
        }

        /// <summary>
        /// Execute in batch mode the enqueued tasks using specific view model instance.
        /// </summary>
        /// <param name="viewModel">The view model instance.</param>
        /// <param name="show">Indicates whether the view model will be shown.</param>
        /// <param name="asycn">Indicates whether the commit will be executed in asynchronous way or not.</param>
        /// <param name="completedCallback">The completed callback.</param>
        private void CommitUsingViewModel(IProgressNotifyableViewModel viewModel, bool show = true, bool asycn = false, Action completedCallback = null)
        {
            BeginCommit(() => viewModel, show);

            if (asycn)
            {
                _thread = new Thread(() =>
                    {
                        bool initialized = false;
                        do
                        {
                            _dispatcherService.Invoke(() => initialized = Application.Current.MainWindow != null);
                            Thread.Sleep(100);
                        }
                        while (!initialized);

                        Execute();
                    });
#if !SILVERLIGHT
                _thread.SetApartmentState(ApartmentState.STA);
#endif                
                _completedCallback = completedCallback;
                _thread.Start();
            }
            else
            {
                if (!Dispatcher.CheckAccess())
                {
                    throw new NotSupportedException("This method must be executed in non-UI thread. Please try with CommitAsync.");
                }
                
                if (Application.Current.MainWindow == null)
                {
                    throw new NotSupportedException("The application is not completly initialized. Please try with CommitAsync.");
                }

                Execute();
            }
        }

        /// <summary>
        /// Create an implementation of the <see cref="IProgressNotifyableViewModel" />.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The instance of <paramref name="viewModelType" />.</returns>
        private IProgressNotifyableViewModel TryCreateProgressNotifyableViewModelFrom(Type viewModelType)
        {
            IProgressNotifyableViewModel viewModel = null;

            if (viewModelType != null)
            {
                Argument.IsOfType(() => viewModelType, typeof(IProgressNotifyableViewModel));

                viewModel = (IProgressNotifyableViewModel)_viewModelFactory.CreateViewModel(viewModelType, null);
            }

            return viewModel;
        }

        /// <summary>
        /// Verifies the state of the service and also sets the commiting state to <c>true</c>.
        /// </summary>
        /// <param name="viewModelFunc">The view model instance.</param>
        /// <param name="show">Indicates whether the view model will be shown. If the view model is <c>null</c> then this argument will be ignored.</param>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        private void BeginCommit(Func<IProgressNotifyableViewModel> viewModelFunc = null, bool show = true)
        {
            lock (_syncObj)
            {
                if (IsCommitting || IsRunning)
                {
                    throw new InvalidOperationException(ExecutionIsInProgressErrorMessage);
                }

                if (_tasks.Count == 0)
                {
                    throw new InvalidOperationException(AtLeastOneTaskShouldBeRegisteredErrorMessage);
                }

                _progressNotifyableViewModel = viewModelFunc == null ? null : viewModelFunc.Invoke();
                if (_progressNotifyableViewModel != null && show)
                {
                    _dispatcherService.Invoke(() => _uiVisualizerService.Show(_progressNotifyableViewModel).RunSynchronously());
                }

                IsCommitting = true;
            }
        }

        #endregion
    }
}

#endif