---
title: "SchedulerService" 
---
The `ISchedulerService` allows a developer to schedule an action in the relative or absolute future. The `SchedulerService` will use the `DispatcherTimer` to invoke the action.

Note that the `SchedulerService` does not provide any persistence of actions and schedules. When the application is closed, all schedules are lost because they are kept in memory.

## Scheduling an action in the relative future

To schedule an action in the relative future, use the `Schedule` method with the `TimeSpan` overload. The code below starts the action with a delay of 50 milliseconds.

```
var dependencyResolver = this.GetDependencyResolver();
var schedulerService = dependencyResolver.Resolve<ISchedulerService>();
schedulerService.Schedule(() => DoSomething(), new TimeSpan(0, 0, 0, 0, 50));
```

## Scheduling an action in the absolute future

To schedule an action in the absolute future, use the `Schedule` method with the `DateTime` overload. The code below starts the action in 5 minutes.

```
var dependencyResolver = this.GetDependencyResolver();
var schedulerService = dependencyResolver.Resolve<ISchedulerService>();
schedulerService.Schedule(() => DoSomething(), DateTime.Now.AddMinutes(5));
```

