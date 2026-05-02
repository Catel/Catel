---
title: "Messaging via attributes" 
---
The message mediator is a great way to communicate between instances in an application. It does however require to manually subscribe to and unsubscribe from classes. This issue can be bypassed using the attribute based approach. This is an alternative for registering a method in the message mediator and not be obliged to use Register\<T\> method.

## Subscribing and unsubscribing

When attributes are using inside a class, it is required to call the *MessageMediatorHelper.SubscripeRecipient*. To unsubscribe an object, it is required to call *MessageMediatorHelper.UnsubscribeRecipient*.

There are two options to decorate methods with the attribute. Either with or without tag.

## Subscribing without a tag

In this case, the mediator will send the message to all the methods that has subscribe using the attribute to receive the message and not one especially. The code below broadcasts a message without any tag. This is just regular behavior of the message mediator.

```
/// <summary>
/// Method to invoke when the command is executed.
/// </summary>
private void OnCmdExecute()
{
    var dependencyResolver = this.GetDependencyResolver();
    var mediator = dependencyResolver.Resolve<IMessageMediator>();
    mediator.SendMessage("Test Value");
}
```

If a class, for example a view model, is interested in these messages, the only thing that needs to be done is to decorate a method with the MessageRecipient attribute as shown below:

```
/// <summary>
/// Shows the message.
/// </summary>
/// <param name="value">The value.</param>
[MessageRecipient]
private void ShowMessage(string value)
{
    var dependencyResolver = this.GetDependencyResolver();
    var messageService = dependencyResolver.Resolve<IMessageService>();
    messageService.Show(value);
}
```

## Subscribing with a tag

A tag can be used to specify some sort of grouping for messages. The MessageRecipient attribute also supports this as shown in the code below. First lets take a look how to send a message and specify a tag.

```
/// <summary>
/// Method to invoke when the command is executed.
/// </summary>
private void OnCmdExecute()
{
    var dependencyResolver = this.GetDependencyResolver();
    var mediator = dependencyResolver.Resolve<IMessageMediator>();
    mediator.SendMessage("Test Value", "myTag");
}
```

The message is now sent with the tag. The attribute has to be used as shown below:

```
/// <summary>
/// Shows the message.
/// </summary>
/// <param name="value">The value.</param>
[MessageRecipient(Tag = "myTag")]
private void ShowMessage(string value)
{
    var dependencyResolver = this.GetDependencyResolver();
    var messageService = dependencyResolver.Resolve<IMessageService>();
    messageService.Show(value);
}
```

