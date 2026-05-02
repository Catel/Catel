---
title: "Creating the models" 
---
In this step we will create models. Since this application is about families and persons inside those families, we need to create the following models: *Settings*, *Family* and *Person*. 

## Creating the model classes

The models that will be used in this application will derive from the `ModelBase`, `ValidatableModelBase` or `SavableModelBase` class of Catel. These classes enable support for change notifications, validations and persistence. The `SavableModelBase` adds additional methods to save and load from/to streams or files without having to create a serializer first.

To create the model classes, create the following classes in the *Models* folder.

The *model* code snippet is available to create models

### Settings class

The settings class is the top container that will store all families and other settings (which might be added in the future).

```
namespace WPF.GettingStarted.Models
{
    using Catel.Data;

    public class Settings : SavableModelBase<Settings>
    {
    }
}
```

### Family class

```
namespace WPF.GettingStarted.Models
{
    using Catel.Data;

    public class Family : ValidatableModelBase
    {
    }
}
```

### Person class

```
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

At first sight, these properties might look very overwhelming. Let's take a look at how the property system works. The most important thing is the actual property registration:

```
public static readonly PropertyData PersonsProperty = RegisterProperty("Persons", typeof(ObservableCollection<Person>), () => new ObservableCollection<Person>());
```

This defines a property on the model with the following data:

-   Name =\> Persons
-   Type =\> ObservableCollection<Person\>
-   DefaultValue =\> new ObservableCollection<Person\>()

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

### Settings class

```
public class Settings : SavableModelBase<Settings>
{
    /// <summary>
    /// Gets or sets all the families.
    /// </summary>
    public ObservableCollection<Family> Families
    {
        get { return GetValue<ObservableCollection<Family>>(FamiliesProperty); }
        set { SetValue(FamiliesProperty, value); }
    }

    /// <summary>
    /// Register the Families property so it is known in the class.
    /// </summary>
    public static readonly PropertyData FamiliesProperty = RegisterProperty("Families", typeof(ObservableCollection<Family>), () => new ObservableCollection<Family>());
}
```

### Family class

```
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

```
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

