// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Catel.Logging;

#if !XAMARIN && !XAMARIN_FORMS
    using InputGesture = Catel.Windows.Input.InputGesture;

#if NETFX_CORE
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
    using global::Windows.UI.Xaml;
#else 
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
#endif

#endif

    /// <summary>
    /// Manager that takes care of application-wide commands and can dynamically forward
    /// them to the right view models.
    /// </summary>
    public partial class CommandManager : ICommandManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _lockObject = new object();
        private readonly Dictionary<string, ICompositeCommand> _commands = new Dictionary<string, ICompositeCommand>();

#if !XAMARIN && !XAMARIN_FORMS
        private readonly Dictionary<string, InputGesture> _originalCommandGestures = new Dictionary<string, InputGesture>();
        private readonly Dictionary<string, InputGesture> _commandGestures = new Dictionary<string, InputGesture>();

        private bool _suspendedKeyboardEvents;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        public CommandManager()
        {
#if !XAMARIN && !XAMARIN_FORMS
            SubscribeToKeyboardEvents();
#endif
        }

        #region Properties
#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Gets or sets a value indicating whether the keyboard events are suspended.
        /// </summary>
        /// <value><c>true</c> if the keyboard events are suspended; otherwise, <c>false</c>.</value>
        public bool IsKeyboardEventsSuspended
        {
            get { return _suspendedKeyboardEvents; }
            set
            {
                if (_suspendedKeyboardEvents == value)
                {
                    return;
                }

                _suspendedKeyboardEvents = value;

                if (value)
                {
                    Log.Debug("Suspended keyboard events");
                }
                else
                {
                    Log.Debug("Resumed keyboard events");
                }
            }
        }
#endif
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a command has been created.
        /// </summary>
        public event EventHandler<CommandCreatedEventArgs> CommandCreated;
        #endregion

#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Creates the command inside the command manager.
        /// <para />
        /// If the <paramref name="throwExceptionWhenCommandIsAlreadyCreated"/> is <c>false</c> and the command is already created, only
        /// the input gesture is updated for the existing command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="inputGesture">The input gesture.</param>
        /// <param name="compositeCommand">The composite command. If <c>null</c>, this will default to a new instance of <see cref="CompositeCommand" />.</param>
        /// <param name="throwExceptionWhenCommandIsAlreadyCreated">if set to <c>true</c>, this method will throw an exception when the command is already created.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="CreateCommand" /> method.</exception>
        public void CreateCommand(string commandName, InputGesture inputGesture = null, ICompositeCommand compositeCommand = null,
            bool throwExceptionWhenCommandIsAlreadyCreated = true)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                Log.Debug("Creating command '{0}' with input gesture '{1}'", commandName, ObjectToStringHelper.ToString(inputGesture));

                if (_commands.ContainsKey(commandName))
                {
                    var error = $"Command '{commandName}' is already created using the CreateCommand method";
                    Log.Error(error);

                    if (throwExceptionWhenCommandIsAlreadyCreated)
                    {
                        throw new InvalidOperationException(error);
                    }

                    _commandGestures[commandName] = inputGesture;
                    return;
                }

                if (compositeCommand == null)
                {
                    compositeCommand = new CompositeCommand();
                }

                _commands.Add(commandName, compositeCommand);
                _originalCommandGestures.Add(commandName, inputGesture);
                _commandGestures.Add(commandName, inputGesture);

                CommandCreated.SafeInvoke(this, () => new CommandCreatedEventArgs(compositeCommand, commandName));
            }
        }
#else
        /// <summary>
        /// Creates the command inside the command manager.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="compositeCommand">The composite command. If <c>null</c>, this will default to a new instance of <see cref="CompositeCommand"/>.</param>
        /// <param name="throwExceptionWhenCommandIsAlreadyCreated">if set to <c>true</c>, this method will throw an exception when the command is already created.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="CreateCommand" /> method.</exception>
        public void CreateCommand(string commandName, ICompositeCommand compositeCommand = null,
            bool throwExceptionWhenCommandIsAlreadyCreated = true)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                Log.Debug("Creating command '{0}'", commandName);

                if (_commands.ContainsKey(commandName))
                {
                    var error = string.Format("Command '{0}' is already created using the CreateCommand method", commandName);
                    Log.Error(error);

                    if (throwExceptionWhenCommandIsAlreadyCreated)
                    {
                        throw new InvalidOperationException(error);
                    }

                    return;
                }

                if (compositeCommand == null)
                {
                    compositeCommand = new CompositeCommand();
                }

                _commands.Add(commandName, compositeCommand);

                InvalidateCommands();

                CommandCreated.SafeInvoke(this, () => new CommandCreatedEventArgs(compositeCommand, commandName));
            }
        }
#endif

        /// <summary>
        /// Invalidates the all the currently registered commands.
        /// </summary>
        public void InvalidateCommands()
        {
            lock (_lockObject)
            {
                foreach (var commandName in _commands.Keys)
                {
                    var command = _commands[commandName];
                    if (command != null)
                    {
                        command.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the registered commands.
        /// </summary>
        /// <returns>The names of the commands.</returns>
        public IEnumerable<string> GetCommands()
        {
            lock (_lockObject)
            {
                return _commands.Keys.ToList();
            }
        }

        /// <summary>
        /// Gets the command created with the command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>The <see cref="ICommand"/> or <c>null</c> if the command is not created.</returns>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        public ICommand GetCommand(string commandName)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                if (_commands.ContainsKey(commandName))
                {
                    return _commands[commandName];
                }

                return null;
            }
        }

        /// <summary>
        /// Determines whether the specified command name is created.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns><c>true</c> if the specified command name is created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        public bool IsCommandCreated(string commandName)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                return _commands.ContainsKey(commandName);
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void ExecuteCommand(string commandName)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                Log.Debug("Executing command '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commands[commandName].Execute(null);
            }
        }

        /// <summary>
        /// Registers a command with the specified command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void RegisterCommand(string commandName, ICommand command, IViewModel viewModel = null)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("command", command);

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            lock (_lockObject)
            {
                Log.Debug("Registering command to '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commands[commandName].RegisterCommand(command, viewModel);

                InvalidateCommands();
            }
        }

        /// <summary>
        /// Registers the action with the specified command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void RegisterAction(string commandName, Action action)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("action", action);

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            lock (_lockObject)
            {
                Log.Debug("Registering action to '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commands[commandName].RegisterAction(action);

                InvalidateCommands();
            }
        }

        /// <summary>
        /// Registers the action with the specified command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void RegisterAction(string commandName, Action<object> action)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("action", action);

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            lock (_lockObject)
            {
                Log.Debug("Registering action to '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commands[commandName].RegisterAction(action);

                InvalidateCommands();
            }
        }

        /// <summary>
        /// Unregisters a command with the specified command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void UnregisterCommand(string commandName, ICommand command)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("command", command);

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            lock (_lockObject)
            {
                Log.Debug("Unregistering command from '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commands[commandName].UnregisterCommand(command);

                InvalidateCommands();
            }
        }

        /// <summary>
        /// Unregisters the action with the specified command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void UnregisterAction(string commandName, Action action)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("action", action);

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            lock (_lockObject)
            {
                Log.Debug("Unregistering action from '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commands[commandName].UnregisterAction(action);

                InvalidateCommands();
            }
        }

        /// <summary>
        /// Unregisters the action with the specified command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void UnregisterAction(string commandName, Action<object> action)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("action", action);

            if (CatelEnvironment.IsInDesignMode)
            {
                return;
            }

            lock (_lockObject)
            {
                Log.Debug("Unregistering action from '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commands[commandName].UnregisterAction(action);

                InvalidateCommands();
            }
        }

#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Gets the original input gesture with which the command was initially created.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>The input gesture or <c>null</c> if there is no input gesture for the specified command.</returns>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public InputGesture GetOriginalInputGesture(string commandName)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                return _originalCommandGestures[commandName];
            }
        }

        /// <summary>
        /// Gets the input gesture for the specified command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>The input gesture or <c>null</c> if there is no input gesture for the specified command.</returns>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public InputGesture GetInputGesture(string commandName)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                return _commandGestures[commandName];
            }
        }

        /// <summary>
        /// Updates the input gesture for the specified command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="inputGesture">The new input gesture.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void UpdateInputGesture(string commandName, InputGesture inputGesture = null)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                Log.Debug("Updating input gesture of command '{0}' to '{1}'", commandName, ObjectToStringHelper.ToString(inputGesture));

                if (!_commands.ContainsKey(commandName))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Command '{0}' is not yet created using the CreateCommand method", commandName);
                }

                _commandGestures[commandName] = inputGesture;
            }
        }

        /// <summary>
        /// Resets the input gestures to the original input gestures with which the commands were registered.
        /// </summary>
        public void ResetInputGestures()
        {
            lock (_lockObject)
            {
                Log.Info("Resetting input gestures");

                foreach (var command in _commands)
                {
                    Log.Debug("Resetting input gesture for command '{0}' to '{1}'", command.Key, _originalCommandGestures[command.Key]);

                    _commandGestures[command.Key] = _originalCommandGestures[command.Key];
                }
            }
        }

        /// <summary>
        /// Subscribes to keyboard events.
        /// </summary>
        public void SubscribeToKeyboardEvents()
        {
            SubscribeToKeyboardEventsInternal();
        }

        partial void SubscribeToKeyboardEventsInternal();
#endif
    }
}