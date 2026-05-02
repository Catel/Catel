---
title: "LocationService" 
description: ""
---
The `ILocationService` allows a developer to use GPS devices inside a view model.

{{% notice warning %}}
It is important that the service must be started and stopped to retrieve values
{{% /notice %}}

## Starting the service

The GPS service needs to be started and stopped. To start the GPS service, use the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var locationService = dependencyResolver.Resolve<ILocationService>();
locationService.LocationChanged += OnCurrentLocationChanged;
locationService.Start();
```

The service will raise the `LocationChanged` event when a new location becomes available.

## Stopping the service

It is required to stop the service when it is no longer needed. The service can be stopped using the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var locationService = dependencyResolver.Resolve<ILocationService>();
locationService.LocationChanged -= OnCurrentLocationChanged;
locationService.Stop();
```

## Emulating GPS without device

It is possible to emulate GPS without actually owning a device or emulate data in the emulator. To accomplish this, it is required to use the `Catel.MVVM.Services.Test.LocationService` class. This class can be used in the following way:

```
var dependencyResolver = this.GetDependencyResolver();
Test.LocationService service = (Test.LocationService)dependencyResolver.Resolve<ILocationService>();

// Queue the next location (and then wait 5 seconds)
var locationTestData = new LocationTestData(new Location(100d, 100d), new TimeSpan(0, 0, 0, 5)));
service.ExpectedLocations.Add(locationTestData);

// Go to the next location manually
service.ProceedToNextLocation();
```

It is also possible to enqueue lots of coordinates with a time span and emulate a path.

