namespace Catel.MVVM;

using System;
using System.Reflection;
using Catel.Logging;
using Catel.Reflection;
using Catel.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static partial class IServiceCollectionExtensions
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(IServiceCollectionExtensions));

    public static void AddCommandWithInputGesture(this IServiceCollection serviceCollection, 
        Type containerType, string commandNameFieldName)
    {
        ArgumentNullException.ThrowIfNull(containerType);
        Argument.IsNotNullOrWhitespace("commandNameFieldName", commandNameFieldName);

        Logger.LogDebug($"Creating command '{commandNameFieldName}'");

        // Note: we must store binding flags inside variable otherwise invalid IL will be generated
        var bindingFlags = BindingFlags.Public | BindingFlags.Static;
        var commandNameField = containerType.GetFieldEx(commandNameFieldName, bindingFlags);
        if (commandNameField is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Command '{CommandName}' is not available on container type '{TypeName}'",
                commandNameFieldName, containerType.GetSafeFullName(false));
        }

        var commandName = (string?)commandNameField.GetValue(null);
        if (commandName is null)
        {
            throw Logger.LogErrorAndCreateException<CatelException>($"Command name is not valid on on container type '{containerType.GetSafeFullName()}'");
        }

        InputGesture? commandInputGesture = null;
        var inputGestureField = containerType.GetFieldEx($"{commandNameFieldName}InputGesture", bindingFlags);
        if (inputGestureField is not null)
        {
            commandInputGesture = inputGestureField.GetValue(null) as InputGesture;
        }

        serviceCollection.AddSingleton<CommandContainerRegistration>(x =>
        {
            return new CommandContainerRegistration(commandName, commandInputGesture,
                x.GetRequiredService<ICommandManager>(), x);
        });
    }
}
