---
title: "ViewExportService" 
description: ""
---
The `IViewExportService` allows a developer to export a specific view that belongs to a view model to the clipboard, a file or a printer.

## Exporting a view

To export a view, use the following code:

```
var dependencyResolver = this.GetDependencyResolver();
var viewExportService = dependencyResolver.Resolve<IViewExportService>();
viewExportService.Export(myViewModel, ExportMode.File);
```

