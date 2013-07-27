// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM.Services;

    /// <summary>
    /// Interface for task progress report.
    /// </summary>
    public interface ITaskProgressReport
    {
        #region Properties
        /// <summary>
        ///     Status of the task progress.
        /// </summary>
        string Status { get; }
        #endregion
    }

    /// <summary>
    /// Class to implement asynchronous task commands in the <see cref="ViewModelBase" />.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the can execute parameter.</typeparam>
    /// <typeparam name="TProgress">The type of the progress report value.</typeparam>
    public class TaskCommand<TExecuteParameter, TCanExecuteParameter, TProgress> : ICatelTaskCommand<TProgress>
        where TProgress : ITaskProgressReport
    {
        #region Fields
        // ReSharper disable StaticFieldInGenericType
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IAuthenticationProvider AuthenticationProvider =
            ServiceLocator.Default.ResolveTypeAndReturnNullIfNotRegistered<IAuthenticationProvider>();

        private static readonly IDispatcherService DispatcherService = ServiceLocator.Default.ResolveType<IDispatcherService>();

        // ReSharper restore StaticFieldInGenericType

        private readonly Func<TCanExecuteParameter, bool> _canExecuteWithParameter;

        private readonly Func<bool> _canExecuteWithoutParameter;

        private readonly Action<TProgress> _reportProgress;

        private readonly Func<TExecuteParameter, CancellationToken, IProgress<TProgress>, Task> _execute;

        private readonly Progress<TProgress> _progress;

        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        ///     List of subscribed event handlers so the commands can be unsubscribed upon disposing.
        /// </summary>
        private readonly List<EventHandler> _subscribedEventHandlers = new List<EventHandler>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<Task> execute, Func<bool> canExecute = null, object tag = null)
            : this(canExecuteWithoutParameter: canExecute, tag: tag)
        {
            _execute = (executeParameter, cancellationToken, progress) => execute();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<CancellationToken, Task> execute, Func<bool> canExecute = null, object tag = null)
            : this(canExecuteWithoutParameter: canExecute, tag: tag)
        {
            _execute = (executeParameter, cancellationToken, progress) => execute(cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<CancellationToken, IProgress<TProgress>, Task> execute, Func<bool> canExecute = null, Action<TProgress> reportProgress = null, object tag = null)
            : this(canExecuteWithoutParameter: canExecute, reportProgress: reportProgress, tag: tag)
        {
            _execute = (executeParameter, cancellationToken, progress) => execute(cancellationToken, progress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter, Task> execute, Func<TCanExecuteParameter, bool> canExecute = null, object tag = null)
            : this(canExecuteWithParameter: canExecute, tag: tag)
        {
            _execute = (executeParameter, cancellationToken, progress) => execute(executeParameter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter, CancellationToken, Task> execute, Func<TCanExecuteParameter, bool> canExecute = null, object tag = null)
            : this(canExecuteWithParameter: canExecute, tag: tag)
        {
            _execute = (executeParameter, cancellationToken, progress) => execute(executeParameter, cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter, CancellationToken, IProgress<TProgress>, Task> execute, Func<TCanExecuteParameter, bool> canExecute = null,
            Action<TProgress> reportProgress = null, object tag = null)
            : this(canExecuteWithParameter: canExecute, reportProgress: reportProgress, tag: tag)
        {
            _execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="canExecuteWithParameter">The function to call to determine wether the command can be executed with
        /// parameter.</param>
        /// <param name="canExecuteWithoutParameter">The function to call to determine wether the command can be executed without
        /// parameter.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        internal TaskCommand(Func<TCanExecuteParameter, bool> canExecuteWithParameter = null, Func<bool> canExecuteWithoutParameter = null,
            Action<TProgress> reportProgress = null, object tag = null)
        {
            _canExecuteWithParameter = canExecuteWithParameter;
            _canExecuteWithoutParameter = canExecuteWithoutParameter;
            _reportProgress = reportProgress;

            Tag = tag;
            AutomaticallyDispatchEvents = true;

            CancelCommand = new Command(() =>
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
            }, () => IsExecuting);

            _progress = new Progress<TProgress>(OnProgressChanged);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Catel.MVVM.Command{TExecuteParameter,TCanExecuteParameter}" /> is reclaimed by garbage collection.
        /// </summary>
        ~TaskCommand()
        {
            Dispose(false);
        }
        #endregion

        #region Events
#if NET
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;

                lock (_subscribedEventHandlers)
                {
                    _subscribedEventHandlers.Add(value);
                }
            }

            remove
            {
                lock (_subscribedEventHandlers)
                {
                    _subscribedEventHandlers.Remove(value);
                }

                CommandManager.RequerySuggested -= value;
            }
        }
#else
        public event EventHandler CanExecuteChanged;
#endif

        /// <summary>
        /// Occurs when the command has just been executed successfully.
        /// </summary>
        public event EventHandler<CommandExecutedEventArgs> Executed;

        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        public event EventHandler<CommandCanceledEventArgs> Executing;

        /// <summary>
        /// Occurs when the command is canceled.
        /// </summary>
        public event EventHandler<CommandEventArgs> Canceled;

        /// <summary>
        /// Raised for each reported progress value.
        /// </summary>
        public event EventHandler<CommandProgressChangedEventArgs<TProgress>> ProgressChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the tag for this command. A tag is a way to link any object to a command so you can use your own
        /// methods to recognize the commands, for example by ID or string.
        /// <para />
        /// By default, the value is <c>null</c>.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if [automatically dispatch events]; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchEvents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is executing.
        /// </summary>
        /// <value><c>true</c> if this instance is executing; otherwise, <c>false</c>.</value>
        public bool IsExecuting
        {
            get { return _cancellationTokenSource != null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cancellation requested.
        /// </summary>
        /// <value><c>true</c> if this instance is cancellation requested; otherwise, <c>false</c>.</value>
        public bool IsCancellationRequested
        {
            get { return _cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested; }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public Command CancelCommand { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        /// <remarks>Not a default parameter value because the <see cref="ICommand.CanExecute" /> has no default parameter value.</remarks>
        public bool CanExecute()
        {
            return CanExecute(null);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can
        /// be set to null.</param>
        /// <returns><c>true</c> if this command can be executed; otherwise, <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            if (!(parameter is TExecuteParameter))
            {
                parameter = default(TCanExecuteParameter);
            }

            return CanExecute((TCanExecuteParameter) parameter);
        }

        /// <summary>
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>
        public virtual bool CanExecute(TCanExecuteParameter parameter)
        {
            if (IsExecuting)
            {
                return false;
            }

            if (AuthenticationProvider != null)
            {
                if (!AuthenticationProvider.CanCommandBeExecuted(this, parameter))
                {
                    return false;
                }
            }

            if (_canExecuteWithParameter != null)
            {
                return _canExecuteWithParameter(parameter);
            }

            if (_canExecuteWithoutParameter != null)
            {
                return _canExecuteWithoutParameter();
            }

            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        /// <remarks>Not a default parameter value because the <see cref="ICommand.Execute" /> has no default parameter value.</remarks>
        public void Execute()
        {
            Execute(null);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can
        /// be set to null.</param>
        public void Execute(object parameter)
        {
            if (!(parameter is TExecuteParameter))
            {
                parameter = default(TCanExecuteParameter);
            }

            Execute((TExecuteParameter) parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can
        /// be set to null.</param>
        public void Execute(TExecuteParameter parameter)
        {
            ExecuteAsync(parameter, false);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can
        /// be set to null.</param>
        /// <param name="ignoreCanExecuteCheck">if set to <c>true</c>, the check on <see cref="CanExecute()" /> will be used before
        /// actually executing the action.</param>
        protected virtual async void ExecuteAsync(TExecuteParameter parameter, bool ignoreCanExecuteCheck)
        {
            // Double check whether execution is allowed, some controls directly call Execute
            if (_execute == null || IsExecuting || (!ignoreCanExecuteCheck && !CanExecute(parameter)))
            {
                return;
            }

            var args = new CommandCanceledEventArgs(parameter);
            Executing.SafeInvoke(this, args);

            if (args.Cancel)
            {
                return;
            }

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
            }
            _cancellationTokenSource = new CancellationTokenSource();

            RaiseCanExecuteChanged();

            Task executionTask = _execute(parameter, _cancellationTokenSource.Token, _progress);
            try
            {
                Log.Info("Executing task command...");
                await executionTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Log.Info("Task was canceled.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Task ended with exception.");
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            if (executionTask.IsCanceled || executionTask.IsFaulted)
            {
                Canceled.SafeInvoke(this, new CommandEventArgs(parameter));
            }
            else
            {
                RaiseExecuted(parameter);
            }
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var action = new Action(() =>
            {
#if NET
                foreach (EventHandler handler in _subscribedEventHandlers)
                {
                    handler.SafeInvoke(this);
                }

                CommandManager.InvalidateRequerySuggested();
#else
                CanExecuteChanged.SafeInvoke(this);
#endif
            });

            AutoDispatchIfRequired(action);
        }

        /// <summary>
        /// Requests cancellation of the command.
        /// </summary>
        public void Cancel()
        {
            if (CancelCommand.CanExecute())
            {
                CancelCommand.Execute();
            }
        }

        /// <summary>
        /// Raises the <see cref="Executed" /> event.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected void RaiseExecuted(object parameter)
        {
            var action = new Action(() => Executed.SafeInvoke(this, new CommandExecutedEventArgs(this, parameter)));
            AutoDispatchIfRequired(action);
        }

        private void OnProgressChanged(TProgress progress)
        {
            if (_reportProgress != null)
            {
                _reportProgress(progress);
            }

            var action = new Action(() => ProgressChanged.SafeInvoke(this, new CommandProgressChangedEventArgs<TProgress>(progress)));
            AutoDispatchIfRequired(action);
        }

        private void AutoDispatchIfRequired(Action action)
        {
            if (AutomaticallyDispatchEvents)
            {
                DispatcherService.BeginInvokeIfRequired(action);
            }
            else
            {
                action();
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to
        /// release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                lock (_subscribedEventHandlers)
                {
#if NET
                    foreach (EventHandler eventHandler in _subscribedEventHandlers)
                    {
                        CommandManager.RequerySuggested -= eventHandler;
                    }
#endif

                    _subscribedEventHandlers.Clear();
                }
                CancelCommand.Dispose();
            }
        }
        #endregion
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" /> class with only the
    /// <typeparamref name="TExecuteParameter" /> as generic type.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    public class TaskCommand<TExecuteParameter> : TaskCommand<TExecuteParameter, TExecuteParameter, ITaskProgressReport>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter, Task> execute, Func<TExecuteParameter, bool> canExecute = null, object tag = null)
            : base(execute, canExecute, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(
            Func<TExecuteParameter, CancellationToken, Task> execute,
            Func<TExecuteParameter, bool> canExecute = null,
            object tag = null)
            : base(execute, canExecute, tag)
        {
        }
        #endregion
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" /> class with
    /// <see cref="Object" /> as generic types.
    /// </summary>
    public class TaskCommand : TaskCommand<object, object, ITaskProgressReport>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<Task> execute, Func<bool> canExecute = null, object tag = null)
            : base(execute, canExecute, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<CancellationToken, Task> execute, Func<bool> canExecute = null, object tag = null)
            : base(execute, canExecute, tag)
        {
        }
        #endregion
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
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressiveTaskCommand{TProgress, TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public ProgressiveTaskCommand(Func<CancellationToken, IProgress<TProgress>, Task> execute, Func<bool> canExecute = null, Action<TProgress> reportProgress = null, object tag = null)
            : base(execute, canExecute, reportProgress, tag)
        {
        }
        #endregion
    }

    /// <summary>
    /// Implements the <see cref="TaskCommand{TExecuteParameter,TCanExecuteParameter,TProgress}" /> class with only the
    /// <typeparamref name="TProgress" /> as generic type.
    /// </summary>
    /// <typeparam name="TProgress">Type of the progress change info.</typeparam>
    public class ProgressiveTaskCommand<TProgress> : TaskCommand<object, object, TProgress>
        where TProgress : ITaskProgressReport
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public ProgressiveTaskCommand(Func<CancellationToken, IProgress<TProgress>, Task> execute, Func<bool> canExecute = null, Action<TProgress> reportProgress = null, object tag = null)
            : base(execute, canExecute, reportProgress, tag)
        {
        }
        #endregion
    }
}