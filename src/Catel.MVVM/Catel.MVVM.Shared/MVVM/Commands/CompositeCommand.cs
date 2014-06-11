// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Composite command which allows several commands inside a single command being exposed to a view.
    /// </summary>
    public class CompositeCommand : Command, ICompositeCommand
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();
        private readonly List<CommandInfo> _commandInfo = new List<CommandInfo>();
        private readonly List<Action> _actions = new List<Action>();
        private readonly List<Action<object>> _actionsWithParameter = new List<Action<object>>();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}" /> class.
        /// </summary>
        public CompositeCommand()
            : base(null)
        {
            AllowPartialExecution = false;

            InitializeActions(ExecuteCompositeCommand, null, CanExecuteCompositeCommand, null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether this command should check the can execute of all commands to determine can execute for composite command.
        /// <para />
        /// The default value is <c>true</c> which means the composite command can only be executed if all commands can be executed. If
        /// there is a requirement to allow partial invocation, set this property to false.
        /// </summary>
        /// <value>The check can execute of all commands to determine can execute for composite command.</value>
        public bool CheckCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand
        {
            get { return !AllowPartialExecution; }
            set { AllowPartialExecution = !value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether partial execution of commands is allowed. If this value is <c>true</c>, this composite
        /// command will always be executable and only invoke the internal commands that are executable.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if partial execution is allowed; otherwise, <c>false</c>.</value>
        public bool AllowPartialExecution { get; set; }
        #endregion

        #region Methods
        private void ExecuteCompositeCommand(object parameter)
        {
            lock (_lock)
            {
                var commands = (from commandInfo in _commandInfo
                                select commandInfo.Command).ToList();

                foreach (var command in commands)
                {
                    try
                    {
                        if (command != null)
                        {
                            if (command.CanExecute(parameter))
                            {
                                command.Execute(parameter);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to execute one of the commands in the composite commands, execution will continue");
                    }
                }

                var actions = _actions.ToList();
                foreach (var action in actions)
                {
                    try
                    {
                        if (action != null)
                        {
                            action();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to execute one of the actions in the composite commands, execution will continue");
                    }
                }

                var actionsWithParameter = _actionsWithParameter.ToList();
                foreach (var actionWithParameter in actionsWithParameter)
                {
                    try
                    {
                        if (actionWithParameter != null)
                        {
                            actionWithParameter(parameter);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to execute one of the actions in the composite commands, execution will continue");
                    }
                }
            }
        }

        private bool CanExecuteCompositeCommand(object parameter)
        {
            lock (_lock)
            {
                if (!AllowPartialExecution)
                {
                    var commands = (from commandInfo in _commandInfo
                                    select commandInfo.Command).ToList();

                    foreach (var command in commands)
                    {
                        if (command != null)
                        {
                            if (!command.CanExecute(parameter))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }

                return true;
            }
        }

        /// <summary>
        /// Registers the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model. If specified, the command will automatically be unregistered when the view model is closed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Note that if the view model is not specified, the command must be unregistered manually in order to prevent memory leaks.
        /// </remarks>
        public void RegisterCommand(ICatelCommand command, IViewModel viewModel = null)
        {
            Argument.IsNotNull("command", command);

            lock (_lock)
            {
                var commandInfo = new CommandInfo(this, command, viewModel);

                _commandInfo.Add(commandInfo);
                command.CanExecuteChanged += OnCommandCanExecuteChanged;

                Log.Debug("Registered command in CompositeCommand");
            }
        }

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void RegisterAction(Action action)
        {
            Argument.IsNotNull("action", action);

            lock (_lock)
            {
                _actions.Add(action);

                Log.Debug("Registered action in CompositeCommand");
            }
        }

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void RegisterAction(Action<object> action)
        {
            Argument.IsNotNull("action", action);

            lock (_lock)
            {
                _actionsWithParameter.Add(action);

                Log.Debug("Registered action<object> in CompositeCommand");
            }
        }

        /// <summary>
        /// Unregisters the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public void UnregisterCommand(ICatelCommand command)
        {
            Argument.IsNotNull("command", command);

            lock (_lock)
            {
                for (int i = _commandInfo.Count - 1; i >= 0; i--)
                {
                    var commandInfo = _commandInfo[i];

                    if (ReferenceEquals(commandInfo.Command, command))
                    {
                        command.CanExecuteChanged -= OnCommandCanExecuteChanged;
                        _commandInfo.RemoveAt(i);

                        Log.Debug("Unregistered command from CompositeCommand");
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void UnregisterAction(Action action)
        {
            Argument.IsNotNull("action", action);

            lock (_lock)
            {
                for (int i = _actions.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_actions[i], action))
                    {
                        _actions.RemoveAt(i);

                        Log.Debug("Unregistered action from CompositeCommand");
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void UnregisterAction(Action<object> action)
        {
            Argument.IsNotNull("action", action);

            lock (_lock)
            {
                for (int i = _actionsWithParameter.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_actionsWithParameter[i], action))
                    {
                        _actionsWithParameter.RemoveAt(i);

                        Log.Debug("Unregistered action<object> from CompositeCommand");
                    }
                }
            }
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }
        #endregion

        #region Nested type: CommandInfo
        private class CommandInfo
        {
            private readonly CompositeCommand _compositeCommand;

            #region Constructors
            public CommandInfo(CompositeCommand compositeCommand, ICatelCommand command, IViewModel viewModel)
            {
                _compositeCommand = compositeCommand;

                Command = command;
                ViewModel = viewModel;

                if (viewModel != null)
                {
                    viewModel.Closed += OnViewModelClosed;
                }
            }
            #endregion

            #region Properties
            public ICatelCommand Command { get; private set; }
            public IViewModel ViewModel { get; private set; }
            #endregion

            private void OnViewModelClosed(object sender, ViewModelClosedEventArgs e)
            {
                Log.Debug("ViewModel '{0}' is closed, automatically unregistering command from CompositeCommand", ViewModel);

                _compositeCommand.UnregisterCommand(Command);

                ViewModel.Closed -= OnViewModelClosed;
                ViewModel = null;
            }
        }
        #endregion
    }
}