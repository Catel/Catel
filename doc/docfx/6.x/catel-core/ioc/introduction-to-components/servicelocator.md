---
title: "ServiceLocator" 
---
The *ServiceLocator* services as the container inside Catel.

Internally it uses the `TypeFactory` as instantiator for the services.

Catel uses it's own `ServiceLocator` implementing the `IServiceLocator` to gather all services required by Catel. For example, default services are the `IPleaseWaitService` and the `IUIVisualizerService`. By default, when the first view model is instantiated, Catel registers all default out of the box services to the `ServiceLocator`. However, it only does this when the specific services are not already registered. This allows an end-developer to register his/her own implementations of the services before any view model is instantiated (for example, at application startup).

The `ServiceLocator` can be instantiated, but Catel instantiates one instance that can be used and shared amongst all objects inside the same `AppDomain`. The `ServiceLocator` can be retrieved by using `ServiceLocator`.`Default`.

For more information how types are instantiated and dependency injection, take a look at the [TypeFactory documentation]({{< relref "catel-core/ioc/introduction-to-components/typefactory.md" >}})

## Registering a type

Use the following code to register a specific type in the `ServiceLocator`:

```
ServiceLocator.Default.RegisterType<IPleaseWaitService, PleaseWaitService>();
```

## Registering a late-bound type

Use the following code to register a late-bound type in the `ServiceLocator`:

```
ServiceLocator.Default.RegisterType<IPleaseWaitService>((typeFactory, serviceLocatorRegistration) => new PleaseWaitService());
```

## Registering an instance of a type

Catel uses the `TypeFactory` or `Activator.CreateInstance` to create the interface implementations when the object is first resolved. However, sometimes a service constructor requires parameters or takes a long time to construct. In such cases, it is recommended to create the type manually and register the instance of the type:

```
var pleaseWaitService = new PleaseWaitService();
ServiceLocator.Default.RegisterInstance<IPleaseWaitService>(pleaseWaitService);
```

## Registering a type via MissingType event

The ServiceLocator gives the end-developer a last-resort chance to register a type when it is not registered in the ServiceLocator or any of the external containers. This event is very useful for logging (then the developer in the log knows exactly what type is missing from the IoC container) or it can be used to determine at runtime in a very late stage what implementation of the service must be used. To register a type via the event, subscribe to the event and then use the following code:

```
private void OnMissingType(object sender, MissingTypeEventArgs e)
{
    if (e.InterfaceType == typeof(IPleaseWaitService))
    {
        // Register an instance
        e.ImplementingInstance = new PleaseWaitService();

        // Or a type
        e.ImplementingType = typeof(PleaseWaitService);
    }
}
```

If both the ImplementingInstance and ImplementingType are filled, the ImplementingIntance will be used.

## Resolving a type

To retrieve the implementation of a service, use the following code:

```
var pleaseWaitService = ServiceLocator.Default.ResolveType<IPleaseWaitService>();
```

