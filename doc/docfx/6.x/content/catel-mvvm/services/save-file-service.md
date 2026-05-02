---
title: "SaveFileService" 
description: ""
---
The `ISaveFileService` allows a developer to let the user choose a file from inside a view model.

## Choosing a file

To select a file to save, it is required to set the right properties of the service and then make a call to the `DetermineFile` method:

```
var dependencyResolver = this.GetDependencyResolver();
var saveFileService = dependencyResolver.Resolve<ISaveFileService>();
saveFileService.Filter = "C# File|*.cs";
if (saveFileService.DetermineFile())
{
    // User selected a file
}
```

