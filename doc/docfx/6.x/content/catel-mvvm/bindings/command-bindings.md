---
title: "Command bindings" 
description: ""
---
Catel will automatically hook up theÂ `CanExecute`Â of the command to theÂ `Enabled`Â property of the element it is bound to. A command binding is always bound to an event of a specific element as shown below.

## Android

```
protected override void AddBindings(BindingContext bindingContext, IViewModel viewModel)
{
    var vm = (MainViewModel) viewModel;

    bindingContext.AddCommandBinding(_testButton, "Click", vm.RunCommand);
}
```

## iOS

{{% notice warning %}}
iOS not yet documented
{{% /notice %}}

