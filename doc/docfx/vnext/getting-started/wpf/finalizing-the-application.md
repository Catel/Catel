---
title: "Finalizing the application" 
---
The application we have created so far is fully functional, but misses a bit of the "magic". Below are some additional steps that might make your application more appealing and more user friendly. Of course you can go as far as you want by creating custom animations and such, but this guide focuses purely on making the basics more appealing.

## Adding validation

Adding validation with Catel is extremely easy. There are two flavors to pick from, but they work exactly the same (since both the models and view models internally derive from `ValidatableModelBase`). To add validation to the `Person` model, use this code:

```
protected override void ValidateFields(List<IFieldValidationResult> validationResults)
{
    if (string.IsNullOrWhiteSpace(FirstName))
    {
        validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "The first name is required"));
    }

    if (string.IsNullOrWhiteSpace(LastName))
    {
        validationResults.Add(FieldValidationResult.CreateError(LastNameProperty, "The last name is required"));
    }
}
```

The validation for the `Family` model is very easy as well:

```
protected override void ValidateFields(List<IFieldValidationResult> validationResults)
{
    if (string.IsNullOrWhiteSpace(FamilyName))
    {
        validationResults.Add(FieldValidationResult.CreateError(FamilyNameProperty, "The family name is required"));
    }
}
```

Note that this validation code can be used in both the model and/or the view models

## Adding behaviors to enable double-click on the list boxes

The user must manually click the *Edit* buttons in the editable views to edit a specific model. To make it easier for the user, we can enable double click to command behaviors. To do so, navigate to the `MainWindow` and add this to the `ListBox` definition:

```
<ListBox x:Name="listBox" ItemsSource="{Binding Families}" SelectedItem="{Binding SelectedFamily}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Grid>
                <i:Interaction.Behaviors>
                    <catel:DoubleClickToCommand Command="{Binding ElementName=listBox, Path=DataContext.EditFamily}" />
                </i:Interaction.Behaviors>

                <views:FamilyView DataContext="{Binding}" />
            </Grid>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

The same goes for the `FamilyWindow`:

```
<ListBox x:Name="listBox" ItemsSource="{Binding Persons}" SelectedItem="{Binding SelectedPerson}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Grid>
                <i:Interaction.Behaviors>
                    <catel:DoubleClickToCommand Command="{Binding ElementName=listBox, Path=DataContext.EditPerson}" />
                </i:Interaction.Behaviors>

                <views:PersonView DataContext="{Binding}" />
            </Grid>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>

```

Note that the `xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"` must be added in order for the code above to compile

## Adding search functionality to the main window

A functionality that is needed in a lot of applications is search functionality. To implement this we will need to modify the `MainWindowViewModel`. Below are the steps required to implement search functionality.

### Adding additional properties to the view model

Lets start by adding the additional properties required to implement searching in the `MainWindowViewModel`:

```
/// <summary>
/// Gets the filtered families.
/// </summary>
public ObservableCollection<Family> FilteredFamilies
{
    get { return GetValue<ObservableCollection<Family>>(FilteredFamiliesProperty); }
    private set { SetValue(FilteredFamiliesProperty, value); }
}

/// <summary>
/// Register the FilteredFamilies property so it is known in the class.
/// </summary>
public static readonly PropertyData FilteredFamiliesProperty = RegisterProperty("FilteredFamilies", typeof(ObservableCollection<Family>));

/// <summary>
/// Gets or sets the search filter.
/// </summary>
public string SearchFilter
{
    get { return GetValue<string>(SearchFilterProperty); }
    set { SetValue(SearchFilterProperty, value); }
}

/// <summary>
/// Register the SearchFilter property so it is known in the class.
/// </summary>
public static readonly PropertyData SearchFilterProperty = RegisterProperty("SearchFilter", typeof(string), null, (sender, e) => ((MainWindowViewModel)sender).UpdateSearchFilter());
```

Note that this property contains an additional change callback function which will be called when the property has changed.

Add the following import to the view model. You will needed because native `ObservableCollection` class does not support `ReplaceRange()`

```
using Catel.Collections;
```

Add this method to the view model:

```
/// <summary>
/// Updates the filtered items.
/// </summary>
private void UpdateSearchFilter()
{
    if (FilteredFamilies == null)
    {
        FilteredFamilies = new ObservableCollection<Family>();
    }

    if (string.IsNullOrWhiteSpace(SearchFilter))
    {
        FilteredFamilies.ReplaceRange(Families);
    }
    else
    {
        var lowerSearchFilter = SearchFilter.ToLower();
        FilteredFamilies.ReplaceRange(from family in Families
                                      where !string.IsNullOrWhiteSpace(family.FamilyName) && family.FamilyName.ToLower().Contains(lowerSearchFilter)
                                      select family);
    }
}
```

Then, add this code to the `OnAddFamilyExecute` function:

```
private async Task OnAddFamilyExecuteAsync()
{
    var family = new Family();

    // Note that we use the type factory here because it will automatically take care of any dependencies
    // that the FamilyWindowViewModel will add in the future
    var typeFactory = this.GetTypeFactory();
    var familyWindowViewModel = typeFactory.CreateInstanceWithParametersAndAutoCompletion<FamilyWindowViewModel>(family);
    if (await _uiVisualizerService.ShowDialog(familyWindowViewModel) ?? false)
    {
        Families.Add(family);
        UpdateSearchFilter();
    }
}
```

Last but not least, add this to the `InitializeAsync` method **after** the `Families` is set from the `IFamilyService`

```
protected override async Task InitializeAsync()
{
    var families = _familyService.LoadFamilies();
    Families = new ObservableCollection<Family>(families);

    UpdateSearchFilter();
}
```

### Adding the search functionality to the view

Replace the xaml of the main window by the following content:

```
 <Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>

    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="100" />
    </Grid.ColumnDefinitions>

    <Grid Grid.Row="0" Grid.ColumnSpan="2">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <Label Grid.Row="0" Grid.Column="0" Content="Filter:" />
        <TextBox Grid.Row="0" Grid.Column="1" Text="{Binding SearchFilter}">
            <i:Interaction.Behaviors>
                <catel:UpdateBindingOnTextChanged UpdateDelay="500" />
            </i:Interaction.Behaviors>
        </TextBox>
    </Grid>

    <ListBox Grid.Row="1" Grid.Column="0" x:Name="listBox" ItemsSource="{Binding FilteredFamilies}" SelectedItem="{Binding SelectedFamily}">
        <ListBox.ItemTemplate>
            <DataTemplate>
                <Grid>
                    <i:Interaction.Behaviors>
                        <catel:DoubleClickToCommand Command="{Binding ElementName=listBox, Path=DataContext.EditFamily}" />
                    </i:Interaction.Behaviors>
                    <views:FamilyView DataContext="{Binding}" />
                </Grid>
            </DataTemplate>
        </ListBox.ItemTemplate>
    </ListBox>

    <StackPanel Grid.Row="1" Grid.Column="1">
        <Button Command="{Binding AddFamily}" Content="Add..." />
        <Button Command="{Binding EditFamily}" Content="Edit..." />
        <Button Command="{Binding RemoveFamily}" Content="Remove" />
    </StackPanel>
</Grid>
```

