---
title: "MessageService" 
---
The `IMessageService` allows a developer to show message boxes from a view model.

## Showing a message

To show a message from a view model, inject the service via the constructor and use:

```csharp
private readonly IMessageService _messageService;

public MyViewModel(IServiceProvider serviceProvider, IMessageService messageService)
    : base(serviceProvider)
{
    _messageService = messageService;
}
```

```csharp
await _messageService.Show("My first message via the service");
```

## Showing an error

Showing a warning or error is very easy. Use the following code:

```csharp
await _messageService.ShowError("Whoops, something went wrong");
```

## Requesting confirmation

It is also possible to request confirmation from the user. The following code must be used to request confirmation:

```csharp
if (await _messageService.Show("Are you sure you want to do this?", "Are you sure?", MessageButton.YesNo) == MessageResult.Yes)
{
    // Do it!
}
```


