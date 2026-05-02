---
title: "VibrateService" 
description: ""
---
The `IVibrateService` allows a developer to start and stop vibration of the device via a service.

## Starting vibration

To start the vibration, use the following code (will vibrate for 250 ms). Note that the time span must be between 0 and 5 seconds.

```
var dependencyResolver = this.GetDependencyResolver();
var vibrateService = dependencyResolver.Resolve<IVibrateService>();
vibrateService.Start(new TimeSpan(0, 0, 0, 0, 250);
```

## Stopping the vibration earlier than initially planned

By default, the vibration stops automatically after the specified time span has passed. However, it is possible to stop the vibration manually.

```
var dependencyResolver = this.GetDependencyResolver();
var vibrateService = dependencyResolver.Resolve<IVibrateService>();
vibrateService.Stop();
```

