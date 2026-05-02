---
title: "MessageBase" 
---
The MessageMediator is a very powerful class to send messages to other objects inside an application. However, it can sometimes by cumbersome to register and create messages. Therefore the MessageBase class is a very nice convenience class to create messages and allow easier registration.

The MessageBase provides the following additional functionality out of the box:

-   Send messages with data without instantiating a message
-   Register message handlers
-   Unregister message handlers

## Creating messages based on the MessageBase

Â It is very easy to create a new message. The message below is a message that contains a string and this little class provides lots of capabilities.

```
public class DemoMessage : MessageBase<DemoMessage, string>
{
    public DemoMessage() { }

    public DemoMessage(string content)
        : base(content) { }
}
```

Note that the message needs an empty constructor

##Â Sending messages

A user can send a message by using the following code:

```
DemoMessage.SendWith("hello world");
```

Registering to messages

Â A class that is interested in message can register to a message using the Register method:

```
DemoMessage.Register(this, OnDemoMessage);
```

## Unregistering from messages

```
DemoMessage.Unregister(this, OnDemoMessage);
```

## Instantiating a message with data

The MessageBase class can also instantiate messages by using the With method:

```
var message = DemoMessage.With("hello world");
```

