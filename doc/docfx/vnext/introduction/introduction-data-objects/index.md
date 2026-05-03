---
title: "Introduction to data objects" 
---
It is very important to understand the data objects in Catel because they form the base pillar of all components used by the MVVM framework.

## The ObservableObject class

Very basic class implementing the System.ComponentModel.INotifyPropertyChanging and T:System.ComponentModel.INotifyPropertyChanged interfaces as well as Catels Catel.Data.IAdvancedNotifyPropertyChanging and Catels Catel.Data.IAdvancedNotifyPropertyChanged.

## The ModelBase class

The [ModelBase]({{< relref "catel-core/data-handling/modelbase.md" >}})(previously known as the `DataObjectBase`) class is a generic base class that can be used for all your data classes. This comes fully supporting property changed notifications and error checking.

## Creating your first data object

First of all, it is very important to realize that you shouldn't bore yourself with writing all the code below yourself. Catel contains lots of code snippets that allow you to create data objects very easily in a short amount of time.

This example shows the simplest way to declare a data object using the *ModelBase* class. By using a code snippet, the class is created in just 10 seconds.

**Code snippets**

-   model - Declares a data object based on the *ModelBase* class

**Steps**

1.  Create a new class file called *FirstModel.cs*.
2.  Inside the namespace, use the *model* codesnippet and fill in the name of the class, in this case *FirstModel*.

**Code**

```
/// <summary> 
/// FirstModel class which fully supports property changed notifications and error checking.
/// </summary> 
public class FirstModel : ModelBase
{
    #region Fields
    #endregion

    #region Constructors
    /// <summary> 
    /// Initializes a new object from scratch.
    /// </summary> 
    public FirstModel()
    { }
    #endregion 

    #region Properties
    // TODO: Define your custom properties here using the propdata code snippet
    #endregion 

    #region Methods
    /// <summary>
    /// Validates the field values of this object. Override this method to enable
    /// validation of field values.
    /// </summary>
    /// <param name="validationResults">The validation results, add additional results to this list.</param>
    protected override void ValidateFields(List<FieldValidationResult> validationResults)
    {
    }

    /// <summary>
    /// Validates the field values of this object. Override this method to enable
    /// validation of field values.
    /// </summary>
    /// <param name="validationResults">The validation results, add additional results to this list.</param>
    protected override void ValidateBusinessRules(List<BusinessRuleValidationResult> validationResults)
    {
    }
    #endregion 
}
```

## Declaring properties

The next step to learn on the *ModelBase* class is how to declare properties. There are several types of properties, and they will all be handled in this part of the documentation.

The *ModelBase* class uses a dependency property a-like notation of properties.

### Simple properties

This example shows how to declare the simplest property. In this example, a string property with a default value will be declared with the use of a code snippet.

**Code snippets**

-   modelprop - Declares a simple property on a model

**Steps**

1.  Open *FirstModel.cs* created in the previous step.
2.  In the Properties region, use the code snippet *modelprop*, and use the following values:

Code snippet item|Value
---|---
description|Gets or sets the simple property
type|string
name|SimpleProperty
defaultvalue|"Simple property"

**Code**

```
/// <summary> 
/// Gets or sets the simple property.
/// </summary> 
public string SimpleProperty
{
    get { return GetValue<string>(SimplePropertyProperty); }
    set { SetValue(SimplePropertyProperty, value); }
}

/// <summary> 
/// Register the SimpleProperty property so it is known in the class.
/// </summary> 
public static readonly PropertyDataSimplePropertyProperty = RegisterProperty("SimpleProperty", typeof(string), "Simple property");
```

### Properties with property change callback

**Code snippets**

-   modelpropchanged - Declares a simple property on a model with a property changed callback

**Steps**

1.  Open *FirstModel.cs* created in the previous step.
2.  In the Properties region, use the code snippet *modelpropchanged*, and use the following values:

Code snippet item|Value
---|---
description|Gets or sets the callback property
type|string
name|CallbackProperty
defaultvalue|"Callback property"

**Code**

```
/// <summary> 
/// Gets or sets the callback property.
/// </summary> 
public string CallbackProperty
{
    get { return GetValue<string>(CallbackPropertyProperty); }
    set { SetValue(CallbackPropertyProperty, value); }
}

/// <summary> 
/// Register the CallbackProperty property so it is known in the class.
/// </summary> 
public static readonly PropertyDataCallbackPropertyProperty = RegisterProperty("CallbackProperty", typeof(string), "Callback property", 
    (sender, e) => ((FirstDataObject)sender).OnCallbackPropertyChanged());

/// <summary> 
/// Called when the CallbackProperty property has changed.
/// </summary> 
private void OnCallbackPropertyChanged()
{
    // TODO: Implement logic
}
```

## Adding validation

It is very easy to add validation to a class (both the *ModelBase* and *ViewModelBase*). There are several ways, but this getting started guide will handle only the most simple one.

To enable validation, you must override at least one of the following methods:

```
/// <summary>
/// Validates the field values of this object. Override this method to enable
/// validation of field values.
/// </summary>
/// <param name="validationResults">The validation results, add additional results to this list.</param>
protected override void ValidateFields(List<FieldValidationResult> validationResults)
{
    if (string.IsNullOrEmpty(FirstName))
    {
        validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "First name cannot be empty"));
    }
}

/// <summary>
/// Validates the field values of this object. Override this method to enable
/// validation of field values.
/// </summary>
/// <param name="validationResults">The validation results, add additional results to this list.</param>
protected override void ValidateBusinessRules(List<BusinessRuleValidationResult> validationResults)
{
    if (SomeBusinessErrorOccurs)
    {
        validationResults.Add(BusinessRuleValidationResult.CreateError("A business error occurred"));
    }
}
```

After the validation is implemented into the object, the validation will occur every time a property on the object changes. It is also possible to manually validate by calling the Validate method.

There are also other ways to add validation to a data object:

- Validation via data annotations - attributes such as the *RequiredAttribute*
- Validation via *IValidator* - custom validation such as *FluentValidation*

The great thing is that Catel will gather all validation results from all different mappings and combine these into the *ValidationContext*. This context can be used to query all sorts of validation info about an object.

Note that this is just an introduction, more information about validation can be found in other parts of the documentation

## Saving objects

> **Note:** `SavableModelBase` has been removed in Catel 7. The built-in serialization engine was removed. For serializing objects to disk, use alternatives such as `System.Text.Json`, `Newtonsoft.Json`, or `Orc.Serialization`.

