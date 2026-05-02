---
title: "Replacing the default components" 
description: ""
weight: 50
---
By default, Catel provides very fast and functional implementations of the component interfaces. It is possible though that one needs to use a different container than the specified ones.

{{% notice warning %}}
Note that when any component is replaced, it must be registered with the other instances that are already running. Catel cannot do this automatically because it is not aware how other (customized) components interact or require registration.
{{% /notice %}}

## Replacing default components

Starting with Catel 3.9, it is very easy to customize the components. This can be achieved by customizing the factory methods that are available on the *IoCConfiguration* class.

{{% notice warning %}}
Note that the customization of the IoCConfiguration is the **first **** thing that must be done at application start up
{{% /notice %}}

To replace any component, first create a custom implementation of the specific component, for example the *IServiceLocator*. Then update the factory and callÂ *UpdateDefaultComponents*:

```
Catel.IoC.IoCFactory.CreateServiceLocatorFunc = () => new MyCustomServiceLocator();
Catel.IoC.IoCFactory.CreateTypeFactoryFunc = () => new MyCustomTypeFactory();
Â 
Catel.IoC.IoCConfiguration.UpdateDefaultComponents();
```

At this moment, Catel will fully replace the components (in this case the *IServiceLocator* and *ITypeFactory*), but will keep using the default implementation of theÂ *IDependencyResolver*.

## Creating IoC components in code

It is best to respect the customization of the IoC components in the code. Therefore it is wise to always use theÂ *IoCFactory* to create a *ServiceLocator* when aÂ **new instance** is needed:

```
var serviceLocator = IoCFactory.CreateServiceLocator();
```

Catel will automatically create the rightÂ *IDependencyResolver* and *ITypeFactory* and register them in the newly createdÂ *IServiceLocator*.

