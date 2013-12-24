﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Catel.Logging;

#if !WINDOWS_PHONE
    using InputGesture = Catel.Windows.Input.InputGesture;
#endif

#if NETFX_CORE
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
#else
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
#endif

    /// <summary>
    /// Manager that takes care of application-wide commands and can dynamically forward
    /// them to the right view models.
    /// </summary>
    public class CommandManager : ICommandManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _lockObject = new object();
        private readonly Dictionary<string, ICompositeCommand> _commands = new Dictionary<string, ICompositeCommand>();

#if !WINDOWS_PHONE

#if NET
        private bool _subscribedToApplicationActivedEvent;
#endif

        private bool _subscribedToKeyboardEvent;
        private readonly Dictionary<string, InputGesture> _commandGestures = new Dictionary<string, InputGesture>();
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManager"/> class.
        /// </summary>
        public CommandManager()
        {
#if !WINDOWS_PHONE
            SubscribeToKeyboardEvents();
#endif
        }

#if !WINDOWS_PHONE
        /// <summary>
        /// Creates the command inside the command manager.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="inputGesture">The input gesture.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="CreateCommand"/> method.</exception>
        public void CreateCommand(string commandName, InputGesture inputGesture = null)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            AddCommandInternal(commandName, inputGesture, new CompositeCommand());
        }

        /// <summary>
        /// Creates the command inside the command manager.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="inputGesture">The input gesture.</param>
        /// <param name="command">Command instance</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="ICommandManager.CreateCommand"/> method.</exception>
        public void AddCommand(string commandName, InputGesture inputGesture = null, ICompositeCommand command = null)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            AddCommandInternal(commandName, inputGesture, command);
        }

        private void AddCommandInternal(string commandName, InputGesture inputGesture, ICompositeCommand command)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                Log.Debug("Adding command '{0}' with input gesture '{1}'", commandName, ObjectToStringHelper.ToString(inputGesture));

                if (_commands.ContainsKey(commandName))
                {
                    string error = string.Format("Command '{0}' is already created using the CreateCommand or AddCommand method", commandName);
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                if (_commands.ContainsValue(command))
                {
                    string error = string.Format("Command '{0}' is already added using the AddCommand method", commandName);
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                _commands.Add(commandName, command);
                _commandGestures.Add(commandName, inputGesture);
            }
        }

#else
        /// <summary>
        /// Creates the command inside the command manager.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="CreateCommand"/> method.</exception>
        public void CreateCommand(string commandName)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
                
            AddCommandInternal(commandName, new CompositeCommand());
        }

         /// <summary>
        /// Creates the command inside the command manager.
        /// </summary>
        /// <param name="command">Command instance</param>
        /// <param name="commandName">Name of the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="CommandManager.CreateCommand"/> method.</exception>
        void AddCommand(string commandName, ICompositeCommand command);
        {
            Argument.IsNotNull("command", command);
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            AddCommandInternal(commandName, command);
        }

        private void AddCommandInternal(string commandName, ICompositeCommand command)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);

            lock (_lockObject)
            {
                Log.Debug("Adding command '{0}' with input gesture '{1}'", commandName, ObjectToStringHelper.ToString(inputGesture));

                if (_commands.ContainsKey(commandName))
                {
                    string error = string.Format("Command '{0}' is already created using the CreateCommand or AddCommand method", commandName);
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                if (_commands.ContainsValue(command))
                {
                    string error = string.Format("Command '{0}' is already added using the AddCommand method", commandName);
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                _commands.Add(commandName, command);
                _commandGestures.Add(commandName, inputGesture);
            }
        }
#endif

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
                    string error = string.Format("Command '{0}' is not yet created using the CreateCommand method", commandName);
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                _commands[commandName].Execute(null);
            }
        }

        /// <summary>
        /// Registers a command with the specified
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void RegisterCommand(string commandName, ICatelCommand command, IViewModel viewModel = null)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("command", command);

            lock (_lockObject)
            {
                Log.Debug("Registering command to '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    string error = string.Format("Command '{0}' is not yet created using the CreateCommand method", commandName);
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                _commands[commandName].RegisterCommand(command, viewModel);
            }
        }

        /// <summary>
        /// Registers a command with the specified
        /// </summary>
        /// <param name="compositeCommand">Composite Command.</param>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="compositeCommand"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CommandManager.CreateCommand"/> method.</exception>
        public void RegisterCommand(ICompositeCommand compositeCommand, ICatelCommand command, IViewModel viewModel = null)
        {
            Argument.IsNotNull("compositeCommand", compositeCommand);
            Argument.IsNotNull("command", command);

            lock (_lockObject)
            {
                Log.Debug("Registering command to '{0}'", ObjectToStringHelper.ToString(compositeCommand));

                if (!_commands.ContainsValue(compositeCommand))
                {
                    string error = string.Format("Command '{0}' is not yet created using the AddCommand method", ObjectToStringHelper.ToString(compositeCommand));
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                compositeCommand.RegisterCommand(command, viewModel);
            }
        }

        /// <summary>
        /// Unregisters a command
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CreateCommand"/> method.</exception>
        public void UnregisterCommand(string commandName, ICatelCommand command)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            Argument.IsNotNull("command", command);

            lock (_lockObject)
            {
                Log.Debug("Unregistering command from '{0}'", commandName);

                if (!_commands.ContainsKey(commandName))
                {
                    string error = string.Format("Command '{0}' is not yet created using the CreateCommand method", commandName);
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                _commands[commandName].UnregisterCommand(command);
            }
        }

        /// <summary>
        /// Unregisters a command
        /// </summary>
        /// <param name="compositeCommand">Instance of the composite command.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="compositeCommand"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CommandManager.CreateCommand"/> method.</exception>
        public void UnregisterCommand(ICompositeCommand compositeCommand, ICatelCommand command)
        {
            Argument.IsNotNull("compositeCommand", compositeCommand);
            Argument.IsNotNull("command", command);

            lock (_lockObject)
            {
                Log.Debug("Unregistering command from '{0}'", ObjectToStringHelper.ToString(compositeCommand));

                if (!_commands.ContainsValue(compositeCommand))
                {
                    string error = string.Format("Command '{0}' is not yet created using the CreateCommand method", ObjectToStringHelper.ToString(compositeCommand));
                    Log.Error(error);
                    throw new InvalidOperationException(error);
                }

                compositeCommand.UnregisterCommand(command);
            }
        }

#if !WINDOWS_PHONE
        /// <summary>
        /// Subscribes to keyboard events.
        /// </summary>
        public void SubscribeToKeyboardEvents()
        {
            if (_subscribedToKeyboardEvent)
            {
                return;
            }

#if NET
            var application = System.Windows.Application.Current;
            if (application == null)
            {
                Log.Warning("Application.Current is null, cannot subscribe to keyboard events");
                return;
            }

            var mainWindow = application.MainWindow;
            if (mainWindow == null)
            {
                if (!_subscribedToApplicationActivedEvent)
                {
                    application.Activated += OnApplicationActivated;
                    _subscribedToApplicationActivedEvent = true;

                    Log.Info("Application.MainWindow is null, cannot subscribe to keyboard events, subscribed to Application.Activated event");
                }

                return;
            }

            mainWindow.KeyDown += OnKeyDown;
            _subscribedToKeyboardEvent = true;
#elif SILVERLIGHT
            var application = System.Windows.Application.Current;
            if (application == null)
            {
                Log.Warning("Application.Current is null, cannot subscribe to keyboard events");
                return;
            }

            var rootVisual = application.RootVisual;
            if (rootVisual == null)
            {
                Log.Warning("Application.RootVisual is null, cannot subscribe to keyboard events");

                return;
            }

            rootVisual.KeyDown += OnKeyDown;
            _subscribedToKeyboardEvent = true;
#elif NETFX_CORE
    // TODO: Grab events
#endif
        }

#if NET
        private void OnApplicationActivated(object sender, EventArgs e)
        {
            var application = (System.Windows.Application)sender;
            application.Activated -= OnApplicationActivated;

            SubscribeToKeyboardEvents();
        }
#endif

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            lock (_lockObject)
            {
                foreach (var commandGesture in _commandGestures)
                {
                    if (commandGesture.Value.Matches(e))
                    {
                        ExecuteCommand(commandGesture.Key);
                    }
                }
            }
        }
#endif
    }
}