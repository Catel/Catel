---
title: "Keeping view models alive" 
---
In Catel, view models are automatically closed when the `UserControl` is unloaded from the visual tree. This is because there is noÂ guaranteeÂ that the control will be loaded again. This works great in most of the cases, but sometimes you need more control about the lifetime of the view model. One good example is the use of the `TabControl`. When a tab control contains a user control with a view model, every time a new tab is selected, the controls on the previously selected tab are unloaded (and thus the view models are closed).

It is possible to have more control about the lifetime of view models. To keep a view model alive, even when the view is unloaded, set the `CloseViewModelOnUnloaded` property of the `UserControl` to false in the constructor of the view:

```
CloseViewModelOnUnloaded = false;
```

The view model will now be re-used when the view is loaded into the visual tree again.

Keep in mind that the developer is responsible for actually closing the view model

[Orc.Controls](https://github.com/wildgums/orc.controls) contains an implementation of a `TabControl` that gives more control over the lifetime of the tabs

The Catel Examples repository contains an example demonstrating controlling the lifetime of view models


