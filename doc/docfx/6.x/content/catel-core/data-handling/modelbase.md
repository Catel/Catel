---
title: "ModelBase" 
description: ""
weight: 30
---
The *ModelBase* class is a generic base class that can be used for all your data classes.

- **Fully serializable**
  It is now really easy to store objects on disk or serialize them into memory, either binary or in XML. The data object supports this out of the box, and automatically handles the (de)serialization.
- **Support property changed notifications**
  The class supports the *INotifyPropertyChanging* and *INotifyPropertyChanged* interfaces so this class can easily be used in applications to reflect changes to the user.
- **Backwards compatibility**
  When serializing your objects to binary, it is hard to maintain the right versions. When you add a new property to a binary class, or change a namespace, the object cannot be loaded any longer. The data object base takes care of this issue and supports backwards compatibility.
- **Backup & revert**
  The class implements the *IEditableObject* interface which makes it possible to create a state of the object. Then all properties can be edited, and finally, the changes can be applied or cancelled.

## Using the class

Using the class is extremely simple. Just declare a new class that derives from *ModelBase* and you are ready to go:

```
/// <summary>
/// MyObject class which fully supports serialization,
/// property changed notifications, backwards compatibility and error checking.
/// </summary>
[Serializable]
public class MyObject : ModelBase<MyObject>
{
    /// <summary>
    /// Initializes a new object from scratch.
    /// </summary>
    public MyObject() { }

/// <summary>
/// Initializes a new object based on <see cref="SerializationInfo"/>.
/// </summary>
/// <param name="info"><see cref="SerializationInfo"/>
//     that contains the information.</param>
/// <param name="context"><see cref="StreamingContext"/>.</param>
protected MyObject(SerializationInfo info, StreamingContext context)
    : base(info, context) { }
}
```

As you can see in the code above, the *MyObject* class derives from *ModelBase* and provides an empty constructor, but also a constructor that is used for binary deserialization. The code above might look complex, but it is created using the model code snippet, and you only have to type the name of the class.

### Defining properties

Defining properties for the class is very easy, and works the same like dependency properties. The advantages of this way of defining properties are:

-   Properties defined like this are automatically included during serialization; no need to specify complex data contracts;
-   You can specify a default value for a property which will be used when the class is constructed or the property is not found during deserialization (in case this property is added to an existing class);
-   The *PropertyData* object can be used to retrieve property values so the compiler checks for errors;
-   You can directly subscribe to change notifications, and all properties automatically support *INotifyPropertyChanged* out of the box.

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

A registered property can be excluded from serialization if wanted. When the object is deserialized, the default value will be used for the property in that case.

### Default values for reference types

In lots of cases, a default value for reference types is required in the property definitions. However, and you might have noticed this behavior in for example dependency properties, using an instance as default value can result in unexpected behavior.

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

The `ModelBase` provides a lot of functionality out of the box. A few points I want to mention are:

**INotifyPropertyChanged**

All properties registered using the *RegisterProperty* method automatically take care of change notifications.

**IEditableObject**

The data object can automatically create an internal backup and restore it, if required, using the *IEditableObject* interface.

**Serialization**

As told many times before, using the `[SavableModelBase]({{< relref "catel-core/data-handling/savablemodelbase.md" >}})`, you can simply save your file to a stream (file on disk, stream in memory, etc.).

Keep in mind that this class is not suitable for database communication, there are much better ways to handle this (ORM mappers such as Entity Framework, NHibernate, LLBLGen Pro, etc.).

