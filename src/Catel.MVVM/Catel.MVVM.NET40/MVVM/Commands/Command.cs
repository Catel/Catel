// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Command.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// Both Command and CompositeCommand implement ICommand
#pragma warning disable 1956

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    using Catel.MVVM.Services;

    using IoC;

    /// <summary>
    /// Base class for generic command classes. Contains protected static services for using in derived classes.
    /// </summary>
    public abstract class CommandBase
    {
        /// <summary>
        /// Authentication provider.
        /// </summary>
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

            AuthenticationProvider = dependencyResolver.Resolve<IAuthenticationProvider>();
            DispatcherService = dependencyResolver.Resolve<IDispatcherService>();
        }
    }

    /// <summary>
    /// Class to implement commands in the <see cref="ViewModelBase"/>.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the can execute parameter.</typeparam>
    public class Command<TExecuteParameter, TCanExecuteParameter> : CommandBase, ICatelCommand
    {
        #region Fields
        private Func<TCanExecuteParameter, bool> _canExecuteWithParameter;
        private Func<bool> _canExecuteWithoutParameter;
        private Action<TExecuteParameter> _executeWithParameter;
        private Action _executeWithoutParameter;

        /// <summary>
        /// List of subscribed event handlers so the commands can be unsubscribed upon disposing.
        /// </summary>
        private readonly List<EventHandler> _subscribedEventHandlers = new List<EventHandler>();
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

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Command&lt;TExecuteParameter, TCanExecuteParameter&gt;"/> is reclaimed by garbage collection.
        /// </summary>
        ~Command()
        {
            Dispose(false);
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
#if NET
        public event EventHandler CanExecuteChanged
        {
            add
            {
                System.Windows.Input.CommandManager.RequerySuggested += value;

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

                System.Windows.Input.CommandManager.RequerySuggested -= value;
            }
        }
#else
        public event EventHandler CanExecuteChanged;
#endif

        /// <summary>
        /// Occurs when the command has just been executed successfully.
        /// </summary>
        public event EventHandler<CommandExecutedEventArgs> Executed;
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
        /// 	<c>true</c> if this command can be executed; otherwise, <c>false</c>.
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
        /// 	<c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.
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
                parameter = default(TCanExecuteParameter);
            }

            Execute((TExecuteParameter)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(TExecuteParameter parameter)
        {
            Execute(parameter, false);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <param name="ignoreCanExecuteCheck">if set to <c>true</c>, the check on <see cref="CanExecute()"/> will be used before actually executing the action.</param>
        protected virtual void Execute(TExecuteParameter parameter, bool ignoreCanExecuteCheck)
        {
            // Double check whether execution is allowed, some controls directly call Execute
            if (!ignoreCanExecuteCheck && !CanExecute(parameter))
            {
                return;
            }

            if (_executeWithParameter != null)
            {
                _executeWithParameter(parameter);
                RaiseExecuted(parameter);
            }
            else if (_executeWithoutParameter != null)
            {
                _executeWithoutParameter();
                RaiseExecuted(parameter);
            }
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            var action = new Action(() =>
            {
#if NET
                foreach (var handler in _subscribedEventHandlers)
                {
                    handler.SafeInvoke(this);
                }

                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#else
                CanExecuteChanged.SafeInvoke(this);
#endif
            });

            AutoDispatchIfRequired(action);
        }

        /// <summary>
        /// Raises the <see cref="Executed"/> event.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected void RaiseExecuted(object parameter)
        {
            var action = new Action(() => Executed.SafeInvoke(this, new CommandExecutedEventArgs(this, parameter)));

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
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                lock (_subscribedEventHandlers)
                {
#if NET
                    foreach (var eventHandler in _subscribedEventHandlers)
                    {
                        System.Windows.Input.CommandManager.RequerySuggested -= eventHandler;
                    }
#endif

                    _subscribedEventHandlers.Clear();
                }
            }
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
