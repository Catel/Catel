---
title: "IoC (ServiceLocator and TypeFactory)" 
description: ""
---
Before Catel 2.0, the IoC container used internally was Unity. However, this forced all users to use and configure Unity as the IoC container in their apps and required the shipping of the libraries as well. Since Catel 2.0, a different technique is used which allows the end-developer to use the IoC container technique of their choice.

## Different components in IoC

There are several different components that are very important for the IoC in Catel:

-   **ServiceLocator**
    Component that is responsible for the registrations of all types. This is the actual IoC container.
-   **TypeFactory**
    Component that is responsible to create types. This uses theÂ *IServiceLocatorÂ *to retrieve the types which are required to instantiate a type.
-   **DependencyResolver**
    Light-weight implementation of theÂ *IServiceLocator* which does not expose any register methods, but only allows to resolve types.**
    **

## Getting components for any object

In every object, it is possible to use the *Default* properties to retrieve the instances of each component. This will cause problems when different scoping is used. To always be sure to get the right component for the object you are working with, it is recommended to use the following extension methods:

```
using Catel.IoC; // Contains ObjectExtensions which allow use of below extension methods

public class MyService
{
    public void SomeMethod()
    {
        // If you need to create a type with the current scope type factory
        var typeFactory = this.GetTypeFactory();
Â 
        // If you need to register a type with the current scope service locator
        var serviceLocator = this.GetServiceLocator();
Â 
        // If you need to resolve a type with the current scope and the type is not injected via dependency injection
        var dependencyResolver = this.GetDependencyResolver();
    }
}
```

