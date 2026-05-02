---
title: "Using compiled bindings" 
---
A common scenario in UWP is to use `x:Bind` instead of `Binding`. This requires knowledge about the actual type of the view model.

There are 2 options to support this.

## Use the `VM` property

Catel by default exposes the `ViewModel` property as `IViewModel`. However, there is a reserved `VM` property you can use that is typed:

```
public sealed partial class BikeSummaryView
{
    public BikeSummaryView()
    {
        InitializeComponent();
    }

    internal BikeSummaryViewModel VM
    {
        get { return ViewModel as BikeSummaryViewModel; }
    }
}
```

Since this is a reserved keyword, Catel will automatically take care of the change notifications and `x:Bind` can be used directly against `VM`:

```
<TextBlock Text="{x:Bind VM.Title}" />
```

Note that the binding should either be set to `Mode=OneWay` per binding or for all bindings in a view by specifying `x:DefaultBindMode="OneWay"`.

## Cast every compiled bindings

Another option is to use casts everywhere in the view:

```
<TextBlock Text="{x:Bind ((vm:BikeSummaryView)VM).Title}" />
```

