---
title: "OpenFileService" 
description: ""
---
The `IOpenFileService` allows a developer to let the user choose a file from inside a view model.

## Opening a file

To open a file, it is required to set the right properties of the service and then make a call to the `DetermineFile` method:

```
var dependencyResolver = this.GetDependencyResolver();
var openFileService = dependencyResolver.Resolve<IOpenFileService>();
openFileService.Filter = "All files|*.*";
if (openFileService.DetermineFile())
{
    // User selected a file
}
```

