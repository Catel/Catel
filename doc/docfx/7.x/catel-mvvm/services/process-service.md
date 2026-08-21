---
title: "ProcessService" 
---
The `IProcessService` allows a developer to run processes from inside a view model.

## Running a process asynchronously

To run a process and await its completion, inject the service via the constructor and use:

```csharp
private readonly IProcessService _processService;

public MyViewModel(IServiceProvider serviceProvider, IProcessService processService)
    : base(serviceProvider)
{
    _processService = processService;
}
```

```csharp
var context = new ProcessContext
{
    FileName = "notepad.exe",
    Arguments = @"C:\mytextfile.txt"
};

var result = await _processService.RunAsync(context);
```

## Starting a process with arguments

To start a process without waiting for it to complete, use the following code:

```csharp
_processService.StartProcess("notepad.exe", @"C:\mytextfile.txt");
```

## Starting a process with arguments and completed callback

To start a process with arguments and receive a callback on completion, use the following code:

```csharp
_processService.StartProcess("notepad.exe", @"C:\mytextfile.txt", OnProcessCompleted);
```


