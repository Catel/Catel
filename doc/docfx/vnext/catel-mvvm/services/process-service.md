---
title: "ProcessService" 
---
The `IProcessService` allows a developer to run processes from inside a view model.

## Starting a process with arguments

To start a process with arguments, inject the service via the constructor and use:

```csharp
private readonly IProcessService _processService;

public MyViewModel(IServiceProvider serviceProvider, IProcessService processService)
    : base(serviceProvider)
{
    _processService = processService;
}
```

```csharp
_processService.StartProcess("notepad.exe", @"C:\mytextfile.txt");
```

## Starting a process with arguments and completed callback

To start a process with arguments and receive a callback on completion, use the following code:

```csharp
_processService.StartProcess("notepad.exe", @"C:\mytextfile.txt", OnProcessCompleted);
```


