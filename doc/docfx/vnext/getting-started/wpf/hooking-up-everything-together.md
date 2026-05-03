---
title: "Hooking up everything together" 
---
In this step we will hook everything together and add additional logic to the remaining view models.

## Hooking up the view models

We now have most of the application ready. However we need some logic in the view models to hook up everything together.

### Adding additional logic to FamilyWindowViewModel

The first thing we are going to do is to finalize the `FamilyWindowViewModel` we created in the previous step. To do this, we are going to add a few properties and commands to the view model.

#### Adding additional dependencies being injected

Since we will be using additional services inside the `FamilyWindowViewModel`, it is important to add them as dependency via the constructor. The updated constructor will look like this:

```
public FamilyWindowViewModel(Family family, IUIVisualizerService uiVisualizerService, IMessageService messageService)
{
    Argument.IsNotNull(() => family);
    Argument.IsNotNull(() => uiVisualizerService);
    Argument.IsNotNull(() => messageService);

    Family = family;
    _uiVisualizerService = uiVisualizerService;
    _messageService = messageService;
}
```

Don't forget to create the right backing fields `_uiVisualizerService` and `_messageService`

#### Adding the properties

We need a property representing the currently selected person in edit mode of a family. Below is the property definition which needs to be added to the view model:

```
/// <summary>
/// Gets or sets the selected person.
/// </summary>
public Person SelectedPerson
{
    get { return GetValue<Person>(SelectedPersonProperty); }
    set { SetValue(SelectedPersonProperty, value); }
}

/// <summary>
/// Register the SelectedPerson property so it is known in the class.
/// </summary>
public static readonly PropertyData SelectedPersonProperty = RegisterProperty("SelectedPerson", typeof(Person), null);
```

#### Adding the commands

Note that we recommend that you use the *vmtaskcommand* and *vmtaskcommandwithcanexecute* code snippets available [here](http://www.catelproject.com/download/general-files/)

Below is the code which comes in two parts.

1. Add this code to the constructor:

```
AddPerson = new TaskCommand(OnAddPersonExecuteAsync);
EditPerson = new TaskCommand(OnEditPersonExecuteAsync, OnEditPersonCanExecute);
RemovePerson = new TaskCommand(OnRemovePersonExecuteAsync, OnRemovePersonCanExecute);
```

2. Add this code to the view model itself:

```csharp
 /// <summary>
/// Gets the AddPerson command.
/// </summary>
public TaskCommand AddPerson { get; private set; }

/// <summary>
/// Method to invoke when the AddPerson command is executed.
/// </summary>
private async Task OnAddPersonExecuteAsync()
{
    var person = new Person();
    person.LastName = FamilyName;
    var personViewModel = _viewModelFactory.CreateViewModel<PersonViewModel>(person);
    if (await _uiVisualizerService.ShowDialogAsync(personViewModel) ?? false)
    {
        Persons.Add(person);
    }
}

/// <summary>
/// Gets the EditPerson command.
/// </summary>
public TaskCommand EditPerson { get; private set; }

/// <summary>
/// Method to check whether the EditPerson command can be executed.
/// </summary>
/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
private bool OnEditPersonCanExecute()
{
    return SelectedPerson != null;
}

/// <summary>
/// Method to invoke when the EditPerson command is executed.
/// </summary>
private async Task OnEditPersonExecuteAsync()
{
    var personViewModel = _viewModelFactory.CreateViewModel<PersonViewModel>(SelectedPerson);
    await _uiVisualizerService.ShowDialogAsync(personViewModel);
}

/// <summary>
/// Gets the RemovePerson command.
/// </summary>
public TaskCommand RemovePerson { get; private set; }

/// <summary>
/// Method to check whether the RemovePerson command can be executed.
/// </summary>
/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
private bool OnRemovePersonCanExecute()
{
    return SelectedPerson != null;
}

/// <summary>
/// Method to invoke when the RemovePerson command is executed.
/// </summary>
private async Task OnRemovePersonExecuteAsync()
{
    if (await _messageService.ShowAsync(string.Format("Are you sure you want to delete the person '{0}'?", SelectedPerson), 
        "Are you sure?", MessageButton.YesNo, MessageImage.Question) == MessageResult.Yes)
    {
        Persons.Remove(SelectedPerson);
        SelectedPerson = null;
    }
}
```

### Adding additional logic to the MainWindowViewModel

The same edit functionality we added to the `FamilyWindowViewModel` must be added to the `MainWindowViewModel`. The difference is that instead of adding / editing / removing persons, the `MainWindowViewModel` will do this for families.

#### Adding additional dependencies being injected

We will again need additional dependencies. Below is the updated constructor for the `MainWindowViewModel`:

```
public MainWindowViewModel(IFamilyService familyService, IUIVisualizerService uiVisualizerService, IMessageService messageService)
{
    Argument.IsNotNull(() => familyService);
    Argument.IsNotNull(() => uiVisualizerService);
    Argument.IsNotNull(() => messageService);

    _familyService = familyService;
    _uiVisualizerService = uiVisualizerService;
    _messageService = messageService;
}
```

#### Adding the properties

We will again need a property to handle the selected family:

```
/// <summary>
/// Gets or sets the selected family.
/// </summary>
public Family SelectedFamily
{
    get { return GetValue<Family>(SelectedFamilyProperty); }
    set { SetValue(SelectedFamilyProperty, value); }
}

/// <summary>
/// Register the SelectedFamily property so it is known in the class.
/// </summary>
public static readonly PropertyData SelectedFamilyProperty = RegisterProperty("SelectedFamily", typeof(Family), null);
```

#### Adding the commands

Last but not least, we will also add the commands to the `MainWindowViewModel` to handle the logic.

1. Add this code to the constructor:

```
AddFamily = new TaskCommand(OnAddFamilyExecuteAsync);
EditFamily = new TaskCommand(OnEditFamilyExecuteAsync, OnEditFamilyCanExecute);
RemoveFamily = new TaskCommand(OnRemoveFamilyExecuteAsync, OnRemoveFamilyCanExecute);
```

2. Add this code to the view model itself:

```csharp
/// <summary>
/// Gets the AddFamily command.
/// </summary>
public TaskCommand AddFamily { get; private set; }

/// <summary>
/// Method to invoke when the AddFamily command is executed.
/// </summary>
private async Task OnAddFamilyExecuteAsync()
{
    var family = new Family();
    var familyWindowViewModel = _viewModelFactory.CreateViewModel<FamilyWindowViewModel>(family);
    if (await _uiVisualizerService.ShowDialogAsync(familyWindowViewModel) ?? false)
    {
        Families.Add(family);
    }
}

/// <summary>
/// Gets the EditFamily command.
/// </summary>
public TaskCommand EditFamily { get; private set; }

/// <summary>
/// Method to check whether the EditFamily command can be executed.
/// </summary>
/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
private bool OnEditFamilyCanExecute()
{
    return SelectedFamily != null;
}

/// <summary>
/// Method to invoke when the EditFamily command is executed.
/// </summary>
private async Task OnEditFamilyExecuteAsync()
{
    var familyWindowViewModel = _viewModelFactory.CreateViewModel<FamilyWindowViewModel>(SelectedFamily);
    await _uiVisualizerService.ShowDialogAsync(familyWindowViewModel);
}

/// <summary>
/// Gets the RemoveFamily command.
/// </summary>
public TaskCommand RemoveFamily { get; private set; }

/// <summary>
/// Method to check whether the RemoveFamily command can be executed.
/// </summary>
/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
private bool OnRemoveFamilyCanExecute()
{
    return SelectedFamily != null;
}

/// <summary>
/// Method to invoke when the RemoveFamily command is executed.
/// </summary>
private async Task OnRemoveFamilyExecuteAsync()
{
    if (await _messageService.ShowAsync(string.Format("Are you sure you want to delete the family '{0}'?", SelectedFamily),
        "Are you sure?", MessageButton.YesNo, MessageImage.Question) == MessageResult.Yes)
    {
        Families.Remove(SelectedFamily);
        SelectedFamily = null;
    }
}
```

## Hooking up the views

We now have all the views ready, but we don't see anything yet. The reason for this is that we haven't modified the `MainWindow` view yet. To do so, replace the xaml content with the xaml below:

```
 <Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="100" />
    </Grid.ColumnDefinitions>

    <ListBox Grid.Column="0" ItemsSource="{Binding Families}" SelectedItem="{Binding SelectedFamily}">
        <ListBox.ItemTemplate>
            <DataTemplate>
                <views:FamilyView DataContext="{Binding}" />
            </DataTemplate>
        </ListBox.ItemTemplate>
    </ListBox>

    <StackPanel Grid.Column="1">
        <Button Command="{Binding AddFamily}" Content="Add..." />
        <Button Command="{Binding EditFamily}" Content="Edit..." />
        <Button Command="{Binding RemoveFamily}" Content="Remove" />
    </StackPanel>
</Grid>
```

Now run the application and you should see your fully functional family management application.

# Up next

[Finalizing the application]({{< relref "getting-started/wpf/finalizing-the-application.md" >}})

