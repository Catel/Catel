namespace Catel.MVVM;

using System;
using System.Threading.Tasks;

public interface ICommandFactory
{
    ICatelCommand CreateCommand(Action executeAction, Func<bool>? canExecute, object? tag = null);
    ICatelTaskCommand CreateCommand(Func<Task> executeAction, Func<bool>? canExecute, object? tag = null);
}