---
title: "Auditing" 
---
There are many lightweight MVVM frameworks out there, which work great for the basics. However, if you are writing larger enterprise applications, notifying the UI of changed properties is not enough. For example, did you think about Command Authentication? Or what about sensor emulation for Windows Phone 7 (that Microsoft do not provide)?

## Why auditing

There are many reasons why auditing should be added to an application. Most developers only add auditing to the database, but below are several reasons to add auditing to the client as well:

- Logging (what user did what on specific moments)
- Gather statistics (which views (view models) are used most)
- See what features of your software are being used by checking if anyone is actually invoking specific commands
- Measure performance (how long does it take to update specific properties or why is a specific view-model so slow?)

With the auditing capabilities of Catel, you can create and register custom auditors that can handled changes and events of view models. This way, you can gather many statistics or any information that you want to gather about the user experience. Below is a list of events that can be handled:

- OnViewModelCreating
- OnViewModelCreated
- OnPropertyChanging
- OnPropertyChanged
- OnCommandExecuting
- OnViewModelSaving
- OnViewModelSaved
- OnViewModelCanceling
- OnViewModelCanceled
- OnViewModelClosing
- OnViewModelClosed

The developer has all the freedom to handle one or more methods in an auditor. multiple auditors are possible as well.

## Creating an auditor

Creating a new auditor is simple. Create a new class, derive from AuditorBase and override the methods you are interested in. The class example tracks the event to a fake analytics framework.

```
/// <summary>
/// Logs all commands to a custom analytics service.
/// </summary>
public class CommandAuditor : AuditorBase
{
    private Analytics _analytics = new Analytics();
    /// <summary>
    /// Called when a command of a view model has just been executed.
    /// </summary>
    /// <param name="viewModel">The view model.</param>
    /// <param name="commandName">Name of the command, which is the name of the command property.</param>
    /// <param name="command">The command that has been executed.</param>
    /// <param name="commandParameter">The command parameter.</param>
    public override void OnCommandExecuted(IViewModel viewModel, string commandName, ICatelCommand command, object commandParameter)
    {
        _analytics.TrackEvent(viewModel.GetType(), "commandName");
    }
}
```

## Registering an auditor

Registering a new auditor is extremely easy as shown, in the code below:

```
AuditingManager.RegisterAuditor(new CommandAuditor());
```

