---
title: "SelectDirectoryService" 
---
The `ISelectDirectoryService` allows a developer to let the user choose a directory from inside a view model.

## Selecting a directory

To select a directory, inject the service via the constructor and use:

```csharp
private readonly ISelectDirectoryService _selectDirectoryService;

public MyViewModel(IServiceProvider serviceProvider, ISelectDirectoryService selectDirectoryService)
    : base(serviceProvider)
{
    _selectDirectoryService = selectDirectoryService;
}
```

```csharp
var context = new DetermineDirectoryContext();

var result = await _selectDirectoryService.DetermineDirectoryAsync(context);
if (result.Result)
{
    // User selected a directory, available via result.DirectoryName
}
```


