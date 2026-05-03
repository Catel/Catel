---
title: "Creating the view models" 
---
In this step we will create the view models. Since this is a simple application, just a few view models are required. A view model in essence is nothing more than a class that derives from the `ViewModelBase` class

The *vm* code snippet is available to create view models. There is also an on-line item template available for Catel view models

## Creating the PersonViewModel

Below is the class definition of the `PersonViewModel`. This view model will be used to show the details of a `Person` model.

```
namespace WPF.GettingStarted.ViewModels
{
    using Catel.MVVM;

    public class PersonViewModel : ViewModelBase
    {
    }
}
```

### Enabling model injection

In hierarchy views, it is important to manage the state of views and view models based on the actual context where the view (thus view model) is located. Catel does this by allowing model injection. The view models will only be created when the model is available within the context of the view.

```
public class PersonViewModel : ViewModelBase
{
    public PersonViewModel(Person person)
    {
        Argument.IsNotNull(() => person);

        Person = person;
    }

    /// <summary>
    /// Gets or sets the person.
    /// </summary>
    [Model]
    public Person Person
    {
        get { return GetValue<Person>(PersonProperty); }
        set { SetValue(PersonProperty, value); }
    }

    /// <summary>
    /// Register the Person property so it is known in the class.
    /// </summary>
    public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person), null);
}
```

Note that the `Person` property is decorated with the `Model` attribute. If the `Person` model implements `IEditableObject`, Catel will automatically call `IEditableObject.EndEdit` when the view model is saved, and `IEditableObject.CancelEdit` when the view model is canceled so that all changes on the model will be reverted. Note that `ModelBase` does not implement `IEditableObject` by default; if you want this behavior, implement `IEditableObject` in your model class.

### Exposing properties of a model

One powerful feature of Catel is that it can automatically map properties from a model to a view model. This way the user does not have to write repetitive code to map the properties from the model to the view model at startup and map the properties from view model to model when the view model is closed. Catel will take care of this all automatically.

```
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
public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);

/// <summary>
/// Gets or sets the last name.
/// </summary>
[ViewModelToModel("Person")]
public string LastName
{
    get { return GetValue<string>(LastNameProperty); }
    set { SetValue(LastNameProperty, value); }
}

/// <summary>
/// Register the LastName property so it is known in the class.
/// </summary>
public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);
```

Note that the properties are decorated with the *ViewModelToModel* attribute which enables the automatic mappings feature in Catel.

## Creating the FamilyViewModel

The `FamilyViewModel` must be set up the same way as the `PersonViewModel` above.

```
namespace WPF.GettingStarted.ViewModels
{
    using System.Collections.ObjectModel;
    using Catel;
    using Catel.Data;
    using Catel.MVVM;
    using WPF.GettingStarted.Models;

    public class FamilyViewModel : ViewModelBase
    {
        public FamilyViewModel(Family family)
        {
            Argument.IsNotNull(() => family);

            Family = family; 
        }

        /// <summary>
        /// Gets the family.
        /// </summary>
        [Model]
        public Family Family
        {
            get { return GetValue<Family>(FamilyProperty); }
            private set { SetValue(FamilyProperty, value); }
        }

        /// <summary>
        /// Register the Family property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FamilyProperty = RegisterProperty("Family", typeof(Family), null);

        /// <summary>
        /// Gets the family members.
        /// </summary>
        [ViewModelToModel("Family")]
        public ObservableCollection<Person> Persons
        {
            get { return GetValue<ObservableCollection<Person>>(PersonsProperty); }
            private set { SetValue(PersonsProperty, value); }
        }

        /// <summary>
        /// Register the Persons property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonsProperty = RegisterProperty("Persons", typeof(ObservableCollection<Person>), null);

        /// <summary>
        /// Gets or sets the family name.
        /// </summary>
        [ViewModelToModel("Family")]
        public string FamilyName
        {
            get { return GetValue<string>(FamilyNameProperty); }
            set { SetValue(FamilyNameProperty, value); }
        }

        /// <summary>
        /// Register the FamilyName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FamilyNameProperty = RegisterProperty("FamilyName", typeof(string));
    }
}
```

## Up next

[Creating the views (user controls)]({{< relref "getting-started/wpf/creating-the-user-controls.md" >}})

