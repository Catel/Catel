// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsynchronousCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if !XAMARIN_FORMS
namespace Catel.MVVM
{
    using System;

    using Catel.Services;

    using IoC;

#if NETFX_CORE
    using global::Windows.System.Threading;
#else
    using System.Threading;
#endif

    /// <summary>
    /// Command that allows an action to be exceuted asynchronous. This way, it can be canceled and will not block the
    /// UI thread.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the can execute parameter.</typeparam>
    public class AsynchronousCommand<TExecuteParameter, TCanExecuteParameter> : Command<TExecuteParameter, TCanExecuteParameter>
    {
        private static readonly IDispatcherService _dispatcherService;

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="AsynchronousCommand{TExecuteParameter, TCanExecuteParameter}"/> class.
        /// </summary>
        static AsynchronousCommand()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            _dispatcherService = dependencyResolver.Resolve<IDispatcherService>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousCommand{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public AsynchronousCommand(Action execute, Func<bool> canExecute = null, object tag = null)
            : this(null, execute, null, canExecute, tag) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousCommand{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public AsynchronousCommand(Action<TExecuteParameter> execute, Func<TCanExecuteParameter, bool> canExecute = null, object tag = null)
            : this(execute, null, canExecute, null, tag) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousCommand{TExecuteParameter, TCanExecuteParameter}"/> class.
        /// </summary>
        /// <param name="executeWithParameter">The action to execute with parameter.</param>
        /// <param name="executeWithoutParameter">The action to execute without parameter.</param>
        /// <param name="canExecuteWithParameter">The function to call to determine wether the command can be executed with parameter.</param>
        /// <param name="canExecuteWithoutParameter">The function to call to determine wether the command can be executed without parameter.</param>
        /// <param name="tag">The tag of the command.</param>
        internal AsynchronousCommand(Action<TExecuteParameter> executeWithParameter, Action executeWithoutParameter,
            Func<TCanExecuteParameter, bool> canExecuteWithParameter, Func<bool> canExecuteWithoutParameter, object tag)
            : base(executeWithParameter, executeWithoutParameter, canExecuteWithParameter, canExecuteWithoutParameter, tag)
        {
            CancelCommand = new Command(() => { IsCancelationRequested = true; }, () => IsExecuting);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this instance is executing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is executing; otherwise, <c>false</c>.
        /// </value>
        public bool IsExecuting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cancelation requested.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is cancelation requested; otherwise, <c>false</c>.
        /// </value>
        public bool IsCancelationRequested { get; set; }

        /// <summary>
        /// Gets a value indicating whether the command should cancel itself.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the command should be canceled; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldCancel
        {
            get { return IsCancelationRequested; }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public Command CancelCommand { get; private set; }
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
        #endregion

        #region Methods
        /// <summary>
        /// Reports progress on the UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c></exception>
        public void ReportProgress(Action action)
        {
            Argument.IsNotNull("action", action);

            if (!IsExecuting)
            {
                return;
            }

            _dispatcherService.BeginInvoke(() =>
            {
                action();

                RaiseCanExecuteChanged();
            });
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <param name="ignoreCanExecuteCheck">if set to <c>true</c>, the check on <see cref="System.Windows.Input.ICommand.CanExecute"/> will be used before actually executing the action.</param>
        protected override async void Execute(TExecuteParameter parameter, bool ignoreCanExecuteCheck)
        {
            if (IsExecuting)
            {
                return;
            }

            // It might be possible that the IsExecuting is used as a check whether the command can be executed again,
            // so use that as a check
            var canExecute = CanExecute(parameter);
            if (!canExecute)
            {
                return;
            }

            var args = new CommandCanceledEventArgs(parameter);
            Executing.SafeInvoke(this, args);

            if (args.Cancel)
            {
                return;
            }

            IsExecuting = true;

            RaiseCanExecuteChanged();

            // Run the action on a new thread from the thread pool (this will therefore work in Silverlight and Windows Phone as well)
#if NETFX_CORE
            await ThreadPool.RunAsync(state =>
#else
            ThreadPool.QueueUserWorkItem(state =>
#endif
            {
                // Skip the check, we already did that
                base.Execute(parameter, true);

                ReportProgress(() =>
                {
                    IsExecuting = false;

                    if (IsCancelationRequested)
                    {
                        Canceled.SafeInvoke(this, new CommandEventArgs(parameter));
                    }
                    else
                    {
                        RaiseExecuted(parameter);
                    }

                    IsCancelationRequested = false;
                });
            });
        }
        #endregion
    }

    /// <summary>
    /// Implements the <see cref="AsynchronousCommand{TExecuteParameter, TCanExecuteParameter}"/> class with only the <typeparamref name="TExecuteParameter"/> 
    /// as generic type.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    public class AsynchronousCommand<TExecuteParameter> : AsynchronousCommand<TExecuteParameter, TExecuteParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousCommand{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public AsynchronousCommand(Action execute, Func<bool> canExecute = null, object tag = null)
            : base(null, execute, null, canExecute, tag) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousCommand{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public AsynchronousCommand(Action<TExecuteParameter> execute, Func<TExecuteParameter, bool> canExecute = null, object tag = null)
            : base(execute, null, canExecute, null, tag) { }
    }

    /// <summary>
    /// Implements the <see cref="AsynchronousCommand{TExecuteParameter, TCanExecuteParameter}"/> class with <see cref="Object"/> as generic types.
    /// </summary>
    public class AsynchronousCommand : AsynchronousCommand<object, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousCommand{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public AsynchronousCommand(Action execute, Func<bool> canExecute = null, object tag = null)
            : base(execute, canExecute, tag) { }
    }
}
#endif