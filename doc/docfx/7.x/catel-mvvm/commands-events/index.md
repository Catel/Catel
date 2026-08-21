---
title: "Commands & events" 
---
Commanding is supported by Catel. Catel supports Command classes, which is also known as `RelayCommand` or `DelegateCommand` in other frameworks. Defining a command on a view model is easy, as shown, in the code below:

```
// TODO: Move code below to constructor
Edit = new Command(OnEditExecute, OnEditCanExecute);
// TODO: Move code above to constructor

/// <summary>
/// Gets the Edit command.
/// </summary>
public Command Edit { get; private set; }

/// <summary>
/// Method to check whether the Edit command can be executed.
/// </summary>
private bool OnEditCanExecute()
{
    return true;
}

/// <summary>
/// Method to invoke when the Edit command is executed.
/// </summary>
private void OnEditExecute()
{
    // TODO: Handle command logic here
}
```

There are some people who do not like the `ICommand` implementations. For example, Caliburn (Micro) uses convention and does not require the creation of a command. There are a few downsides for that:

- It requires you to ensure the name of the control is the same as the method;
- It is not clear that it is actually a command if you are not fully familiar with the conventions;
- The methods need to be public (otherwise, how are you going to invoke the commands during unit tests?), which make them freely available (and that is not something we like);
- You will always have to invoke CanExecute yourself again in Execute, because you have no guarantee that the source of Execute is actually the convention mapping;
- There is no way to manually refresh the CanExecute state on the bound controls.

