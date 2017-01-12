// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Command.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1956 // Both Command and CompositeCommand implement ICommand
#pragma warning disable 3021 // 'type' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute

namespace Catel.MVVM
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Services;

    using IoC;
    using Logging;
    using Threading;

    /// <summary>
    /// Base class for generic command classes. Contains protected static services for using in derived classes.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// Authentication provider.
        /// </summary>
        [CLSCompliant(false)]
        protected static readonly IAuthenticationProvider AuthenticationProvider;

        /// <summary>
        /// Dispatcher service.
        /// </summary>
        protected static readonly IDispatcherService DispatcherService;

        /// <summary>
        /// Initializes static members of the <see cref="CommandBase"/> class.
        /// </summary>
        static CommandBase()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            AuthenticationProvider = dependencyResolver.TryResolve<IAuthenticationProvider>();
            DispatcherService = dependencyResolver.TryResolve<IDispatcherService>();
        }
    }

    /// <summary>
    /// Class to implement commands in the <see cref="ViewModelBase"/>.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the can execute parameter.</typeparam>
    public class Command<TExecuteParameter, TCanExecuteParameter> : CommandBase, ICatelCommand<TExecuteParameter, TCanExecuteParameter>
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private Func<TCanExecuteParameter, bool> _canExecuteWithParameter;
        private Func<bool> _canExecuteWithoutParameter;
        private Action<TExecuteParameter> _executeWithParameter;
        private Action _executeWithoutParameter;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public Command(Action execute, Func<bool> canExecute = null, object tag = null)
            : this(null, execute, null, canExecute, tag) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public Command(Action<TExecuteParameter> execute, Func<TCanExecuteParameter, bool> canExecute = null, object tag = null)
            : this(execute, null, canExecute, null, tag) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TExecuteParameter, TCanExecuteParameter}"/> class.
        /// </summary>
        /// <param name="executeWithParameter">The action to execute with parameter.</param>
        /// <param name="executeWithoutParameter">The action to execute without parameter.</param>
        /// <param name="canExecuteWithParameter">The function to call to determine wether the command can be executed with parameter.</param>
        /// <param name="canExecuteWithoutParameter">The function to call to determine wether the command can be executed without parameter.</param>
        /// <param name="tag">The tag of the command.</param>
        internal Command(Action<TExecuteParameter> executeWithParameter, Action executeWithoutParameter,
            Func<TCanExecuteParameter, bool> canExecuteWithParameter, Func<bool> canExecuteWithoutParameter,
            object tag)
        {
            InitializeActions(executeWithParameter, executeWithoutParameter, canExecuteWithParameter, canExecuteWithoutParameter);

            Tag = tag;
            AutomaticallyDispatchEvents = true;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Occurs when the command has just been executed successfully.
        /// </summary>
        public event EventHandler<CommandExecutedEventArgs> Executed;

        /// <summary>
        /// Occurs when the command has just been executed successfully.
        /// </summary>
        public event AsyncEventHandler<CommandExecutedEventArgs> ExecutedAsync;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the tag for this command. A tag is a way to link any object to a command so you can use your own
        /// methods to recognize the commands, for example by ID or string.
        /// <para/>
        /// By default, the value is <c>null</c>.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        public bool AutomaticallyDispatchEvents { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the actions.
        /// </summary>
        protected void InitializeActions(Action<TExecuteParameter> executeWithParameter, Action executeWithoutParameter,
            Func<TCanExecuteParameter, bool> canExecuteWithParameter, Func<bool> canExecuteWithoutParameter)
        {
            _canExecuteWithParameter = canExecuteWithParameter;
            _canExecuteWithoutParameter = canExecuteWithoutParameter;
            _executeWithParameter = executeWithParameter;
            _executeWithoutParameter = executeWithoutParameter;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this command can be executed; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Not a default parameter value because the <see cref="ICommand.CanExecute"/> has no default parameter value.
        /// </remarks>
        public bool CanExecute()
        {
            return CanExecute(null);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// <c>true</c> if this command can be executed; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            if (!(parameter is TCanExecuteParameter))
            {
                parameter = default(TCanExecuteParameter);
            }

            return CanExecute((TCanExecuteParameter)parameter);
        }

        /// <summary>
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// <c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanExecute(TCanExecuteParameter parameter)
        {
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
        /// <remarks>
        /// Not a default parameter value because the <see cref="ICommand.Execute"/> has no default parameter value.
        /// </remarks>
        public void Execute()
        {
            Execute(null);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (!(parameter is TExecuteParameter))
            {
                parameter = default(TExecuteParameter);
            }

            Execute((TExecuteParameter)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(TExecuteParameter parameter)
        {
#pragma warning disable 4014
            ExecuteAsync(parameter, false);
#pragma warning restore 4014
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <param name="ignoreCanExecuteCheck">if set to <c>true</c>, the check on <see cref="CanExecute()"/> will be used before actually executing the action.</param>
        protected virtual async Task ExecuteAsync(TExecuteParameter parameter, bool ignoreCanExecuteCheck)
        {
            // Double check whether execution is allowed, some controls directly call Execute
            if (!ignoreCanExecuteCheck && !CanExecute(parameter))
            {
                return;
            }

            if (_executeWithParameter != null)
            {
                _executeWithParameter(parameter);
                await RaiseExecutedAsync(parameter);
            }
            else if (_executeWithoutParameter != null)
            {
                _executeWithoutParameter();
                await RaiseExecutedAsync(parameter);
            }

            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            AutoDispatchIfRequiredAsync(async () =>
            {
                try
                {
                    CanExecuteChanged.SafeInvoke(this);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to raise CanExecuteChanged");
                }
            });
        }

        /// <summary>
        /// Raises the <see cref="Executed" /> event.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task.</returns>
        protected Task RaiseExecutedAsync(object parameter)
        {
            var action = new Func<Task>(async () =>
            {
                var eventArgs = new CommandExecutedEventArgs(this, parameter);
                Executed.SafeInvoke(this, eventArgs);
                await ExecutedAsync.SafeInvokeAsync(this, eventArgs);
            });

            return AutoDispatchIfRequiredAsync(action);
        }

        private Task AutoDispatchIfRequiredAsync(Func<Task> action)
        {
            if (AutomaticallyDispatchEvents)
            {
                DispatcherService.BeginInvokeIfRequired(action);
            }
            else
            {
                return action();
            }

            return TaskHelper.Completed;
        }
        #endregion
    }

    /// <summary>
    /// Implements the <see cref="Command{TExecuteParameter, TCanExecuteParameter}"/> class with only the <typeparamref name="TExecuteParameter"/> as generic type.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    public class Command<TExecuteParameter> : Command<TExecuteParameter, TExecuteParameter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public Command(Action execute, Func<bool> canExecute = null, object tag = null)
            : base(null, execute, null, canExecute, tag) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public Command(Action<TExecuteParameter> execute, Func<TExecuteParameter, bool> canExecute = null, object tag = null)
            : base(execute, null, canExecute, null, tag) { }
    }

    /// <summary>
    /// Implements the <see cref="Command{TExecuteParameter, TCanExecuteParameter}"/> class with <see cref="Object"/> as generic types.
    /// </summary>
    public class Command : Command<object, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}"/> class.
        /// </summary>
        /// <param name="execute">The action to execute.</param>
        /// <param name="canExecute">The function to call to determine wether the command can be executed.</param>
        /// <param name="tag">The tag of the command.</param>
        public Command(Action execute, Func<bool> canExecute = null, object tag = null)
            : base(execute, canExecute, tag) { }
    }
}
