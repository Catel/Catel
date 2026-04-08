namespace Catel.MVVM;

using System;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class CommandFactory : ICommandFactory
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(CommandFactory));

    private readonly IServiceProvider _serviceProvider;

    public CommandFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public virtual ICatelCommand CreateCommand(Action executeAction, Func<bool>? canExecute,
        object? tag = null)
    {
        return new Command(_serviceProvider, executeAction, canExecute, tag);
    }

    public virtual ICatelTaskCommand CreateCommand(Func<Task> executeAction, Func<bool>? canExecute,
        object? tag = null)
    {
        return new TaskCommand(_serviceProvider, executeAction, canExecute, tag);
    }
}
