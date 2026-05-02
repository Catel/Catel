---
title: "Message mediator" 
---
Catel allows sending messages to unknown targets by implementing the mediator pattern. The mediator is assured memory leak free, and can be used safely in any .NET environment (even ASP.NET). Below are a few usage examples of the MessageMediator class.

## Registering to a message

To register a handler for a specific message type, in this case a string, use the following code:

```
var mediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
mediator.Register<string>(this, OnMessage);
```

## Sending out a message

To send a message to all recipients, use the following code:

```
var mediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
mediator.SendMessage<string>("message");
```

## Sending out a message with a tag

Sometimes, you want to send messages only based on a tag. For example, you want to let other view models know that you just added a person. All recipients that registered to the string message type with the Person tag will receive the message:

```
var mediator = ServiceLocator.Default.ResolveType<IMessageMediator>();
mediator.SendMessage<string>("Person added", "Person");
```

