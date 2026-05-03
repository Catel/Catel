---
title: "OpenFileService" 
---
The `IOpenFileService` allows a developer to let the user choose a file from inside a view model.

## Opening a file

To open a file, inject the service via the constructor and use:

```csharp
private readonly IOpenFileService _openFileService;

public MyViewModel(IServiceProvider serviceProvider, IOpenFileService openFileService)
    : base(serviceProvider)
{
    _openFileService = openFileService;
}
```

```csharp
_openFileService.Filter = "All files|*.*";
if (_openFileService.DetermineFile())
{
    // User selected a file
}
```


