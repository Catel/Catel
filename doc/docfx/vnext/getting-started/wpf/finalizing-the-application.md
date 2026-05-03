---
title: "Finalizing the application" 
---
The application we have created so far is fully functional. Below are some additional steps that make it more polished and user-friendly.

## Validation

Validation with Catel is straightforward. Both models and view models internally derive from `ValidatableModelBase`, so validation can be added in either layer.

### Model validation

The `Person` model already contains field validation (added in the *Creating the models* step):

```csharp
protected override void ValidateFields(List<IFieldValidationResult> validationResults)
{
    if (string.IsNullOrEmpty(FirstName))
    {
        validationResults.Add(FieldValidationResult.CreateError(nameof(FirstName), "First name is required"));
    }

    if (string.IsNullOrEmpty(LastName))
    {
        validationResults.Add(FieldValidationResult.CreateError(nameof(LastName), "Last name is required"));
    }

    if (Gender == Gender.Unknown)
    {
        validationResults.Add(FieldValidationResult.CreateError(nameof(Gender), "Gender cannot be unknown"));
    }
}
```

Because the `PersonWindow` binds with `ValidatesOnDataErrors=True, NotifyOnValidationError=True`, WPF will automatically display the validation errors next to the relevant fields.

### View model validation

You can also add validation at the view model level. This is useful for cross-field or business-rule validation that does not belong in the model:

```csharp
protected override void ValidateFields(List<IFieldValidationResult> validationResults)
{
    if (!string.IsNullOrEmpty(CustomError))
    {
        validationResults.Add(FieldValidationResult.CreateError(nameof(CustomError), CustomError));
    }
}
```

## Double-click to edit

To allow the user to double-click a person in the `MainWindow` list to open the editor, use the `xamlbehaviors:EventTrigger` together with `catel:EventToCommand`. This is already included in the `MainWindow` XAML shown in the *Creating the views (windows)* step:

```xml
<xamlbehaviors:Interaction.Triggers>
    <xamlbehaviors:EventTrigger EventName="MouseDoubleClick">
        <catel:EventToCommand Command="{Binding Edit}"
                               DisableAssociatedObjectOnCannotExecute="False" />
    </xamlbehaviors:EventTrigger>
</xamlbehaviors:Interaction.Triggers>
```

Add `xmlns:xamlbehaviors="http://schemas.microsoft.com/xaml/behaviors"` to the window declaration if it is not already present.

## Adding custom buttons to DataWindow

The `DataWindow` base class supports custom buttons in addition to the standard **OK** / **Cancel** pair. Pass a command name (matching a `TaskCommand` or `Command` property on the view model) to `AddCustomButton`:

```csharp
AddCustomButton(new DataWindowButton("Generate data", nameof(PersonViewModel.GenerateData)));
AddCustomButton(new DataWindowButton("Toggle error", nameof(PersonViewModel.ToggleCustomError)));
```

Each button will automatically be enabled or disabled based on the command's `CanExecute` status.

## Complete example

The complete source code for this example is available at:

<https://github.com/Catel/Catel.Examples/tree/master/src/Catel.Examples.WPF.PersonApplication>


