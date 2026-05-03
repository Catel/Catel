---
title: "SaveFileService" 
---
The `ISaveFileService` allows a developer to let the user choose a file from inside a view model.

## Choosing a file

To select a file to save, inject the service via the constructor and use:

```csharp
private readonly ISaveFileService _saveFileService;

public MyViewModel(IServiceProvider serviceProvider, ISaveFileService saveFileService)
    : base(serviceProvider)
{
    _saveFileService = saveFileService;
}
```

```csharp
_saveFileService.Filter = "C# File|*.cs";
if (_saveFileService.DetermineFile())
{
    // User selected a file
}
```


