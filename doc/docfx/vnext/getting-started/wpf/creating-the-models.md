---
title: "Creating the models" 
---
In this step we will create models. Since this application is about families and persons inside those families, we need to create the following models: *Family* and *Person*. 

## Creating the model classes

The models that will be used in this application will derive from the `ModelBase` or `ValidatableModelBase` class of Catel. These classes enable support for change notifications and validations.

To create the model classes, create the following classes in the *Models* folder.

The *model* code snippet is available to create models

### Family class

```csharp
namespace WPF.GettingStarted.Models
{
    using Catel.Data;

    public class Family : ValidatableModelBase
    {
    }
}
```

### Person class

```csharp
namespace WPF.GettingStarted.Models
{
    using Catel.Data;

    public class Person : ValidatableModelBase
    {
    }
}
```

## Adding properties to the models

The next step is to add properties to the models. An important concept to understand is that Catel uses specific "dependency-a-like" properties in order to provide all the functionality in the `ModelBase` classes. Below are the properties per model that need to be registered.

At first sight, these properties might look very overwhelming. examine how the property system works. The most important thing is the actual property registration:

```
public static readonly PropertyData PersonsProperty = RegisterProperty("Persons", typeof(ObservableCollection<Person>), () => new ObservableCollection<Person>());
```

This defines a property on the model with the following data:

- Name =\> Persons
- Type =\> ObservableCollection<Person\>
- DefaultValue =\> new ObservableCollection<Person\>()

This will create a property in the property bag of the model. The next piece of the property is the actual wrapper around the property value which is managed by the property bag. The Catel properties always need a wrapper to be exposed to the "outside world" of the class.

```
public ObservableCollection<Person> Persons
{
    get { return GetValue<ObservableCollection<Person>>(PersonsProperty); }
    set { SetValue(PersonsProperty, value); } 
}
```

If you want to get the functionality in the `ModelBase` classes without the "dependency-a-like" properties you have the option to make use of normal properties and Catel.Fody.

The *modelprop* code snippet is available to create models

### Family class

```csharp
public class Family : ValidatableModelBase
{
    /// <summary>
    /// Gets or sets the family name.
    /// </summary>
    public string FamilyName
    {
        get { return GetValue<string>(FamilyNameProperty); }
        set { SetValue(FamilyNameProperty, value); }
    }

    /// <summary>
    /// Register the FamilyName property so it is known in the class.
    /// </summary>
    public static readonly PropertyData FamilyNameProperty = RegisterProperty("FamilyName", typeof(string), null);

    /// <summary>
    /// Gets or sets the list of persons in this family.
    /// </summary>
    public ObservableCollection<Person> Persons
    {
        get { return GetValue<ObservableCollection<Person>>(PersonsProperty); }
        set { SetValue(PersonsProperty, value); }
    }

    /// <summary>
    /// Register the Persons property so it is known in the class.
    /// </summary>
    public static readonly PropertyData PersonsProperty = RegisterProperty("Persons", typeof(ObservableCollection<Person>), () => new ObservableCollection<Person>());

    public override string ToString()
    {
        return FamilyName;
    }
}
```

### Person class

```csharp
public class Person : ValidatableModelBase
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName
    {
        get { return GetValue<string>(FirstNameProperty); }
        set { SetValue(FirstNameProperty, value); }
    }

    /// <summary>
    /// Register the FirstName property so it is known in the class.
    /// </summary>
    public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName
    {
        get { return GetValue<string>(LastNameProperty); }
        set { SetValue(LastNameProperty, value); }
    }

    /// <summary>
    /// Register the LastName property so it is known in the class.
    /// </summary>
    public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);

    public override string ToString()
    {
        string fullName = string.Empty;
        if (!string.IsNullOrEmpty(FirstName))
        {
            fullName += FirstName;
        }

        if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrWhiteSpace(LastName))
        {
            fullName += " ";
        }

        if (!string.IsNullOrWhiteSpace(LastName))
        {
            fullName += LastName;
        }

        return fullName;
    }
}
```

## Up next

[Serializing data from/to disk]({{< relref "getting-started/wpf/serializing-data-from-to-disk.md" >}})


