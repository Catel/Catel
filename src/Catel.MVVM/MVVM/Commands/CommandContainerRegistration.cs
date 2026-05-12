namespace Catel.MVVM;

using System;
using System.Linq;
using Catel.IoC;
using Catel.Logging;
using Catel.Reflection;
using Catel.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// Wrapper class to allow command containers to be registered in the service collection. The 
/// wrapper will automatically be initialized at startup and will register the command in the 
/// command manager, as well as create the command container instance.
/// </summary>
public class CommandContainerRegistration : IInitializeAtStartup
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ICommandManagerExtensions));

    private readonly string _commandName;
    private readonly InputGesture? _inputGesture;
    private readonly ICommandManager _commandManager;
    private readonly IServiceProvider _serviceProvider;

    private object? _commandContainer;

    public CommandContainerRegistration(string commandName, InputGesture? inputGesture,
        ICommandManager commandManager, IServiceProvider serviceProvider)
    {
        _commandName = commandName;
        _inputGesture = inputGesture;
        _commandManager = commandManager;
        _serviceProvider = serviceProvider;
    }

    public void Initialize()
    {
        // Note that this only gets called when the command is registered inside the service locator,
        // at this point we must register it in the command manager

        if (_commandManager.IsCommandCreated(_commandName))
        {
            Logger.LogDebug("Command '{CommandName}' is already created, skipping...", _commandName);
            return;
        }

        _commandManager.CreateCommand(_commandName, _inputGesture);

        var commandContainerName = string.Format("{0}CommandContainer", _commandName.Replace(".", string.Empty));

        // Note: even nested classes can be fetched by name
        // https://github.com/Catel/Catel/issues/1383: CommandManager.CreateCommandWithGesture does not create CommandContainer
        var commandContainerType = (from type in TypeCache.GetTypes(allowInitialization: true)
                                    where type.Name.Equals(commandContainerName, StringComparison.OrdinalIgnoreCase) 
                                    select type).FirstOrDefault();
        if (commandContainerType is null)
        {
            Logger.LogDebug("Couldn't find command container '{CommandContainer}', you will need to add a custom action or command manually in order to make the CompositeCommand useful", commandContainerName);
            return;
        }

        Logger.LogDebug("Creating command container '{CommandContainer}'", commandContainerType.GetSafeFullName(false));

        _commandContainer = ActivatorUtilities.CreateInstance(_serviceProvider, commandContainerType);
    }
}
