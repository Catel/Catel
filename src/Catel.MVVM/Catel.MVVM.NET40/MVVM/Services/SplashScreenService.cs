// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.MVVM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Catel.Logging;
    using Catel.MVVM.Tasks;
    using Views;

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

        private readonly IMessageService _messageService;

        private readonly IViewModelFactory _viewModelFactory;

        private readonly IUIVisualizerService _uiVisualizerService;

        /// <summary>
        /// The _sync obj.
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
        /// The _thread.
        /// </summary>
        private Thread _thread;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenService" /> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="messageService">The message Service.</param>
        /// <param name="viewModelFactory">The view model factory.</param>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="messageService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelFactory" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="uiVisualizerService" /> is <c>null</c>.</exception>
        public SplashScreenService(IDispatcherService dispatcherService, IMessageService messageService, IViewModelFactory viewModelFactory,
            IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => viewModelFactory);
            Argument.IsNotNull(() => uiVisualizerService);

            _dispatcherService = dispatcherService;
            _messageService = messageService;
            _viewModelFactory = viewModelFactory;
            _uiVisualizerService = uiVisualizerService;
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
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Execute in batch mode the enqueued tasks.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <param name="viewModel">
        /// The view model instance.
        /// </param>
        /// <param name="show">
        /// Indicates whether the view model will be shown. If the view model is <c>null</c> then tthis argument will be ignored. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress or committing via async way.
        /// </exception>
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
        /// <param name="task">
        /// The task to enqueue.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="task"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress.
        /// </exception>
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
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <param name="completedCallback">
        /// The completed callback.
        /// </param>
        /// <param name="viewModel">
        /// The view model instance.
        /// </param>
        /// <param name="show">
        /// Indicates whether the view model will be shown. If the view model is <c>null</c> then tthis argument will be ignored. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress or committing via async way.
        /// </exception>
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
        /// <param name="completedCallback">
        /// The completed callback.
        /// </param>
        /// <param name="viewModelType">
        /// The vie model type.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress or committing via async way.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="viewModelType"/> is not of type <see cref="IProgressNotifyableViewModel"/>.
        /// </exception>
        public void CommitAsync(Action completedCallback = null, Type viewModelType = null)
        {
            var viewModel = CreateProgressNotifyableViewModelFrom(viewModelType);
            CommitUsingViewModel(viewModel, true, true, completedCallback);
        }

        /// <summary>
        /// Execute in batch mode the enqueued tasks.
        /// </summary>
        /// <param name="viewModelType">
        /// The view model type.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress or committing via async way.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="viewModelType"/> is not of type <see cref="IProgressNotifyableViewModel"/>.
        /// </exception>
        public void Commit(Type viewModelType = null)
        {
            var viewModel = CreateProgressNotifyableViewModelFrom(viewModelType);
            CommitUsingViewModel(viewModel);
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

            IPleaseWaitService pleaseWaitService = null;
            if (_progressNotifyableViewModel == null)
            {
                pleaseWaitService = GetService<IPleaseWaitService>();
            }

            try
            {
                bool aborted = false, retry = false;
                int progress = 0;
                int total = _tasks.Count;
                var processedTasks = new Stack<ITask>();
                while (!aborted && _tasks.Count > 0)
                {
                    if (!retry)
                    {
                        processedTasks.Push(_tasks.Dequeue());
                    }

                    retry = false;
                    ITask task = processedTasks.Peek();
                    try
                    {
                        Log.Debug("Executing task '{0}'. ", task.Name);
                        
                        if (pleaseWaitService != null)
                        {
                            // TODO: Display smooth detailed progress using the PleasWaitService
// ReSharper disable AccessToModifiedClosure
                            _dispatcherService.Invoke(() => pleaseWaitService.UpdateStatus(progress++, total, task.Name));
// ReSharper restore AccessToModifiedClosure
                        }
                        else if (_progressNotifyableViewModel != null)
                        {
                            _progressNotifyableViewModel.UpdateStatus(progress++, total, task);
                        }

                        task.Execute();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);

                        _dispatcherService.Invoke(() =>
                            {
                                var messageResult = _messageService.Show(string.Format(TaskExecutionErrorMessagePattern, task.Name), "Error", MessageButton.YesNoCancel, MessageImage.Error);
                                switch (messageResult)
                                {
                                    case MessageResult.Yes:
                                        retry = true;
                                        break;

                                    case MessageResult.Cancel:
                                        aborted = true;
                                        break;
                                }        
                            });
                    }
                }

                if (aborted)
                {
                    while (processedTasks.Count > 0)
                    {
                        ITask task = processedTasks.Pop();
                        Log.Debug("Rolling back task '{0}'. ", task.Name);

                        try
                        {
                            if (pleaseWaitService != null)
                            {
// ReSharper disable AccessToModifiedClosure
                                _dispatcherService.Invoke(() => pleaseWaitService.UpdateStatus(progress--, total, string.Format("Rollback '{0}'", task.Name)));
// ReSharper restore AccessToModifiedClosure
                            }
                            else if (_progressNotifyableViewModel != null)
                            {
                                _progressNotifyableViewModel.UpdateStatus(progress--, total, task);
                            }

                            task.Rollback();
                        }
                        catch (Exception e)
                        {
                            Log.Warning("Rollback of task '{0}' failed", task.Name);
                            Log.Error(e);
                        }
                    }
                }
            }
            finally
            {
                if (pleaseWaitService != null)
                {
                    pleaseWaitService.Hide();
                }

                IsRunning = false;

                _dispatcherService.Invoke(() =>
                    {
                        if (_progressNotifyableViewModel != null)
                        {
                            _progressNotifyableViewModel.CloseViewModel(null);
                        }
                    });

                if (_completedCallback != null)
                {
                    _dispatcherService.Invoke(() => _completedCallback.Invoke());
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="show"></param>
        /// <param name="asycn"></param>
        /// <param name="completedCallback"></param>
        private void CommitUsingViewModel(IProgressNotifyableViewModel viewModel, bool show = true, bool asycn = false, Action completedCallback = null)
        {
            StartCommitting(() => viewModel, show);
            if (asycn)
            {
                _thread = new Thread(Execute);
                _thread.SetApartmentState(ApartmentState.STA);
                _completedCallback = completedCallback;
                _thread.Start();
            }
            else
            {
                Execute();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        private IProgressNotifyableViewModel CreateProgressNotifyableViewModelFrom(Type viewModelType)
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
        /// <param name="viewModelFunc">
        /// The view model instance.
        /// </param>
        /// <param name="show">
        /// Indicates whether the view model will be shown. If the view model is <c>null</c> then tthis argument will be ignored. 
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress or committing via async way.
        /// </exception>
        private void StartCommitting(Func<IProgressNotifyableViewModel> viewModelFunc = null, bool show = true)
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
                    _uiVisualizerService.Show(_progressNotifyableViewModel);        
                }

                IsCommitting = true;
            }
        }

        #endregion
    }
}