---
title: "ModelBase" 
---
The *ModelBase* class is a generic base class that can be used for all your data classes.

- **Support property changed notifications**
 The class supports the *INotifyPropertyChanging* and *INotifyPropertyChanged* interfaces so this class can be used in applications to reflect changes to the user.
- **Backup & revert**
 To support backup and revert functionality, implement the *IEditableObject* interface in a derived class. If the model implements *IEditableObject*, the view model framework will automatically call *BeginEdit*, *EndEdit*, and *CancelEdit* at the appropriate times.

## Using the class

Using the class is extremely simple. Just declare a new class that derives from *ModelBase* and you are ready to go:

```
/// <summary>
/// MyObject class which fully supports property changed notifications and error checking.
/// </summary>
public class MyObject : ModelBase
{
    /// <summary>
    /// Initializes a new object from scratch.
    /// </summary>
    public MyObject() { }
}
```

### Defining properties

Defining properties for the class is easy, and works the same like dependency properties. The advantages of this way of defining properties are:

- You can specify a default value for a property which will be used when the class is constructed;
- The *PropertyData* object can be used to retrieve property values so the compiler checks for errors;
- You can directly subscribe to change notifications, and all properties automatically support *INotifyPropertyChanged* out of the box.

Below is the code that defines a new property Name of type string:

```
/// <summary>
/// Gets or sets the name.
/// </summary>
public string Name
{
    get { return GetValue<string>(NameProperty); }
    set { SetValue(NameProperty, value); }
}

/// <summary>
/// Register the Name property so it is known in the class.
/// </summary>
public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);
```

### Default values for reference types

In many cases, a default value for reference types is required in the property definitions. However, and you might have noticed this behavior in for example dependency properties, using an instance as default value can result in unexpected behavior.

Below is an example of a "regular" property registration using a default value for a collection property:

```
public static readonly PropertyData NameProperty = RegisterProperty("PersonCollection", typeof(Collection<Person>), new Collection<Person>());
```

However, instead of creating a new collection for each new object with this property, only one collection will be created that will be used by all classes that have this property registered. One solution is to pass null as default value and create the collection in the constructor. A better solution is to use the override of *RegisterProperty* with the callback parameters:

```
public static readonly PropertyData NameProperty = RegisterProperty("PersonCollection", typeof(Collection<Person>), () => new Collection<Person>());
```

This way, every time a new value is needed, the callback will be invoked to create the default value and you will have a true default value for reference types.

## Functionality provided out of the box

The `ModelBase` provides many functionality out of the box. A few points I want to mention are:

**INotifyPropertyChanged**

All properties registered using the *RegisterProperty* method automatically take care of change notifications.

**IEditableObject**

`ModelBase` does not implement `IEditableObject`. If you need backup and restore functionality on your model, implement the `IEditableObject` interface in a derived class. When the model implements `IEditableObject`, the view model framework will automatically call `BeginEdit`, `EndEdit`, and `CancelEdit` at the appropriate times.

Note that this class is not suitable for database communication, there are much better ways to handle this (ORM mappers such as Entity Framework, NHibernate, LLBLGen Pro, etc.).

