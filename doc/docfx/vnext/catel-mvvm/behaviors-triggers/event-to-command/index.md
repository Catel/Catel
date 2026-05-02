---
title: "EventToCommand" 
---
Almost every respectable MVVM framework supports the `EventToCommand` trigger. It is a trigger that allows a an event to be turned into a command. This way, you never have to manually add event handlers, search for the view model in the code-behind and then call the right command.

The usage is really simple, but requires the `System.Windows.Interactivity.dll` reference (ships with Catel). The example below shows how to add a trigger for the double click of a ListBox.

Â 1) Add the following XML namespaces:

```
xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
xmlns:catel="http://schemas.catelproject.com"
```

2) Use the following definition. This example will invoke the Edit command of the view model):

```
<ListBox ItemsSource="{Binding PersonCollection}" SelectedItem="{Binding SelectedPerson}">
    <i:Interaction.Triggers>
        <i:EventTrigger EventName="MouseDoubleClick">
            <catel:EventToCommand Command="{Binding Edit}" DisableAssociatedObjectOnCannotExecute="False" />
        </i:EventTrigger>
    </i:Interaction.Triggers>

    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <Label Content="{Binding FirstName}" />
                <Label Content="{Binding MiddleName}" />
                <Label Content="{Binding LastName}" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

If you want to use parameters (in the case of this example, get the MouseDoubleClick event args, set PassEventArgsToCommand to true:

```
<Commands:EventToCommand Command="{Binding Edit}" DisableAssociatedObjectOnCannotExecute="False" PassEventArgsToCommand="True" />
```

Then, in the view model, you can even make the command "type-safe":

```
/// <summary>
/// Gets the Edit command.
/// </summary>
public Command<MouseEventArgs> Edit { get; private set; }

// TODO: Move code below to constructor
Edit = new Command<MouseEventArgs>(OnEditExecute, OnEditCanExecute);
// TODO: Move code above to constructor

/// <summary>
/// Method to check whether the Edit command can be executed.
/// </summary>
private bool OnEditCanExecute(MouseEventArgs e)
{
    return true;
}

/// <summary>
/// Method to invoke when the Edit command is executed.
/// </summary>
private void OnEditExecute(MouseEventArgs e)
{
    // TODO: Handle command logic here
}
```

