---
title: "Using attributes" 
---
The [ServiceLocator](../introduction-to-components/servicelocator.md) in Catel can be set up to discover attribute based registration. 

## Declaring a registration since the type definition

There is a way to automatically register types into a service locator. Using *ServiceLocatorRegistrationAttribute* it is possible to register types into the service locator in a declarative way. The following code shows how use this attribute:

```
[ServiceLocatorRegistration(typeof(IMyClass))]
public class MyClass : IMyClass 
{
}
```

All registration options are available in attribute based registration, such as registration type and tag, as *ServiceLocatorRegistrationAttribute* constructor arguments. The following code shows how use such options (it registers the *MyClass* using the *IMyClass* interface in a transient way (new type every time it is resolved) using the tag *MyTag*:

```
[ServiceLocatorRegistration(typeof(IMyClass), RegistrationType.Transient, "MyTag")]
public class MyClass : IMyClass 
{
}
```

## Activating service locator to scan for automatically registration

By default the service locator doesn't scan for automatic registration. In order to activate this you should set *AutoRegisterTypesViaAttributes* to *true*.

```
var serviceLocator = ServiceLocator.Default;
serviceLocator.AutoRegisterTypesViaAttributes = true;
```

