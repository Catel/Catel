// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Logging;
    using Tasks;

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

        /// <summary>
        /// The view model type.
        /// </summary>
        private Type _viewModelType;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenService" /> class.
        /// </summary>
        public SplashScreenService()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether is committing.
        /// </summary>
        private bool IsCommitting { get; set; }

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        public bool IsRunning { get; private set; }
        #endregion

        #region ISplashScreenService Members

        /// <summary>
        /// Execute in batch mode the enqueued tasks
        /// </summary>
        /// <typeparam name="TViewModel">
        /// The view model type.
        /// </typeparam>
        /// <exception cref="InvalidOperationException">If the batch is already committed and the execution is in progress or committing via async way.</exception>
        public void Commit<TViewModel>() 
            where TViewModel : IProgressNotifyableViewModel
        {
            Commit(typeof(TViewModel));
        }

        /// <summary>
        /// Enqueue a task to be executed as batch.
        /// </summary>
        /// <param name="task">
        /// The task to enqueue
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="task"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress.
        /// </exception>
        public void Enqueue(ITask task)
        {
            Argument.IsNotNull("task", task);
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
        /// <exception cref="InvalidOperationException">
        /// If the batch is already committed and the execution is in progress or committing via async way.
        /// </exception>
        public void CommitAsync<TViewModel>(Action completedCallback = null) 
            where TViewModel : IProgressNotifyableViewModel
        {
            CommitAsync(completedCallback, typeof(TViewModel));
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
            if (viewModelType != null)
            {
                Argument.IsOfType("viewModelType", viewModelType, typeof (IProgressNotifyableViewModel));
            }

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

                _viewModelType = viewModelType;
                if (_viewModelType != null)
                {
                    var viewModelFactory = GetService<IViewModelFactory>();
                    _progressNotifyableViewModel = (IProgressNotifyableViewModel)viewModelFactory.CreateViewModel(_viewModelType, null);

                    var visualizerService = GetService<IUIVisualizerService>();
                    visualizerService.Show(_progressNotifyableViewModel);
                }
                else
                {
                    _progressNotifyableViewModel = null;
                }

                _thread = new Thread(() =>
                    {
                        // NOTE: Patch for delay a bit the thread start 
                        ThreadHelper.Sleep(100);
                        Execute();
                    });

                _thread.SetApartmentState(ApartmentState.STA);
                _completedCallback = completedCallback;
                IsCommitting = true;
            }

            _thread.Start();
        }

        /// <summary>
        /// Execute in batch mode the enqueued tasks
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
            if (viewModelType != null)
            {
                Argument.IsOfType("viewModelType", viewModelType, typeof (IProgressNotifyableViewModel));
            }

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

                _viewModelType = viewModelType;
                if (_viewModelType != null)
                {
                    var viewModelFactory = GetService<IViewModelFactory>();
                    _progressNotifyableViewModel = (IProgressNotifyableViewModel)viewModelFactory.CreateViewModel(_viewModelType, null);

                    var visualizerService = GetService<IUIVisualizerService>();
                    visualizerService.Show(_progressNotifyableViewModel);
                }
                else
                {
                    _progressNotifyableViewModel = null;
                }

                IsCommitting = true;
            }

            Execute();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute.
        /// </summary>
        private void Execute()
        {
            var dispatcherService = GetService<IDispatcherService>();

            IsRunning = true;

            lock (_syncObj)
            {
                IsCommitting = false;
            }

            IPleaseWaitService pleaseWaitService = null;
            if (_viewModelType == null)
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
                            pleaseWaitService.UpdateStatus(++progress, total, task.Name);
                        }
                        else if (_progressNotifyableViewModel != null)
                        {
                            _progressNotifyableViewModel.UpdateStatus(progress++, total, task);
                        }

                        dispatcherService.Invoke(task.Execute);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);

                        var messageService = GetService<IMessageService>();
                        var messageResult = messageService.Show(string.Format(TaskExecutionErrorMessagePattern, task.Name), "Error", MessageButton.YesNoCancel, MessageImage.Error);
                        switch (messageResult)
                        {
                            case MessageResult.Yes:
                                retry = true;
                                break;

                            case MessageResult.Cancel:
                                aborted = true;
                                break;
                        }
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
                                pleaseWaitService.UpdateStatus(progress--, total, string.Format("Rollback '{0}'", task.Name));
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

                dispatcherService.Invoke(() =>
                    {
                        if (_progressNotifyableViewModel != null)
                        {
                            _progressNotifyableViewModel.CloseViewModel(null);
                        }
                    });

                if (_completedCallback != null)
                {
                    dispatcherService.Invoke(() => _completedCallback.Invoke());
                }
            }
        }
        #endregion
    }
}