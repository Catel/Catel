---
title: "Introduction to services" 
---
Services are very important in MVVM. They define a way to interact with the user without using fixed controls such as *MessageBox* or *SaveFileDialog*. The interfaces defined in Catel only define generic functionality of what to expect from a specific service. Using services is a great way to abstract away all specific functionality from a view model into a service that can be mocked during unit testing and can be used by other view models as well.

## ServiceLocator

The key behind services is the *ServiceLocator*. The *ServiceLocator* is the IoC (Inversion of Control) container that Catel provides. This is a container that contains all registrations and service instances available throughout the application. Retrieving services from the default *ServiceLocator* in Catel is very simple:

```
var dependencyResolver = this.GetDependencyResolver();
var messageService = dependencyResolver.Resolve<IMessageService>();
```

It is also possible to get services injected into the constructor, which is the recommended approach

## Dependency injection

A slightly better way to manage dependencies is to use dependency injection. The reason is that to instantiate a class, you always have to provide all the dependencies. This way, all dependencies are always known to the caller making it a bit complicated and encouraging high coupling. Using dependency injection however makes it a bit easier to control than having to *know* what services are being used by a component (such as a view model).

Catel fully supports dependency on view models. This means that a view model can have a constructor with several services. Catel will automatically inject the services via the constructor. An example is below:

```
public class PersonViewModel : ViewModelBase
{ 
    private readonly IMessageService _messageService;
    private readonly IPleaseWaitService _pleaseWaitService;

    public PersonViewModel(IMessageService messageService, IPleaseWaitService pleaseWaitService)
    {
        _messageService = messageService;
        _pleaseWaitService = pleaseWaitService;
    }
}
```

## Overview of services

The services below are available in Catel:

Name

Description

ILocationService

Allows a developer to use GPS devices inside a view model.

IMessageService

Allows a developer to show message boxes from a view model.

INavigationService

Allows a developer to navigate to other pages inside an application using view models only.

IOpenFileService

Allows a developer to let the user choose a file from inside a view model.

IPleaseWaitService

Allows a developer to show a please wait message (a.k.a. busy indicator) from a view model.

IProcessService

Allows a developer to run processes from inside a view model.

ISaveFileService

Allows a developer to let the user choose a file from inside a view model.

IUIVisualizerService

Allows a developer to show (modal) windows or dialogs without actually referencing a specific view.

Note that this section is not always fully up-to-date, Catel might provide more services than listed here

