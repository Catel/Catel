---
title: "Weak events" 
---
You have probably heard about weak events before. This documentation is not about the issue of the cause of weak events, there are lots of articles about that. This documentation writes about the solution, which is the WeakEventListener. Shortly said, when you do this in every class (just for the sake of explaining the problem, donâ€™t start thinking this code has no business value):

```
var log = Log.Instance;
log.LogReceived += OnLogReceived;
```

As you can see, the log is a singleton, so there is only one living instance of the Log class. It will probably live as long as the app itself. Now you might be thinking: whatâ€™s wrong with this code? Nothing, until the app starts growing and growing and your users start complaining about memory issues.

What happens here is that you subscribe to the LogReceived event of the Log class. This subscription contains 2 things:

1.  What class do I need to call (null for static, otherwise the instance of the class)
2.  What method do I need to call

So, in fact now the Log class knows about the instance of the class that just subscribed to it and holds a reference to it (how else can it deliver the event, if it doesnâ€™t know the address). Thus, the classes that subscribe to the Log and that do no unsubscribe will never be collected by the garbage collection.

## Open instance delegates

The key feature behind this implementation of the weak event pattern is open instance delegates. You are probably wondering: what the hell are open instance delegates? Well, good question, and I will try to explain it. An open instance delegate is just as a regular delegate, it points to the method of a specific class, but the biggest difference is that it does not bind to a specific instance. This means that it can be described as: I know you live on that street (method), but I have not clue in which city (instance) that is. The instance can be specified later. The delegate for a regular event handler looks like this:

```
public delegate void OpenInstanceHandler(TTarget @this, object sender, TEventArgs e);
```

The @this is nothing special, it allows us to use the this keyword so everyone knows that the target should be passed there. As you can see, it contains 3 parameters. The first one is the target (the city), the second and third parameters are the parameters of the regular event handler.

## Weak references

The weak event listener creates an open instance delegate and stores both the source and target in a WeakReference class. As soon as one of these references are no longer valid, the class is unbound. The good side of this approach is that this weak event listener does not leak when the event never fires.

## What does it support

The following use cases are supported:

-   Instance source (event) and instance target (handler)
-   Static source (event) and instance target (handler)
-   Instance source (event) and static target (handler)

So, actually it handles everything that can cause a memory leak via event subscriptions!

## What does it not support and what are the downsides

This weak event listener follows the rules of the .NET framework. So, it cannot subscribe to private events. If you want private events, do your own hacking (the source is available, you only have to change the DefaultEventBindingFlags at the top of the class).

There are a few downsides about using a weak event listeners in general:

-   Itâ€™s notation is ugly, the â€œoriginalâ€ .NET way looks way better
-   You have to name the event by string, that sucks (if you know a better way, contact me!)
-   It can only handle events with a handler of EventHandler\<TEventArgs\>
-   You become a lazy developer not caring about subscriptions

## How to use

There are 4 categories of event subscriptions, all described below.

### Instance to instance

This is the situation where an instance target subscribes to an instance event. The events are unbound as soon as either the target or source are collected.

```
var source = new EventSource();
var listener = new EventListener();
var weakEventListener = WeakEventListener<EventListener, EventSource, EventArgs>.SubscribeToWeakEvent(listener, source, "PublicEvent", listener.OnPublicEvent);
```

### Instance to static

This is the situation where a static target subscribes to an instance event. The events are unbound as soon as the source is collected.

```
var source = new EventSource();

var weakEventListener = WeakEventListener<EventListener, EventSource, EventArgs>.SubscribeToWeakEvent(null, source, "PublicEvent", EventListener.OnEventStaticHandler);
```

### Static to instance

This is the situation where an instance target subscribes to a static event. The events are unbound as soon as the target is collected.

```
var listener = new EventListener();

var weakEventListener = WeakEventListener<EventListener, EventSource, EventArgs>.SubscribeToWeakEvent(listener, null, "StaticEvent", listener.OnPublicEvent);
```

### Static to static

This is not supported because you shouldnâ€™t be using a weak event listener here. Static events with static event handlers simply cannot cause memory leaks because both the source and the target have no instance. However, it might be possible that you subscribe to an event too many times and the event fires too many times. But again, no memory issues here.

