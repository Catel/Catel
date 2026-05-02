---
title: "Validation via validate methods" 
description: ""
weight: 10
---
{{% notice info %}}
The ViewModelBase derives from ModelBase, thus all information here also applies to the ViewModelBase
{{% /notice %}}

The easiest way to implement validation is to override the ValidateFields and ValidateBusinessRules methods. Below is an example of an implementation of the ValidateFields method:

```
protected override void ValidateFields(List<IFieldValidationResult> validationResults)
{
    if (string.IsNullOrEmpty(FirstName))
    {
        validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "First name is required"));
    }

    if (string.IsNullOrEmpty(LastName))
    {
        validationResults.Add(FieldValidationResult.CreateError(LastNameProperty, "Last name is required"));
    }

    if (Gender == Gender.Unknown)
    {
        validationResults.Add(FieldValidationResult.CreateError(GenderProperty, "Gender cannot be unknown"));
    }
}
```

