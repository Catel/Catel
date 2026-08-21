---
title: "Validation via special model validators" 
---
By default, Catel registers the *AttributeValidatorProvider* as the *IValidatorProvider*. This way the *ModelBase* and all the classes that derive from it can add a custom validator by using the *ValidateModelAttribute*.

Note that it is still possible to register a custom *IValidatorProvider* to customize this behavior. It is even possible to set the *Validator* property of the *ModelBase* on a specific instance of a model

## Implementing the validator

The first thing that needs to be done is to write a custom implementation of the *IValidator* interface. You can either implement all the members yourself or derive from *ValidatorBase *as is shown below:

```
public class PersonValidator : ValidatorBase<PersonModel>
{
    protected override void ValidateFields(PersonModel instance, List<IFieldValidationResult> validationResults)
    {
        if (string.IsNullOrWhiteSpace(instance.FirstName))
        {
            validationResults.Add(FieldValidationResult.CreateError(PersonModel.FirstNameProperty, "First name is required"));
        }

        if (string.IsNullOrWhiteSpace(instance.LastName))
        {
            validationResults.Add(FieldValidationResult.CreateError(PersonModel.FirstNameProperty, "First name is required"));
        }
    }

    protected override void ValidateBusinessRules(PersonModel instance, List<IBusinessRuleValidationResult> validationResults)
    {
        // No business rules validations yet
    }
}
```

Decorating a model with the attribute

Once a validator is available, the only thing that needs to be done is to decorate the model with the *ValidateModelAttribute*:

```
[ValidateModel(typeof(PersonValidator))]
public class PersonModel : ModelBase
{
    public string FirstName
    {
        get { return GetValue<string>(FirstNameProperty); }
        set { SetValue(FirstNameProperty, value); }
    }

    public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);

    public string LastName
    {
        get { return GetValue<string>(LastNameProperty); }
        set { SetValue(LastNameProperty, value); }
    }

    public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), string.Empty);
}
```

The custom validator will now automatically be called.

