---
title: "Commands authentication" 
description: ""
---
One of the questions an MVVM developer faces is how to control the executation state of a command by role or user authentication method. Catel offers an out-of-the-box solution for this problem to check the `CanExecute` state of the commands in the UI.Â 

It is very important that this way of disabling commands is only used to easy the development of consistent user interfaces. It cannot replace the actual check whether a user can or cannot modify data. The actual and final responsibility still lays at the business layer.

## Tagging your commands

To know whether a specific user can execute a command, you need to be able to distinguish one command from another. The `ICatelCommand` interface (which derives from `ICommand`) providers a `Tag` property that allows you to tag the command with any object that fits your needs. In one application, commands might be distinguished using strings, other applications use integer ID's.

A tag must be set in the constructor of a command and cannot be changed:

```
Edit = new Command(OnEditExecute, OnEditCanExecute, "editCommand");
```

## IAuthenticationProvider

The `IAuthenticationProvider` is a provider that needs to be implemented per application and must be registered in the IoC container. Below is the interface definition:

```
/// <summary>
/// Interface to allow an authentication mechanism to control the CanExecute state of a command.
/// </summary>
public interface IAuthenticationProvider
{
    /// <summary>
    /// Determines whether the specified <paramref name="command"/> can be executed. The class implementing this interface
    /// can use any required method to check the command.
    /// <para />
    /// It is recommended to use the <see cref="ICatelCommand.Tag"/> property to identify a command.
    /// </summary>
    /// <param name="command">The command that is requested.</param>
    /// <param name="commandParameter">The command parameter.</param>
    /// <returns>
    ///    <c>true</c> if this instance [can command be executed] the specified command; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The <c>CanExecute</c> state of a command is queried a lot. The command itself does not cache any results because
    /// it is not aware of role or identity changes. If caching is required, this must be implemented in the class implementing
    /// the <see cref="ICommandAuthenticationProvider"/> interface.
    /// </remarks>
    bool CanCommandBeExecuted(ICatelCommand command, object commandParameter);
}
```

To register a custom implementation of the command authentication provider, use the code below:

```
Catel.IoC.ServiceLocator.Instance.RegisterType<IAuthenticationProvider, RoleAuthenticationProvider>();
```

The code above registers a custom made command authentication provider that checks whether a specific role can execute the command.

{{% notice info %}}
Catel checks whether an `IAuthenticationProvider` is registered. If not, the way commands are handled is not affected in any way. If there is an `IAuthenticationProvider` available, the `CanExecute` state is checked, even when there is no custom `CanExecute` implemented.
{{% /notice %}}

