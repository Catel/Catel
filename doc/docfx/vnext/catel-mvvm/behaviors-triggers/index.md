---
title: "Behaviors & triggers" 
---
Behaviors and triggers are very important to correctly separate the view from the view model. For example, to respond to an event in a view model, you cannot simply subscribe to the events in the view. The EventToCommand behavior is a great example to solve this problem.

Catel offers lots of behaviors out of the box, so it is definitely worth taking a look at the behaviors.

## Managing interactivity classes

Starting with Catel 4.0, it is possible to manage interactivity classes such as behaviors from theÂ InteractivityManager class. This allows a developer to get notified when a behavior or trigger is loaded or unloaded.

Note that the *InteractivityManager* is only compatible with behaviors and triggers using one of the Catel base classes

The manager contains both events and methods to retrieve information about triggers. For example, if one is interested in all the *Focus* triggers, one could do the following:

```
public class FocusWatcher
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();
Â 
    private readonly IInteractivityManager _interactivityManager;

    public FocusWatcher(IInteractivityManager interactivityManager)
    {
        Argument.IsNotNull(() => interactivityManager);

        _interactivityManager = interactivityManager;
        _interactivityManager.BehaviorLoaded += OnBehaviorLoaded;
        _interactivityManager.BehaviorUnloaded += OnBehaviorUnloaded;
    }

    private void OnBehaviorLoaded(object sender, BehaviorEventArgs e)
    {
        var focus = e.Behavior as Focus;
        if (focus != null)
        {
            Log.Info("Focus behavior loaded");
        }
    }

    private void OnBehaviorUnloaded(object sender, BehaviorEventArgs e)
    {
        var focus = e.Behavior as Focus;
        if (focus != null)
        {
            Log.Info("Focus behavior unloaded");
        }
    }
}
```

