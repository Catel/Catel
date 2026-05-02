---
title: "PleaseWaitService" 
description: ""
---
The `IPleaseWaitService` allows a developer to show a please wait message (a.k.a. busy indicator) from a view model.Â 

## Showing

```
using Catel.IoC;
```

```
var dependencyResolver = this.GetDependencyResolver();
var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
pleaseWaitService.Show();
```

## Hiding

```
var dependencyResolver = this.GetDependencyResolver();
var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
pleaseWaitService.Hide();
```

## Showing and automatically hide

The `IPleaseWaitService` can automatically hide itself when an action is completed. To use this feature, simply pass a delegate to the `Show` method and the service will hide the window as soon as the delegate has completed.

```
using Catel.IoC;
```

```
var dependencyResolver = this.GetDependencyResolver();
var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
pleaseWaitService.Show(() => Thread.Sleep(1500));
```

## Changing the status

```
var dependencyResolver = this.GetDependencyResolver();
var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
pleaseWaitService.UpdateStatus("new status");
```

## Showing a determinate please wait window

By default, the `IPleaseWaitService` shows an indeterminate state (no actual progress is visible).

The `UpdateStatus` method can be used to show the window. The `statusFormat` argument can contain '{0}' (represents the current item) and '{1}' (represents the total items). However, they can also be left out.

```
var dependencyResolver = this.GetDependencyResolver();
var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
pleaseWaitService.UpdateStatus(1, 5, "Updating item {0} of {1}");
```

The determinate version can be hidden via a call to *Hide* or when the currentItem argument is larger than the number of totalItems.

## Push/Pop

Sometimes, multiple view models or multiple actions use the service. It's not possible to hide the window when the first action is completed, because the user will still have to wait for the other actions to complete (without a please wait window). To implement this correctly, it is possible to use the `Push` and `Pop` methods.

The `Push` method shows the window if it is not already visible and then increases an internal counter. At the start of each (asynchronous) action, the developer can call the `Push` method. When the action is completed, the developer calls `Pop` which will internally decrease the counter. If the counter hits zero (0), the window is automatically hidden.

It is possible to hide the window, even when the internal counter is not yet zero. A call to `Hide` will reset the counter to zero and thus hide the window.

