// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelCommandManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Input;
    using Logging;
    using Reflection;
    using CommandHandler = System.Action<IViewModel, string, System.Windows.Input.ICommand, object>;

    /// <summary>
    /// Command manager that manages the execution state of all commands of a view model.
    /// </summary>
    public class ViewModelCommandManager : IViewModelCommandManager
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary containing all instances of all view model command managers.
        /// </summary>
        private static readonly Dictionary<int, ViewModelCommandManager> _instances = new Dictionary<int, ViewModelCommandManager>();
        #endregion

        #region Fields
        /// <summary>
        /// The lock object.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// A list of registered command handlers.
        /// </summary>
        private readonly List<CommandHandler> _commandHandlers = new List<CommandHandler>();

        /// <summary>
        /// A list of commands that implement the <see cref="ICatelCommand"/> interface.
        /// </summary>
        /// <remarks>
        /// Internal so the <see cref="ViewModelManager"/> can subscribe to the commands. The string is the property name
        /// the command is registered with.
        /// </remarks>
        private readonly Dictionary<ICommand, string> _commands = new Dictionary<ICommand, string>();

        /// <summary>
        /// The view model.
        /// </summary>
        private IViewModel _viewModel;

        /// <summary>
        /// The view model type;
        /// </summary>
        private readonly Type _viewModelType;

        /// <summary>
        /// A list of reflection properties for the commands.
        /// </summary>
        private readonly List<PropertyInfo> _commandProperties = new List<PropertyInfo>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommandManager" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        private ViewModelCommandManager(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            Log.Debug("Creating a ViewModelCommandManager for view model '{0}' with unique identifier '{1}'", viewModel.GetType().FullName, viewModel.UniqueIdentifier);

            _viewModel = viewModel;
            _viewModelType = viewModel.GetType();
            _viewModel.Initialized += OnViewModelInitialized;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.Closed += OnViewModelClosed;

            var properties = new List<PropertyInfo>();
            properties.AddRange(_viewModelType.GetPropertiesEx());

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.PropertyType.ImplementsInterfaceEx(typeof(ICommand)))
                {
                    _commandProperties.Add(propertyInfo);
                }
            }

            RegisterCommands(false);

            Log.Debug("Created a ViewModelCommandManager for view model '{0}' with unique identifier '{1}'", viewModel.GetType().FullName, viewModel.UniqueIdentifier);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the commands in a specific <see cref="IViewModel" /> instance. By subscribing
        /// to all commands, the <see cref="IViewModel.CommandExecuted" /> can be intercepted.
        /// <para />
        /// This method will automatically subscribe to the <see cref="IViewModel.Closed"/> event and unsubscribe all commands
        /// at that time.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public static IViewModelCommandManager Create(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            if (viewModel.IsClosed)
            {
                Log.Warning("View model '{0}' with unique identifier '{1}' is already closed, cannot manage commands of a closed view model", viewModel.GetType().FullName, viewModel.UniqueIdentifier);
                return null;
            }

            if (!_instances.ContainsKey(viewModel.UniqueIdentifier))
            {
                _instances[viewModel.UniqueIdentifier] = new ViewModelCommandManager(viewModel);
            }

            return _instances[viewModel.UniqueIdentifier];
        }

        /// <summary>
        /// Registers the commands in a specific <see cref="IViewModel" /> instance. By subscribing
        /// to all commands, the <see cref="IViewModel.CommandExecuted" /> can be intercepted.
        /// <para />
        /// This method will automatically subscribe to the <see cref="IViewModel.Closed"/> event and unsubscribe all commands
        /// at that time.
        /// </summary>
        /// <param name="force">If <c>true</c>, the already registered commands are cleared and all are registered again.</param>
        private void RegisterCommands(bool force)
        {
            if (_commandProperties.Count == 0)
            {
                return;
            }

            lock (_lock)
            {
                if (_commands.Count > 0)
                {
                    if (!force)
                    {
                        return;
                    }

                    UnregisterCommands();
                }

                Log.Debug("Registering commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, _viewModel.UniqueIdentifier);

                foreach (var propertyInfo in _commandProperties)
                {
                    try
                    {
                        var command = propertyInfo.GetValue(_viewModel, null) as ICommand;
                        if (command != null)
                        {
                            Log.Debug("Found command '{0}' on view model '{1}'", propertyInfo.Name, _viewModelType.Name);

                            var commandAsICatelCommand = command as ICatelCommand;
                            if (commandAsICatelCommand != null)
                            {
                                commandAsICatelCommand.Executed += OnViewModelCommandExecuted;
                            }

                            _commands.Add(command, propertyInfo.Name);
                        }
                    }
                    catch (Exception)
                    {
                        Log.Error("Failed to get command from property '{0}'", propertyInfo.Name);
                    }
                }

                Log.Debug("Registered commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, _viewModel.UniqueIdentifier);
            }
        }

        /// <summary>
        /// Adds a new handler when a command is executed on the specified view model.
        /// </summary>
        /// <param name="handler">The handler to execute when a command is executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public void AddHandler(CommandHandler handler)
        {
            Argument.IsNotNull("handler", handler);

            _commandHandlers.Add(handler);
        }

        /// <summary>
        /// Invalidates all the commands that implement the <see cref="ICatelCommand"/>.
        /// </summary>
        /// <param name="force">If <c>true</c>, the commands are re-initialized. The default value is <c>false</c>.</param>
        public void InvalidateCommands(bool force = false)
        {
            // Safe to call, checks whether commands are already registered
            RegisterCommands(force);

            lock (_lock)
            {
                foreach (var command in _commands.Keys)
                {
                    var commandAsICatelCommand = command as ICatelCommand;
                    if (commandAsICatelCommand != null)
                    {
                        commandAsICatelCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the commands in the <see cref="IViewModel" /> instance.
        /// </summary>
        private void UnregisterCommands()
        {
            Log.Debug("Unregistering commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, _viewModel.UniqueIdentifier);

            lock (_lock)
            {
                foreach (var command in _commands.Keys)
                {
                    var commandAsICatelCommand = command as ICatelCommand;
                    if (commandAsICatelCommand != null)
                    {
                        commandAsICatelCommand.Executed -= OnViewModelCommandExecuted;
                    }
                }

                _commands.Clear();
            }

            Log.Debug("Unregistered commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, _viewModel.UniqueIdentifier);
        }

        private void OnViewModelCommandExecuted(object sender, CommandExecutedEventArgs e)
        {
            lock (_lock)
            {
                foreach (var handler in _commandHandlers)
                {
                    if (handler != null)
                    {
                        handler(_viewModel, _commands[e.Command], e.Command, e.CommandParameter);
                    }
                }
            }
        }

        private void OnViewModelInitialized(object sender, EventArgs e)
        {
            InvalidateCommands(true);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RegisterCommands(false);
        }

        private void OnViewModelClosed(object sender, EventArgs e)
        {
            _instances.Remove(_viewModel.UniqueIdentifier);

            _commandHandlers.Clear();

            UnregisterCommands();

            _viewModel.Initialized -= OnViewModelInitialized;
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _viewModel.Closed -= OnViewModelClosed;
            _viewModel = null;
        }
        #endregion
    }
}