---
title: "Validation via IValidator" 
---
The ViewModelBase derives from ModelBase, thus all information here also applies to the ViewModelBase

The validation in Catel is extremely flexible, but sometimes it is just not enough or you are forced to use external validators. For such cases, Catel provides the IValidatorProvider and IValidator interfaces. These allow very flexible injection or external validators into data objects and view models of Catel.

## Implementing the IValidatorProvider

The IValidatorProvider is responsible to return the right IValidator for a specific type. There is a convenience implementation named ValidatorProviderBase which only requires the implementation of one single method. Below is an example of an implementation of the IValidatorProvider.

```
public class ValidatorProvider : ValidatorProviderBase
{
    /// <summary>
    /// Gets a validator for the specified target type.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <returns>
    /// The <see cref="T:Catel.Data.IValidator"/> for the specified type or <c>null</c> if no validator is available for the specified type.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="targetType"/> is <c>null</c>.</exception>
    public override IValidator GetValidator(Type targetType)
    {
        if (targetType == typeof(ValidationInIValidatorViewModel))
        {
            return new Validator();
        }

        // No validator available for other types
        return null;
    }
}
```

## Implementing the IValidator

The IValidator exposes lots of methods to gain the as much freedom as possible. However, most of the methods that are exposed by the interface are hardly used. Therefore there is a convenience base class named ValidatorBase. To create a basic validator, derive from the class and override the methods required for validation.

```
public class Validator : ValidatorBase<TargetClass>
{
    /// <summary>
    /// Validates the fields of the specified instance. The results must be added to the list of validation
    /// results.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="validationResults">The validation results.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
    public override void ValidateFields(TargetClass instance, List<IFieldValidationResult> validationResults)
    {
        if (string.IsNullOrEmpty(instance.FirstName))
        {
            validationResults.Add(FieldValidationResult.CreateError(TargetClass.FirstNameProperty, "First name cannot be empty"));
        }

        if (string.IsNullOrEmpty(instance.LastName))
        {
            validationResults.Add(FieldValidationResult.CreateError(TargetClass.LastNameProperty, "Last name cannot be empty"));
        }
    }

    /// <summary>
    /// Validates the business rules of the specified instance. The results must be added to the list of validation
    /// results.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="validationResults">The validation results.</param>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
    public override void ValidateBusinessRules(TargetClass instance, List<IBusinessRuleValidationResult> validationResults)
    {
        // No business rules (yet)
    }
}
```

## Setting the validator in ModelBase

To register an IValidator instance on a ModelBase, use the following code:

```
var modelValidation = myModel as IModelValidation;
if (modelValidation != null)
{
    modelValidation .Validator = new MyValidator();
}
```

If an IValidatorProvider instance is available, the following code can be used to allow a more generic approach. Inject `IValidatorProvider` via the constructor:

```csharp
var validator = _validatorProvider.GetValidator(myObject.GetType());
myObject.Validator = validator;
```

If the IValidatorProvider returns null (which is allowed), no custom validator will be used.

## Setting the validator in ViewModelBase

The easiest way to support a validator in a ViewModelBase is to register an `IValidatorProvider` instance in the service collection:

```csharp
services.AddSingleton<IValidatorProvider, MyValidatorProvider>();
```

The ViewModelBase will automatically retrieve the right IValidator for the view model. If no IValidatorProvider is registered, no validator will be set automatically. It is also possible to set the Validator property manually, but it is recommended to use an IValidatorProvider and register it.

