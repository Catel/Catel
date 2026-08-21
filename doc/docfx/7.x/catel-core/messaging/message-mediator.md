---
title: "Message mediator" 
---
Catel allows sending messages to unknown targets by implementing the mediator pattern. The mediator is assured memory leak free, and can be used safely in any .NET environment. Below are a few usage examples of the `MessageMediator` class.

Inject `IMessageMediator` via the constructor:

```csharp
private readonly IMessageMediator _messageMediator;

public MyViewModel(IServiceProvider serviceProvider, IMessageMediator messageMediator)
    : base(serviceProvider)
{
    _messageMediator = messageMediator;
}
```

## Registering to a message

To register a handler for a specific message type, in this case a string, use the following code:

```csharp
_messageMediator.Register<string>(this, OnMessage);
```

## Sending out a message

To send a message to all recipients, use the following code:

```csharp
_messageMediator.SendMessage<string>("message");
```

## Sending out a message with a tag

Sometimes, you want to send messages only based on a tag. For example, you want to let other view models know that you just added a person. All recipients that registered to the string message type with the Person tag will receive the message:

```csharp
_messageMediator.SendMessage<string>("Person added", "Person");
```


