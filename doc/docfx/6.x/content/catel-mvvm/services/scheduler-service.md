---
title: "SchedulerService" 
description: ""
---
TheÂ `ISchedulerService` allows a developer to schedule an action in the relative or absolute future. The `SchedulerService` will use theÂ `DispatcherTimer` to invoke the action.

{{% notice warning %}}
Note that the `SchedulerService` does not provide any persistence of actions and schedules. When the application is closed, all schedules are lost because they are kept in memory.
{{% /notice %}}

## Scheduling an action in the relative future

To schedule an action in the relative future, use theÂ `Schedule` method with theÂ `TimeSpan` overload. The code below starts the action with a delay of 50 milliseconds.

```
var dependencyResolver = this.GetDependencyResolver();
var schedulerService = dependencyResolver.Resolve<ISchedulerService>();
schedulerService.Schedule(() => DoSomething(), new TimeSpan(0, 0, 0, 0, 50));
```

## Scheduling an action in the absolute future

To schedule an action in the absolute future, use theÂ `Schedule`Â method with the `DateTime`Â overload. The code below starts the action in 5 minutes.

```
var dependencyResolver = this.GetDependencyResolver();
var schedulerService = dependencyResolver.Resolve<ISchedulerService>();
schedulerService.Schedule(() => DoSomething(), DateTime.Now.AddMinutes(5));
```

