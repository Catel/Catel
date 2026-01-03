namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel.Data;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Reflection;
    using AsyncCommandHandler = System.Func<IViewModel, string, System.Windows.Input.ICommand, object?, System.Threading.Tasks.Task>;
    using CommandHandler = System.Action<IViewModel, string, System.Windows.Input.ICommand, object?>;

    /// <summary>
    /// Command manager that manages the execution state of all commands of a view model.
    /// </summary>
    public class ViewModelCommandManager : IViewModelCommandManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(ViewModelCommandManager));

        /// <summary>
        /// The lock object.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// A list of registered command handlers.
        /// </summary>
        private readonly List<CommandHandler> _commandHandlers = new List<CommandHandler>();

        /// <summary>
        /// A list of registered command handlers.
        /// </summary>
        private readonly List<AsyncCommandHandler> _asyncCommandHandlers = new List<AsyncCommandHandler>();

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
        private readonly IViewModel _viewModel;

        /// <summary>
        /// The view model type;
        /// </summary>
        private readonly Type _viewModelType;

        /// <summary>
        /// A list of reflection properties for the commands.
        /// </summary>
        private readonly List<PropertyInfo> _commandProperties = new List<PropertyInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommandManager" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public ViewModelCommandManager(IViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            Logger.LogDebug("Creating a ViewModelCommandManager for view model '{0}' with unique identifier '{1}'", viewModel.GetType().FullName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));

            _viewModel = viewModel;
            _viewModelType = viewModel.GetType();
            _viewModel.InitializedAsync += OnViewModelInitializedAsync;
            _viewModel.ClosedAsync += OnViewModelClosedAsync;

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

            Logger.LogDebug("Created a ViewModelCommandManager for view model '{0}' with unique identifier '{1}'", viewModel.GetType().FullName, BoxingCache.GetBoxedValue(viewModel.UniqueIdentifier));
        }

        /// <summary>
        /// Registers the commands in a specific <see cref="IViewModel" /> instance. By subscribing
        /// to all commands, the <see cref="IViewModel.CommandExecutedAsync" /> can be intercepted.
        /// <para />
        /// This method will automatically subscribe to the <see cref="IViewModel.ClosedAsync"/> event and unsubscribe all commands
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
                if (_viewModel is null)
                {
                    return;
                }

                if (_commands.Count > 0)
                {
                    if (!force)
                    {
                        return;
                    }

                    UnregisterCommands();
                }

                Logger.LogDebug("Registering commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, BoxingCache.GetBoxedValue(_viewModel.UniqueIdentifier));

                foreach (var propertyInfo in _commandProperties)
                {
                    try
                    {
                        var command = propertyInfo.GetValue(_viewModel, null) as ICommand;
                        if (command is not null)
                        {
                            if (!_commands.ContainsKey(command))
                            {
                                Logger.LogDebug("Found command '{0}' on view model '{1}'", propertyInfo.Name, _viewModelType.Name);

                                var commandAsICatelCommand = command as ICatelCommand;
                                if (commandAsICatelCommand is not null)
                                {
                                    commandAsICatelCommand.Executed += OnViewModelCommandExecuted;
                                }

                                _commands.Add(command, propertyInfo.Name);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Logger.LogError("Failed to get command from property '{0}'", propertyInfo.Name);
                    }
                }

                Logger.LogDebug("Registered commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, BoxingCache.GetBoxedValue(_viewModel.UniqueIdentifier));
            }
        }

        /// <summary>
        /// Adds a new handler when a command is executed on the specified view model.
        /// </summary>
        /// <param name="handler">The handler to execute when a command is executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public void AddHandler(AsyncCommandHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            lock (_lock)
            {
                _asyncCommandHandlers.Add(handler);
            }
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
                    if (commandAsICatelCommand is not null)
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
            Logger.LogDebug("Unregistering commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, BoxingCache.GetBoxedValue(_viewModel.UniqueIdentifier));

            lock (_lock)
            {
                foreach (var command in _commands.Keys)
                {
                    var commandAsICatelCommand = command as ICatelCommand;
                    if (commandAsICatelCommand is not null)
                    {
                        commandAsICatelCommand.Executed -= OnViewModelCommandExecuted;
                    }
                }

                _commands.Clear();
            }

            Logger.LogDebug("Unregistered commands on view model '{0}' with unique identifier '{1}'", _viewModelType.FullName, BoxingCache.GetBoxedValue(_viewModel.UniqueIdentifier));
        }

#pragma warning disable AvoidAsyncVoid // Avoid async void
        private async void OnViewModelCommandExecuted(object? sender, CommandExecutedEventArgs e)
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            CommandHandler[] syncHandlers;
            AsyncCommandHandler[] asyncHandlers;
            var commandName = string.Empty;

            lock (_lock)
            {
                syncHandlers = _commandHandlers.ToArray();
                asyncHandlers = _asyncCommandHandlers.ToArray();

                if (!_commands.TryGetValue(e.Command, out commandName))
                {
                    commandName = "unknown";
                }
            }

            foreach (var handler in syncHandlers)
            {
                if (handler is not null)
                {
                    handler(_viewModel, commandName, e.Command, e.CommandParameter);
                }
            }

            foreach (var handler in asyncHandlers)
            {
                if (handler is not null)
                {
                    await handler(_viewModel, commandName, e.Command, e.CommandParameter);
                }
            }
        }

        private Task OnViewModelInitializedAsync(object? sender, EventArgs e)
        {
            InvalidateCommands(true);

            return Task.CompletedTask;
        }

        private Task OnViewModelClosedAsync(object? sender, EventArgs e)
        {
            lock (_lock)
            {
                _commandHandlers.Clear();

                UnregisterCommands();

                _viewModel.InitializedAsync -= OnViewModelInitializedAsync;
                _viewModel.ClosedAsync -= OnViewModelClosedAsync;
            }

            return Task.CompletedTask;
        }
    }
}
