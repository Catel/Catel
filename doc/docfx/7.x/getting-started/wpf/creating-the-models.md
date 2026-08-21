---
title: "Creating the models" 
---
In this step we will create the models. The application manages persons, so we need a `Person` model and a `Gender` enum.

## Creating the Gender enum

Add a new file `Gender.cs` to the `Models` folder:

```csharp
namespace Catel.Examples.PersonApplication.Models;

public enum Gender
{
    Unknown,
    Male,
    Female
}
```

## Creating the Person model

Models in Catel derive from `ModelBase` or `ValidatableModelBase`. `ValidatableModelBase` adds built-in support for validation via `ValidateFields` and `ValidateBusinessRules`.

When **Catel.Fody** is installed, you can write plain auto-properties and Catel.Fody will automatically generate the backing `PropertyData` registration and change notification at compile time. This removes all the repetitive boilerplate code.

Add a new file `Person.cs` to the `Models` folder:

```csharp
namespace Catel.Examples.PersonApplication.Models;

using System.Collections.Generic;
using Catel.Data;

public class Person : ValidatableModelBase
{
    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public Gender Gender { get; set; }

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
}
```

Note that validation is built directly into the model. Catel will automatically surface these validation errors through `IDataErrorInfo` so that WPF validation bindings work out of the box.

## Up next

[Creating the view models]({{< relref "getting-started/wpf/creating-the-view-models.md" >}})


