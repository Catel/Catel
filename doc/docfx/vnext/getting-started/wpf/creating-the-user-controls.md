---
title: "Creating the views (user controls)" 
---
The PersonApplication example uses windows (`catel:Window` and `catel:DataWindow`) to display and edit persons. User controls (`catel:UserControl`) follow the same pattern but are embedded inside other views rather than being shown as top-level windows.

A Catel user control:

- Derives from `catel:UserControl` in XAML.
- Follows the same naming convention for automatic view model resolution (`PersonView` → `PersonViewModel`).
- Accepts a model injected through its data context.

## Example: read-only PersonView

If you wanted a reusable, read-only view to embed persons inside a list, you could create a `PersonView` user control:

```xml
<catel:UserControl x:Class="Catel.Examples.PersonApplication.Views.PersonView"
                   xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                   xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                   xmlns:catel="http://schemas.catelproject.com">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>

        <Label Grid.Column="0" Content="{Binding FirstName}" />
        <Label Grid.Column="1" Content="{Binding MiddleName}" />
        <Label Grid.Column="2" Content="{Binding LastName}" />
    </Grid>
</catel:UserControl>
```

The code-behind can be kept empty or minimal because Catel wires up the view model automatically.

## Up next

[Creating the views (windows)]({{< relref "getting-started/wpf/creating-the-windows.md" >}})


