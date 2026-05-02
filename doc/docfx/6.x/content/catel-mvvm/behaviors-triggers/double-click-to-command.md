---
title: "DoubleClickToCommand" 
description: ""
---
Lots of times, a developer needs to handle a double click event. This trigger allows a developer to track a double click on any `FrameworkElement` and respond to that using a command.

1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition. This example will invoke the Edit command of the view model when the item is double clicked):

```
<ListBox x:Name="listBox" ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
  <ListBox.ItemTemplate>
    <DataTemplate>
      <Grid>
        <i:Interaction.Behaviors>
          <catel:DoubleClickToCommand Command="{Binding ElementName=listBox, Path=DataContext.Edit}" />
        </i:Interaction.Behaviors>

        <StackPanel Orientation="Horizontal">
          <Label Content="{Binding FirstName}" />
          <Label Content="{Binding MiddleName}" />
          <Label Content="{Binding LastName}" />
        </StackPanel>
      </Grid>
    </DataTemplate>
  </ListBox.ItemTemplate>
</ListBox>
```

The trigger contains an additional property `AutoFixListBoxItemTemplate` which is set to true by default to easily allow the addition of a double click event to a ListBox. The item template must contain a grid as a base like shown above.

{{% notice info %}}
This behavior also supports a constructor that accepts an Action. This way, an anonymous delegate can be executed when the behavior is created in code
{{% /notice %}}

