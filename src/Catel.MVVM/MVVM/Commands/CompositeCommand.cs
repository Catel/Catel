namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using System.Windows.Input;

    /// <summary>
    /// Composite command which allows several commands inside a single command being exposed to a view.
    /// </summary>
#pragma warning disable CS1956 // Member implements interface member with multiple matches at run-time
    public class CompositeCommand : TaskCommand<object?, object?, ITaskProgressReport>, ICompositeCommand
#pragma warning restore CS1956 // Member implements interface member with multiple matches at run-time
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();
        private readonly List<CommandInfo> _commandInfo = new List<CommandInfo>();
        private readonly List<Action> _actions = new List<Action>();
        private readonly List<Action<object?>> _actionsWithParameter = new List<Action<object?>>();
        private readonly List<Func<Task>> _asyncActions = new List<Func<Task>>();
        private readonly List<Func<object?, Task>> _asyncActionsWithParameter = new List<Func<object?, Task>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}" /> class.
        /// </summary>
        public CompositeCommand()
            : base() // dummy action
        {
            AllowPartialExecution = false;
            AtLeastOneMustBeExecutable = true;

            InitializeAsyncActions(ExecuteCompositeCommandAsync, null, CanExecuteCompositeCommand, null);
        }

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

        /// <summary>
        /// Gets or sets a value indicating whether at least one command must be executable. This will prevent the command to be 
        /// executed without any commands.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if at least one command must be executed; otherwise, <c>false</c>.</value>
        public bool AtLeastOneMustBeExecutable { get; set; }

        private async Task ExecuteCompositeCommandAsync(object? parameter)
        {
            var commandsToExecute = new List<ICommand>();
            var actionsToExecute = new List<Action>();
            var actionsWithParameterToExecute = new List<Action<object?>>();
            var asyncActionsToExecute = new List<Func<Task>>();
            var asyncActionsWithParameterToExecute = new List<Func<object?, Task>>();

            lock (_lock)
            {
                commandsToExecute.AddRange(from commandInfo in _commandInfo
                                           select commandInfo.Command);

                actionsToExecute.AddRange(_actions);
                actionsWithParameterToExecute.AddRange(_actionsWithParameter);
                asyncActionsToExecute.AddRange(_asyncActions);
                asyncActionsWithParameterToExecute.AddRange(_asyncActionsWithParameter);
            }

            Log.Debug($"Executing '{commandsToExecute.Count}' command(s)");

            foreach (var command in commandsToExecute)
            {
                try
                {
                    if (command is not null)
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

            Log.Debug($"Executing '{actionsToExecute.Count}' action(s)");

            foreach (var action in actionsToExecute)
            {
                try
                {
                    if (action is not null)
                    {
                        action();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to execute one of the actions in the composite commands, execution will continue");
                }
            }

            Log.Debug($"Executing '{actionsWithParameterToExecute.Count}' action(s) with parameter");

            foreach (var actionWithParameter in actionsWithParameterToExecute)
            {
                try
                {
                    if (actionWithParameter is not null)
                    {
                        actionWithParameter(parameter);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to execute one of the actions in the composite commands, execution will continue");
                }
            }

            Log.Debug($"Executing '{asyncActionsToExecute.Count}' async action(s)");

            foreach (var action in asyncActionsToExecute)
            {
                try
                {
                    if (action is not null)
                    {
                        await action();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to execute one of the async actions in the composite commands, execution will continue");
                }
            }

            Log.Debug($"Executing '{asyncActionsWithParameterToExecute.Count}' async action(s) with parameter");

            foreach (var action in asyncActionsWithParameterToExecute)
            {
                try
                {
                    if (action is not null)
                    {
                        await action(parameter);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to execute one of the async actions in the composite commands, execution will continue");
                }
            }
        }

        private bool CanExecuteCompositeCommand(object? parameter)
        {
            lock (_lock)
            {
                if (!AllowPartialExecution)
                {
                    var commands = (from commandInfo in _commandInfo
                                    select commandInfo.Command).ToList();

                    foreach (var command in commands)
                    {
                        if (command is not null)
                        {
                            if (!command.CanExecute(parameter))
                            {
                                return false;
                            }
                        }
                    }
                }

                if (AtLeastOneMustBeExecutable)
                {
                    if (_actions.Count > 0 || _actionsWithParameter.Count > 0)
                    {
                        return true;
                    }

                    var commands = (from commandInfo in _commandInfo
                                    select commandInfo.Command).ToList();

                    foreach (var command in commands)
                    {
                        if (command is not null)
                        {
                            if (command.CanExecute(parameter))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the commands currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public IEnumerable<ICommand> GetCommands()
        {
            lock (_lock)
            {
                return (from command in _commandInfo
                        select command.Command).ToList();
            }
        }

        /// <summary>
        /// Gets the actions currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public IEnumerable<Action> GetActions()
        {
            lock (_lock)
            {
                return (from action in _actions
                        select action).ToList();
            }
        }

        /// <summary>
        /// Gets the actions with parameters currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public IEnumerable<Action<object?>> GetActionsWithParameter()
        {
            lock (_lock)
            {
                return (from action in _actionsWithParameter
                        select action).ToList();
            }
        }

        /// <summary>
        /// Gets the actions currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public IEnumerable<Func<Task>> GetAsyncActions()
        {
            lock (_lock)
            {
                return (from action in _asyncActions
                        select action).ToList();
            }
        }

        /// <summary>
        /// Gets the actions with parameters currently registered to this composite command.
        /// </summary>
        /// <returns>IEnumerable.</returns>
        public IEnumerable<Func<object?, Task>> GetAsyncActionsWithParameter()
        {
            lock (_lock)
            {
                return (from action in _asyncActionsWithParameter
                        select action).ToList();
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
        public void RegisterCommand(ICommand command, IViewModel? viewModel = null)
        {
            ArgumentNullException.ThrowIfNull(command);

            lock (_lock)
            {
                var commandInfo = new CommandInfo(this, command, viewModel);

                _commandInfo.Add(commandInfo);
                command.CanExecuteChanged += OnCommandCanExecuteChanged;

                Log.Debug("Registered command in CompositeCommand");
            }
        }

        /// <summary>
        /// Unregisters the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public void UnregisterCommand(ICommand command)
        {
            ArgumentNullException.ThrowIfNull(command);

            lock (_lock)
            {
                for (var i = _commandInfo.Count - 1; i >= 0; i--)
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
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void RegisterAction(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                _actions.Add(action);

                Log.Debug("Registered action in CompositeCommand");
            }
        }

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void UnregisterAction(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                for (var i = _actions.Count - 1; i >= 0; i--)
                {
                    // Check for both ReferenceEquals (original implementation) and == (to fix CTL-654)
                    if (ReferenceEquals(_actions[i], action) || _actions[i] == action)
                    {
                        _actions.RemoveAt(i);

                        Log.Debug("Unregistered action from CompositeCommand");
                    }
                }
            }
        }

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void RegisterAction(Action<object?> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                _actionsWithParameter.Add(action);

                Log.Debug("Registered action<object> in CompositeCommand");
            }
        }

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void UnregisterAction(Action<object?> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                for (var i = _actionsWithParameter.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_actionsWithParameter[i], action))
                    {
                        _actionsWithParameter.RemoveAt(i);

                        Log.Debug("Unregistered action<object> from CompositeCommand");
                    }
                }
            }
        }

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void RegisterAction(Func<Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                _asyncActions.Add(action);

                Log.Debug("Registered async action in CompositeCommand");
            }
        }

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void UnregisterAction(Func<Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                for (var i = _asyncActions.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_asyncActions[i], action))
                    {
                        _asyncActions.RemoveAt(i);

                        Log.Debug("Unregistered async action from CompositeCommand");
                    }
                }
            }
        }

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void RegisterAction(Func<object?, Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                _asyncActionsWithParameter.Add(action);

                Log.Debug("Registered async action<object> in CompositeCommand");
            }
        }

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void UnregisterAction(Func<object?, Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            lock (_lock)
            {
                for (var i = _asyncActionsWithParameter.Count - 1; i >= 0; i--)
                {
                    if (ReferenceEquals(_asyncActionsWithParameter[i], action))
                    {
                        _asyncActionsWithParameter.RemoveAt(i);

                        Log.Debug("Unregistered async action<object> from CompositeCommand");
                    }
                }
            }
        }

        private void OnCommandCanExecuteChanged(object? sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        private class CommandInfo
        {
            private readonly CompositeCommand _compositeCommand;

            public CommandInfo(CompositeCommand compositeCommand, ICommand command, IViewModel? viewModel)
            {
                _compositeCommand = compositeCommand;

                Command = command;
                ViewModel = viewModel;

                if (viewModel is not null)
                {
                    viewModel.ClosedAsync += OnViewModelClosedAsync;
                }
            }

            public ICommand Command { get; private set; }
            public IViewModel? ViewModel { get; private set; }

            private Task OnViewModelClosedAsync(object? sender, ViewModelClosedEventArgs e)
            {
                Log.Debug("ViewModel '{0}' is closed, automatically unregistering command from CompositeCommand", ViewModel);

                _compositeCommand.UnregisterCommand(Command);

                var viewModel = ViewModel;
                if (viewModel is not null)
                {
                    viewModel.ClosedAsync -= OnViewModelClosedAsync;
                    ViewModel = null;
                }

                return Task.CompletedTask;
            }
        }
    }
}
