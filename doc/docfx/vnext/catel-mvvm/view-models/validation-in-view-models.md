---
title: "Validation in view models" 
---
Validation is very important to provide both feedback to the user, but also to make sure that no invalid data reaches the model or database. Catel offers several ways to implement validation. All options are described in this part of the documentation.

## Validation in models via mappings

The best way is to put validation into a model. Most model objects nowadays implement `INotifyPropertyChanged` and `IDataErrorInfo`, the most important classes that are required to use mappings from/to a model inside a view model.

The great advantage of mapping properties from/to models automatically using Catel is that you don't have to write lots of plumbing yourself (getting and setting values in the model and view model). However, if the model implements `INotifyPropertyChanged` and *IDataErrorInfo`, Catel also automatically uses the validation from the model. For example, if there is a `Person` model that checks if the `FirstName` and `LastName` are entered, why rewrite this validation again in the view model?

There are two ways to use automatic mappings.

### Mapping via ViewModelToModelAttribute

Mapping a model property by using the *ViewModelToModelAttribute* requires the definition of a model property and a separate property per mapped property. The code below automatically maps the *FirstName* property.

```
/// <summary>
/// Gets or sets the person.
/// </summary>
[Model]
public Person Person
{
    get { return GetValue<Person>(PersonProperty); }
    private set { SetValue(PersonProperty, value); }
}

/// <summary>
/// Register the Person property so it is known in the class.
/// </summary>
public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));

/// <summary>
/// Gets or sets the first name.
/// </summary>
[ViewModelToModel("Person")]
public string FirstName
{
    get { return GetValue<string>(FirstNameProperty); }
    set { SetValue(FirstNameProperty, value); }
}

/// <summary>
/// Register the FirstName property so it is known in the class.
/// </summary>
public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));
```

If the `Person.FirstName` provides an error via the `IDataErrorInfo` interface, it will automatically be exposed by the view model as well.

### Mapping via ExposeAttribute

Mapping a property via the `ExposeAttribute` is even more simple, but has some disadvantages. First, let's talk about the disadvantages before showing the code:

1.  Catel uses the *ICustomTypeDescriptor* interface to implement this behavior, and the interface is only available in WPF.
2.  The properties are not actually created, so you cannot use them inside your view model as real properties.

In other words, the `ExposeAttribute` is only very useful if 1) you are using WPF and 2) if the property is not used inside the view model but only declared to protect the model from the outside world. If both of these constraints are true, then the `ExposeAttribute` is definitely worth taking a look at. The usage is very simple:

```
/// <summary>
/// Gets or sets the person.
/// </summary>
[Model]
[Expose("FirstName")]
[Expose("MiddleName")]
[Expose("LastName")]
private  Person Person
{
    get { return GetValue<Person>(PersonProperty); }
    set { SetValue(PersonProperty, value); }
}

/// <summary>
/// Register the Person property so it is known in the class.
/// </summary>
public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));
```

Simply declare the model property and decorate it with one or multiple `ExposeAttribute` instances. Not only are the properties automatically available for binding, the view model also checks for errors and automatically maps these as well.

## Validation in view models

Until now, we only spoke about automatic validation for validation that was written in the model. However, sometimes it is required to write validation inside the view model as well. One reason might be that the model is a POCO object not providing any validation. Or, sometimes there is a logical error that has nothing to do with the model, but should be shown to the user anyway.

In such a case, Catel offers lots of possibilities to write custom validation inside the view model. Below are the possibilities:

-   Field warnings
-   Field errors
-   Business rule warnings
-   Business rule errors

The difference between a field and business rule is that a field error or warning is specific for a property. These are returned via `IDataErrorInfo["propertyName"]`. A business rule is a rule that applies to multiple fields or even a whole entity. Business rule validations are returned via `IDataErrorInfo.Error`.

To implement validation into a view model, only two methods need to be implemented. Catel clearly separates the field validation from the business rule validation to make it much clearer to the developer what is going on.

### Validating fields

To validate fields, one should override the *ValidateFields* method. Below is an example of field validation on a view model:

```
/// <summary>
/// Validates the field values of this object. Override this method to enable
/// validation of field values.
/// </summary>
/// <param name="validationResults">The validation results, add additional results to this list.</param>
protected override void ValidateFields(List<IFieldValidationResult> validationResults)
{
    if (!string.IsNullOrEmpty(FirstName))
    {
        validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "First name cannot be empty"));
    }
}
```

### Validating business rules

To validate business rules, one should override the `ValidateBusinessRules` method. Below is an example of business rule validation on a view model:

```
/// <summary>
/// Validates the field values of this object. Override this method to enable
/// validation of field values.
/// </summary>
/// <param name="validationResults">The validation results, add additional results to this list.</param>
protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
{
    if (SomeBusinessErrorOccurs)
    {
        validationResults.Add(BusinessRuleValidationResult.CreateError("A business error occurred"));
    }
}
```

## Translating model validation in the view model

Thanks to the validation system in Catel, it is very easy to implement very advanced validation features in view models. The example below shows how to translate errors that are added to a model in the Data Access Layer or validation layer. Assume that the following pseudo code is used to set an error on a model in the DAL:

```
SetFieldError(â€œFirstNameâ€, â€œFirstNameRequiredâ€);
```

All errors that are mapped from the model to the view model automatically are available in the `validationResults` parameter. This way, the error can be easily translated:

```
/// <summary>
/// Validates the field values of this object. Override this method to enable 
/// validation of field values.
/// </summary>
/// <param name="validationResults">The validation results, add additional results to this list.</param>
protected override void ValidateFields(List<IFieldValidationResult> validationResults)
{
   foreach (var validationResult in validationResults)
   {
      if (validationResult.Message == "FirstNameRequired")
      {
         validationResult.Message = Properties.Resources.FirstNameRequired; 
      }
   }
}
```

Of course this is not something you want to actually do in your view model, so youâ€™ll probably have to write a helper class that translates the validation for you. You might or might not like delaying the translation of the model errors to as close as the view, but it shows how extremely powerful the improved validation of Catel is. And if you think a bit about it,Â wouldn'tÂ it be a good idea to delay the translation from the server to the actual client to as close as the viewâ€¦?

## Validating via annotations

Some people like to add validation to their (view)models using annotations (attributes). Catel also supports this method, but adds additional functionality. The idea behind it is that in the end, Catel always provides all errors of an object via the `IDataErrorInfo` interface. This means that when attribute validation is used, the errors are internally registered and provided in the `ValidateFields` method. This way, all types of validation that are provided by the .NET framework are gathered into one single location where they can be used by the view.

```
/// <summary>
/// Gets or sets the first name.
/// </summary>
[Required("This value is required")]
private string FirstName
{
    get { return GetValue<string>(FirstNameProperty); }
    set { SetValue(FirstNameProperty, value); }
}

/// <summary>
/// Register the FirstName property so it is known in the class.
/// </summary>
public static readonly PropertyData FirstNameProperty= RegisterProperty("FirstName", typeof(string));
```

## To validate required fields or not to validate required fields at startup

Catel does not validate the properties with data annotations at startup. It will only validate the data annotations when properties change or when the view model is about to be saved. This is implemented this way to allow a developer to show required fields with an asterisk (\*) instead of errors. If a developer still wants to initially display errors, only a single call has to be made in the constructor:

```
Validate(true, false);
```

