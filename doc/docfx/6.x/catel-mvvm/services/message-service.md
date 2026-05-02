---
title: "MessageService" 
---
The `IMessageService` allows a developer to show message boxes from a view model.

## Showing a message

To show a message from a view model, use the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var messageService = dependencyResolver.Resolve<IMessageService>();
await messageService.Show("My first message via the service");
```

## Showing an error

Showing a warning or error is very easy. Use the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var messageService = dependencyResolver.Resolve<IMessageService>();
await messageService.ShowError("Whoops, something went wrong");
```

## Requesting confirmation

It is also possible to request confirmation from the user. The number of possibilities depends on the target platform (for example, not all platforms support `YesNo`).

The following code must be used to request confirmation:

```
var dependencyResolver = this.GetDependencyResolver();
var messageService = dependencyResolver.Resolve<IMessageService>();
if (await messageService.Show("Are you sure you want to do this?", "Are you sure?", MessageButton.YesNo) == MessageResult.Yes)
{
    // Do it!
}
```

## Asynchronous confirmation

```
var dependencyResolver = this.GetDependencyResolver();
var messageService = dependencyResolver.Resolve<IMessageService>();
await messageService.Show("Are you sure you want to do this?", "Are you sure?", MessageButton.YesNo, OnMessageServiceComplete);
```

There are two possible callbacks, one with a result of type `Func\<MessageResult\>` or one without a result of type Action.

