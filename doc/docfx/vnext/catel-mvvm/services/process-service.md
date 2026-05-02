---
title: "ProcessService" 
---
The `IProcessService` allows a developer to run processes from inside a view model.

## Starting a process with arguments

To start a process with arguments, use the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var processService = dependencyResolver.Resolve<IProcessService>();
processService.StartProcess("notepad.exe", @"C:\mytextfile.txt");
```

## Starting a process with arguments and completed callback

To start a process with arguments and receive a callback on completion, use the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var processService = dependencyResolver.Resolve<IProcessService>();
processService.StartProcess("notepad.exe", @"C:\mytextfile.txt", OnProcessCompleted);
```

