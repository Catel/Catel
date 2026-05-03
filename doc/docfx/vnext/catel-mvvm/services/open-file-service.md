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
var context = new DetermineOpenFileContext
{
    Filter = "All files|*.*"
};

var result = await _openFileService.DetermineFileAsync(context);
if (result.Result)
{
    // User selected a file, available via result.FileName
}
```


