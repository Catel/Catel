---
title: "Introduction to services" 
---
Services are very important in MVVM. They define a way to interact with the user without using fixed controls such as *MessageBox* or *SaveFileDialog*. The interfaces defined in Catel only define generic functionality of what to expect from a specific service. Using services is a great way to abstract away all specific functionality from a view model into a service that can be mocked during unit testing and can be used by other view models as well.

## Dependency injection

Services are injected into view models via the constructor. Catel fully supports dependency injection on view models. An example is below:

```csharp
public class PersonViewModel : ViewModelBase
{ 
    private readonly IMessageService _messageService;
    private readonly IBusyIndicatorService _busyIndicatorService;

    public PersonViewModel(IServiceProvider serviceProvider, IMessageService messageService, IBusyIndicatorService busyIndicatorService)
        : base(serviceProvider)
    {
        _messageService = messageService;
        _busyIndicatorService = busyIndicatorService;
    }
}
```

## Overview of services

The services below are available in Catel:

Name | Description
--- | ---
IBusyIndicatorService | Allows a developer to show a busy indicator from a view model.
IMessageService | Allows a developer to show message boxes from a view model.
INavigationService | Allows a developer to navigate to other pages inside an application using view models only.
IOpenFileService | Allows a developer to let the user choose a file from inside a view model.
IProcessService | Allows a developer to run processes from inside a view model.
ISaveFileService | Allows a developer to let the user choose a file from inside a view model.
IUIVisualizerService | Allows a developer to show (modal) windows or dialogs without actually referencing a specific view.

Note that this section is not always fully up-to-date, Catel might provide more services than listed here


