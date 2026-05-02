---
title: "SelectDirectoryService" 
---
The `ISelectDirectoryService`Â allows a developer to let the user choose a directory from inside a view model.

## Selecting a directory

To select a directory, it is required to set the right properties of the service and then make a call to theÂ `DetermineDirectory`Â method:

```
var dependencyResolver = this.GetDependencyResolver();
var selectDirectoryService = dependencyResolver.Resolve<ISelectDirectoryService>();
if (selectDirectoryService.DetermineDirectory())
{
    // User selected a directory
}
```

