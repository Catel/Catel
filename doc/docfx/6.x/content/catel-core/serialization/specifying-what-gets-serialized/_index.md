---
title: "Specifying what gets serialized" 
description: ""
weight: 10
---
By default, Catel only serializes the defined Catel properties on theÂ *ModelBase* or any deriving classes. It is possible to customize this behavior. Below is a class which will be used in all examples:

```
public class MyModel : ModelBase
{
    private string _fieldValue;

    public string RegularProperty { get; set; }

    public string CatelProperty
    {
        get { return GetValue<string>(CatelPropertyProperty); }
        set { SetValue(CatelPropertyProperty, value); }
    }

    public static readonly PropertyData CatelPropertyProperty = RegisterProperty("CatelProperty", typeof(string), null);
}
```

Member name|Gets serialized
---|---
_fieldValue|false
RegularProperty|false
CatelProperty|true

## Including fields and properties using IncludeInSerialization attribute

To include fields or regular properties on an object, use the following code:

```
public class MyModel : ModelBase
{
    [IncludeInSerialization]
    private string _fieldValue;

    [IncludeInSerialization]
    public string RegularProperty { get; set; }

    public string CatelProperty
    {
        get { return GetValue<string>(CatelPropertyProperty); }
        set { SetValue(CatelPropertyProperty, value); }
    }

    public static readonly PropertyData CatelPropertyProperty = RegisterProperty("CatelProperty", typeof(string), null);
}
```

Member name|Gets serialized
---|---
_fieldValue|true
RegularProperty|true
CatelProperty|true

{{% notice warning %}}
Note that private members can only be serialized in full .NET, not in limited platforms such as UWP
{{% /notice %}}

## Excluding fields and properties using ExcludeFromSerialization attribute

To exclude Catel properties on an object, use the following code:

```
public class MyModel : ModelBase
{
    private string _fieldValue;

    public string RegularProperty { get; set; }
Â 
    [ExcludeFromSerialization]
    public string CatelProperty
    {
        get { return GetValue<string>(CatelPropertyProperty); }
        set { SetValue(CatelPropertyProperty, value); }
    }

    public static readonly PropertyData CatelPropertyProperty = RegisterProperty("CatelProperty", typeof(string), null);
}
```

Member name|Gets serialized
---|---
_fieldValue|false
RegularProperty|false
CatelProperty|false

## Serializing a ModelBase as collection

There is a very edge case that a class derives fromÂ *ModelBase*, but also implements *IList\<T\>*. In this case, it's hard for the serializers to determine what to do. By default, it will treat the model as a Catel model (since it can contain more properties than just theÂ *Items* property. If such a class should be serialized as collection (meaning it will only serialize theÂ *Items* property), decorate it with theÂ *SerializeAsCollection* attribute:

Â 

```
[SerializeAsCollection]
public class MyModel : ModelBase, IList<int>
{
    // implementation left out for readability
}
```

## Implementing a custom ISerializationManager

Internally Catel uses a default implementation of theÂ *ISerializationManager* to determine what members of a type should be serialized. It is possible to customize this behavior by overriding a single method or by creating a brand new type. Below is an example which always excludesÂ *Password* properties and fields from serialization.

```
public class SafeSerializationManager : SerializationManager
{
    public override HashSet<string> GetFieldsToSerialize(Type type)
    {
        var fieldsList = new List<string>(base.GetFieldsToSerialize(type));

        for (int i = 0; i < fieldsList.Count; i++)
        {
            if (string.Equals(fieldsList[i], "_password"))
            {
                fieldsList.RemoveAt(i--);
            }
        }

        return new HashSet<string>(fieldsList);
    }

    public override HashSet<string> GetPropertiesToSerialize(Type type)
    {
        var propertiesList = new List<string>(base.GetPropertiesToSerialize(type));

        for (int i = 0; i < propertiesList.Count; i++)
        {
            if (string.Equals(propertiesList[i], "Password"))
            {
                propertiesList.RemoveAt(i--);
            }
        }

        return new HashSet<string>(propertiesList);
    }   
}
```

Don't forget to register it in the *ServiceLocator* as well:

```
var serviceLocator = ServiceLocator.Default;
serviceLocator.RegisterType<ISerializationManager, SafeSerializationManager>();
```

