namespace Catel.MVVM
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Catel.Logging;
    using Services;

    /// <summary>
    /// Class to implement asynchronous task commands in the <see cref="ViewModelBase" />.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the can execute parameter.</typeparam>
    /// <typeparam name="TProgress">The type of the progress report value.</typeparam>
    public class TaskCommand<TExecuteParameter, TCanExecuteParameter, TProgress> : Command<TExecuteParameter, TCanExecuteParameter>, ICatelTaskCommand<TProgress>
        where TProgress : ITaskProgressReport
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Action<TProgress>? _reportProgress;

        private readonly Progress<TProgress>? _progress;

#pragma warning disable IDISP006 // Implement IDisposable.
        private CancellationTokenSource? _cancellationTokenSource;
#pragma warning restore IDISP006 // Implement IDisposable.
        private readonly object _cancellationTokenSourceResultObject = new object();


        private Func<TExecuteParameter?, CancellationToken, IProgress<TProgress>?, Task>? _executeAsync;

        private Task? _task;

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<Task> execute, Func<bool>? canExecute = null, object? tag = null)
            : this(canExecuteWithoutParameter: canExecute, tag: tag)
        {
            _executeAsync = (executeParameter, cancellationToken, progress) => execute();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<CancellationToken, Task> execute, Func<bool>? canExecute = null, object? tag = null)
            : this(canExecuteWithoutParameter: canExecute, tag: tag)
        {
            _executeAsync = (executeParameter, cancellationToken, progress) => execute(cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<CancellationToken, IProgress<TProgress>?, Task> execute, Func<bool>? canExecute = null, Action<TProgress>? reportProgress = null, object? tag = null)
            : this(canExecuteWithoutParameter: canExecute, reportProgress: reportProgress, tag: tag)
        {
            _executeAsync = (executeParameter, cancellationToken, progress) => execute(cancellationToken, progress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter?, Task> execute, Func<TCanExecuteParameter?, bool>? canExecute = null, object? tag = null)
            : this(canExecuteWithParameter: canExecute, tag: tag)
        {
            _executeAsync = (executeParameter, cancellationToken, progress) => execute(executeParameter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter?, CancellationToken, Task> execute, Func<TCanExecuteParameter?, bool>? canExecute = null, object? tag = null)
            : this(canExecuteWithParameter: canExecute, tag: tag)
        {
            _executeAsync = (executeParameter, cancellationToken, progress) => execute(executeParameter, cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter?, CancellationToken, IProgress<TProgress>?, Task> execute, Func<TCanExecuteParameter?, bool>? canExecute = null,
            Action<TProgress>? reportProgress = null, object? tag = null)
            : this(canExecuteWithParameter: canExecute, reportProgress: reportProgress, tag: tag)
        {
            _executeAsync = execute;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="canExecuteWithParameter">The function to call to determine whether the command can be executed with
        /// parameter.</param>
        /// <param name="canExecuteWithoutParameter">The function to call to determine whether the command can be executed without
        /// parameter.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        protected TaskCommand(Func<TCanExecuteParameter?, bool>? canExecuteWithParameter = null, Func<bool>? canExecuteWithoutParameter = null,
            Action<TProgress>? reportProgress = null, object? tag = null)
            : base(null, null, canExecuteWithParameter, canExecuteWithoutParameter, tag)
        {
            _reportProgress = reportProgress;

            CancelCommand = new Command(() =>
            {
                if (_cancellationTokenSource is not null)
                {
                    _cancellationTokenSource.Cancel();
                }
            }, () => IsExecuting);

            _progress = new Progress<TProgress>(OnProgressChanged);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to swallow exceptions that happen in the task command. This property can be used
        /// to use the behavior of Catel 4.x to swallow exceptions.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the task exceptions should be swallowed; otherwise, <c>false</c>.
        /// </value>
        public bool SwallowExceptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is executing.
        /// </summary>
        /// <value><c>true</c> if this instance is executing; otherwise, <c>false</c>.</value>
        public bool IsExecuting
        {
            get { return _cancellationTokenSource is not null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cancellation requested.
        /// </summary>
        /// <value><c>true</c> if this instance is cancellation requested; otherwise, <c>false</c>.</value>
        public bool IsCancellationRequested
        {
            get { return _cancellationTokenSource is not null && _cancellationTokenSource.IsCancellationRequested; }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public Command CancelCommand { get; private set; }

        /// <summary>
        /// Gets the asynchronous result that can be awaited.
        /// </summary>
        /// <value>
        /// The asynchronous result.
        /// </value>
        public Task Task
        {
            get { return _task ?? Task.CompletedTask; }
        }

        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        public event EventHandler<CommandCanceledEventArgs>? Executing;

        /// <summary>
        /// Occurs when the command is canceled.
        /// </summary>
        public event EventHandler<CommandEventArgs>? Canceled;

        /// <summary>
        /// Raised for each reported progress value.
        /// </summary>
        public event EventHandler<CommandProgressChangedEventArgs<TProgress>>? ProgressChanged;

        /// <summary>
        /// Initializes the actions.
        /// </summary>
        protected void InitializeAsyncActions(Func<TExecuteParameter?, Task>? executeWithParameter, Func<Task>? executeWithoutParameter,
            Func<TCanExecuteParameter?, bool>? canExecuteWithParameter, Func<bool>? canExecuteWithoutParameter)
        {
            if (executeWithoutParameter is not null)
            {
                _executeAsync = async (parameter, cancellationToken, progress) => await executeWithoutParameter();
            }

            if (executeWithParameter is not null)
            {
                _executeAsync = async (parameter, cancellationToken, progress) => await executeWithParameter(parameter);
            }

            InitializeActions(null, null, canExecuteWithParameter, canExecuteWithoutParameter);
        }

        /// <summary>
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>
        public override bool CanExecute(TCanExecuteParameter? parameter)
        {
            return !IsExecuting && base.CanExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can
        /// be set to null.</param>
        /// <param name="ignoreCanExecuteCheck">if set to <c>true</c>, the check on <see cref="Command{TExecuteParameter, TCanExecuteParameter}.CanExecute()" /> will be used before
        /// actually executing the action.</param>
#pragma warning disable AvoidAsyncVoid
        protected override async void Execute(TExecuteParameter? parameter, bool ignoreCanExecuteCheck)
#pragma warning restore AvoidAsyncVoid
        {
            var executeAsync = _executeAsync;

            // Double check whether execution is allowed, some controls directly call Execute
            if (executeAsync is null || IsExecuting || (!ignoreCanExecuteCheck && !CanExecute(parameter)))
            {
                return;
            }

            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            var args = new CommandCanceledEventArgs(parameter);
            Executing?.Invoke(this, args);

            if (args.Cancel)
            {
                return;
            }

            RaiseCanExecuteChanged();

            // Use TaskCompletionSource to create a separate task that will not contain the
            // exception that might be thrown. This allows us to let the users await the task
            // but still respect the SwallowExceptions property
            var tcs = new TaskCompletionSource<object>();
            _task = tcs.Task;

            Task? executionTask = null;
            var handledException = false;

            try
            {
                Log.Debug("Executing task command");

                executionTask = executeAsync(parameter, _cancellationTokenSource.Token, _progress);
                await executionTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Log.Debug("Task was canceled");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Task ended with exception");

                // Important: end the task, the exception thrown below will be earlier than the finally block
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                if (!SwallowExceptions)
                {
                    handledException = true;
                    tcs.TrySetException(ex);
                    throw;
                }
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                if (!handledException)
                {
                    tcs.TrySetResult(_cancellationTokenSourceResultObject);
                }
            }

            if (executionTask?.IsCanceled ?? false)
            {
                Canceled?.Invoke(this, new CommandEventArgs(parameter));
            }
            else
            {
                RaiseExecuted(parameter);
            }

            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Requests cancellation of the command.
        /// </summary>
        public void Cancel()
        {
            var cancelCommand = CancelCommand;
            if (cancelCommand is not null)
            {
                if (cancelCommand.CanExecute())
                {
                    cancelCommand.Execute();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Command{TExecuteParameter, TCanExecuteParameter}.CanExecuteChanged"/> event
        /// and raise can-execute-changed method of the <see cref="CancelCommand"/>.
        /// </summary>
        public override void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();

            CancelCommand.RaiseCanExecuteChanged();
        }

        private void OnProgressChanged(TProgress progress)
        {
            if (_reportProgress is not null)
            {
                _reportProgress(progress);
            }

            var action = new Action(() => ProgressChanged?.Invoke(this, new CommandProgressChangedEventArgs<TProgress>(progress)));
            AutoDispatchIfRequired(action);
        }

        private void AutoDispatchIfRequired(Action action)
        {
            var dispatcherService = DispatcherService;
            if (dispatcherService is not null && AutomaticallyDispatchEvents)
            {
                dispatcherService.BeginInvokeIfRequired(action);
            }
            else
            {
                action();
            }
        }
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" /> class with only the
    /// <typeparamref name="TExecuteParameter" /> as generic type.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the can execute parameter.</typeparam>
    public class TaskCommand<TExecuteParameter, TCanExecuteParameter> : TaskCommand<TExecuteParameter, TCanExecuteParameter, ITaskProgressReport>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter?, Task> execute, Func<TCanExecuteParameter?, bool>? canExecute = null, object? tag = null)
            : base(execute, canExecute, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter?, CancellationToken, Task> execute, Func<TCanExecuteParameter?, bool>? canExecute = null, object? tag = null)
            : base(execute, canExecute, tag)
        {
        }
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" /> class with only the
    /// <typeparamref name="TExecuteParameter" /> as generic type.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    public class TaskCommand<TExecuteParameter> : TaskCommand<TExecuteParameter, TExecuteParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter?, Task> execute, Func<TExecuteParameter?, bool>? canExecute = null, object? tag = null)
            : base(execute, canExecute, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter?, CancellationToken, Task> execute, Func<TExecuteParameter?, bool>? canExecute = null, object? tag = null)
            : base(execute, canExecute, tag)
        {
        }
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" /> class with
    /// <see cref="Object" /> as generic types.
    /// </summary>
    public class TaskCommand : TaskCommand<object, object, ITaskProgressReport>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<Task> execute, Func<bool>? canExecute = null, object? tag = null)
            : base(execute, canExecute, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<CancellationToken, Task> execute, Func<bool>? canExecute = null, object? tag = null)
            : base(execute, canExecute, tag)
        {
        }
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" />
    /// class with only the <typeparamref name="TExecuteParameter" /> and <typeparamref name="TProgress" /> as generic
    /// types.
    /// </summary>
    /// <typeparam name="TProgress">Type of the progress change info.</typeparam>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    public class ProgressiveTaskCommand<TProgress, TExecuteParameter> : TaskCommand<TExecuteParameter, TExecuteParameter, TProgress>
        where TProgress : ITaskProgressReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressiveTaskCommand{TProgress, TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public ProgressiveTaskCommand(Func<CancellationToken, IProgress<TProgress>?, Task> execute, Func<bool>? canExecute = null, Action<TProgress>? reportProgress = null, object? tag = null)
            : base(execute, canExecute, reportProgress, tag)
        {
        }
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" /> class with only the
    /// <typeparamref name="TProgress" /> as generic type.
    /// </summary>
    /// <typeparam name="TProgress">Type of the progress change info.</typeparam>
    public class ProgressiveTaskCommand<TProgress> : TaskCommand<object, object, TProgress>
        where TProgress : ITaskProgressReport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public ProgressiveTaskCommand(Func<CancellationToken, IProgress<TProgress>?, Task> execute, Func<bool>? canExecute = null, Action<TProgress>? reportProgress = null, object? tag = null)
            : base(execute, canExecute, reportProgress, tag)
        {
        }
    }
}
