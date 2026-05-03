---
title: "BusyIndicatorService" 
---
The `IBusyIndicatorService` (formerly `IPleaseWaitService`) allows a developer to show a busy indicator from a view model. 

## Showing

```csharp
private readonly IBusyIndicatorService _busyIndicatorService;

public MyViewModel(IServiceProvider serviceProvider, IBusyIndicatorService busyIndicatorService)
    : base(serviceProvider)
{
    _busyIndicatorService = busyIndicatorService;
}
```

```csharp
_busyIndicatorService.Show();
```

## Hiding

```csharp
_busyIndicatorService.Hide();
```

## Showing and automatically hide

The `IBusyIndicatorService` can automatically hide itself when an action is completed. To use this feature, pass a delegate to the `Show` method and the service will hide the window as soon as the delegate has completed.

```csharp
_busyIndicatorService.Show(() => Thread.Sleep(1500));
```

## Changing the status

```csharp
_busyIndicatorService.UpdateStatus("new status");
```

## Showing a determinate busy indicator

By default, the `IBusyIndicatorService` shows an indeterminate state (no actual progress is visible).

The `UpdateStatus` method can be used to show progress. The `statusFormat` argument can contain `{0}` (represents the current item) and `{1}` (represents the total items). However, they can also be left out.

```csharp
_busyIndicatorService.UpdateStatus(1, 5, "Updating item {0} of {1}");
```

The determinate version can be hidden via a call to `Hide` or when the `currentItem` argument is larger than the number of `totalItems`.

## Push/Pop

Sometimes, multiple view models or multiple actions use the service. It is not possible to hide the window when the first action is completed, because the user will still have to wait for the other actions to complete (without a busy indicator window). To implement this correctly, it is possible to use the `Push` and `Pop` methods.

The `Push` method shows the window if it is not already visible and then increases an internal counter. At the start of each (asynchronous) action, the developer can call the `Push` method. When the action is completed, the developer calls `Pop` which will internally decrease the counter. If the counter hits zero (0), the window is automatically hidden.

It is possible to hide the window, even when the internal counter is not yet zero. A call to `Hide` will reset the counter to zero and thus hide the window.


