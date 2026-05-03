---
title: "Commands" 
---
Commands are supported in Catel. The base class for commands is `Command`.

## Code snippets

- vmcommand - declare a command on a view model
- vmcommandwithcanexecute - declare a command with support for CanExecute on a view model

## Explanation

To implement commands, and still be able to unit test the view models, a separate command is introduced. This command allows a developer to implement a command that can be invoked both via code (unit testing) and UI.

There are several classes to represent a command:

- `Command\<TCanExecuteParameter, TExecuteParameter\>` - synchronous commands
- `TaskCommand\<TCanExecuteParameter, TExecuteParameter\>` - asynchronous commands

The `TCanExecuteParameter` is the parameter that is passed to the `CanExecute` of the command, ands saves the developer from casting the object (as in the interface `ICommand` to a typed object). The same goes for `TExecuteParameter` which makes the `Execute` of the command typed.

## Examples

Xaml (assuming that the view model is set as datacontext):

```
<Button Content="Click me" Command="{Binding MyCommand}" />
```

### Async

Code:

```
private readonly IMessageService _messageService;

public void MyViewModel(IMessageService messageService)
{
    Argument.IsNotNull(() => messageService);

    _messageService = messageService;

    // Add commands
    MyAction = new TaskCommand(MyAction_ExecuteAsyc);
}

/// <summary>
/// Gets the MyAction command.
/// </summary>
public TaskCommand MyAction { get; private set; }

/// <summary>
/// Method to invoke when the MyAction command is executed.
/// </summary>
/// <param name="parameter">The parameter of the command.</param>
private async Task MyAction_ExecuteAsync(object parameter)
{
    // Show message box
    await _messageService.ShowInfoAsync("My action in MVVM");
}
```

### Sync

Code:

```
private readonly IMessageService _messageService;

public void MyViewModel(IMessageService messageService)
{
    Argument.IsNotNull(() => messageService);

    _messageService = messageService;

    // Add commands
    MyAction = new Command(MyAction_Execute);
}

/// <summary>
/// Gets the MyAction command.
/// </summary>
public Command MyAction { get; private set; }

/// <summary>
/// Method to invoke when the MyAction command is executed.
/// </summary>
/// <param name="parameter">The parameter of the command.</param>
private void MyAction_Execute(object parameter)
{
    // Show message box
    _messageService.ShowInfo("My action in MVVM");
}
```

