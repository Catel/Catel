---
title: "Creating the views (windows)" 
---
In this step we will create the windows for the application. In the previous step we already created the user controls.Â Windows are a great way to show in an edit-context. Catel provides great edit-windows in the form of theÂ `DataWindow`. This is a window that automatically addsÂ *OK*Â andÂ *Cancel*Â buttons (but of course allows customization of the buttons and behavior).

## Person window

It is very important that the window derives from one of the Catel windows. This is required to make the binding system work (same as `UserControl`). Make sure that the window definition in the xaml is either `catel:Window` or `catel:DataWindow`

To add a newÂ `DataWindow`,Â right-click theÂ *Views*Â folder in the solution =\>Â *Add*Â =\>Â *New item...*Â =\>Â *On-line*Â =\> and search for Catel as you can see in the screen below:

![](../../../images/getting-started/wpf/creating-the-windows/itemtemplate.png)

Give the new view the nameÂ `PersonWindow`. The view will be added to theÂ *Views*Â folder.

Note that we can use the `PersonViewModel` for both the `PersonView` (user control) and `PersonWindow`. Both views represent the same models and view models, just a different context. To make sure that the `IUIVisualizerService` knows what view to pick first, register the `PersonWindow` in the `IUIVisualizerService` at application startup:

```
var uiVisualizerService = serviceLocator.ResolveType<IUIVisualizerService>();
uiVisualizerService.Register(typeof(PersonViewModel), typeof(PersonWindow));
```

The template will also create a constructor to inject a view model into the window. Please make sure that the constructor takes a view model of the typeÂ `PersonViewModel` instead of the generatedÂ `PersonWindowModel`. Then replace the content of the view with the xaml below:

```
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>

    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <Label Grid.Row="0" Grid.Column="0" Content="First name" />
    <TextBox Grid.Row="0" Grid.Column="1" Text="{Binding FirstName, ValidatesOnDataErrors=True, NotifyOnValidationError=True}" />

    <Label Grid.Row="1" Grid.Column="0" Content="Last name" />
    <TextBox Grid.Row="1" Grid.Column="1" Text="{Binding LastName, ValidatesOnDataErrors=True, NotifyOnValidationError=True}" />
</Grid>
```

## Family window

TheÂ `FamilyWindow`Â is a bit different because we want additional logic in this window. We want to create add / edit / remove buttons for the family members. Therefore we need to create a separate view model which contains this logic.Â 

### Creating theÂ FamilyWindowViewModel

Since theÂ `FamilyWindowViewModel` will look a lot like theÂ `FamilyViewModel`,  just copy/paste theÂ `FamilyViewModel` and rename the copy toÂ `FamilyWindowViewModel`.

Note that the `FamilyWindowViewModel` needs additional logic, but that will be handled in the next part of this getting started guide

### Creating the FamilyWindow

Once the `FamilyWindowViewModel` is created, the `FamilyWindow` must be created exactly the same way as theÂ `PersonWindow`. Again make sure to use the right view model (`FamilyWindowViewModel`) in the constructor of the window in the code-behind. Then use the following xaml:

```
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>

    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>

    <Label Grid.Row="0" Grid.Column="1" Content="Family name" />
    <TextBox Grid.Row="0" Grid.Column="1" Text="{Binding FamilyName, NotifyOnValidationError=True, ValidatesOnDataErrors=True}" />

    <Label Grid.Row="1" Grid.ColumnSpan="2" Content="Persons" />

    <Grid Grid.Row="2" Grid.ColumnSpan="2">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>

        <ListBox Grid.Column="0" ItemsSource="{Binding Persons}" SelectedItem="{Binding SelectedPerson}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <views:PersonView DataContext="{Binding}" />
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>

        <StackPanel Grid.Column="1">
            <Button Command="{Binding AddPerson}" Content="Add..." />
            <Button Command="{Binding EditPerson}" Content="Edit..." />
            <Button Command="{Binding RemovePerson}" Content="Remove" />
        </StackPanel>
    </Grid>
</Grid>
```

## Up next

[Hooking up everything together]({{< relref "getting-started/wpf/hooking-up-everything-together.md" >}})

