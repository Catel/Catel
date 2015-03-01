// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

#if NET40 || SILVERLIGHT
    using Microsoft;
#endif

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
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Action<TProgress> _reportProgress;

        private readonly Func<TExecuteParameter, CancellationToken, IProgress<TProgress>, Task> _execute;

        private readonly Progress<TProgress> _progress;

        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand{TExecuteParameter,TCanExecuteParameter, TProgress}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecuteWithParameter">The function to call to determine whether the command can be executed with
        /// parameter.</param>
        /// <param name="canExecuteWithoutParameter">The function to call to determine whether the command can be executed without
        /// parameter.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        private TaskCommand(Func<TCanExecuteParameter, bool> canExecuteWithParameter = null, Func<bool> canExecuteWithoutParameter = null,
            Action<TProgress> reportProgress = null, object tag = null)
            : base(null, null, canExecuteWithParameter, canExecuteWithoutParameter, tag)
        {
            _reportProgress = reportProgress;

            CancelCommand = new Command(() =>
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                }
            }, () => IsExecuting);

            _progress = new Progress<TProgress>(OnProgressChanged);
        }
        #endregion

        #region Events

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
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>
        public override bool CanExecute(TCanExecuteParameter parameter)
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
        protected override async void Execute(TExecuteParameter parameter, bool ignoreCanExecuteCheck)
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter, Task> execute, Func<TExecuteParameter, bool> canExecute = null, object tag = null)
            : base(execute, canExecute, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskCommand{TExecuteParameter}" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<TExecuteParameter, CancellationToken, Task> execute, Func<TExecuteParameter, bool> canExecute = null, object tag = null)
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public TaskCommand(Func<Task> execute, Func<bool> canExecute = null, object tag = null)
            : base(execute, canExecute, tag)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catel.MVVM.TaskCommand" /> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
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
        /// <param name="canExecute">The function to call to determine whether the command can be executed.</param>
        /// <param name="reportProgress">Action is executed each time task progress is reported.</param>
        /// <param name="tag">The tag of the command.</param>
        public ProgressiveTaskCommand(Func<CancellationToken, IProgress<TProgress>, Task> execute, Func<bool> canExecute = null, Action<TProgress> reportProgress = null, object tag = null)
            : base(execute, canExecute, reportProgress, tag)
        {
        }
        #endregion
    }
}