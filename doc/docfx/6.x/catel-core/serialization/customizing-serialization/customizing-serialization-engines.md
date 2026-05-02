---
title: "Customizing the serialization engines" 
---
Since the *SerializerBase* does all the heavy lifting, it is very easy to customize the behavior of an existing serializer or create a completely new one. Each serializer implements its own interface and are registered in the *ServiceLocator *using the following interfaces:

-   XmlSerializer =\> IXmlSerializer
-   BinarySerializer =\> IBinarySerializer

To customize a serializer, derive from an existing class and customize a method. The serializer below makes sure that specific members are never serialized. It keeps all other serialization logic intact.

```
public class SafeXmlSerializer : XmlSerializer
{
    protected override bool ShouldIgnoreMember(object model, PropertyData property)
    {
        if (model is SecurityModel)
        {
            if (string.Equals(property.Name, "Password"))
            {
                return true;
            }
        }
        return base.ShouldIgnoreProperty(model, property);
    }
}
```

The only thing to do now is to register this custom instance in the *ServiceLocator*:

```
ServiceLocator.Default.RegisterType<IXmlSerializer, SafeXmlSerializer>();
```

The following methods on the serializer classes might be of interest when customizing the serialization:

-   ShouldIgnoreProperty

-   BeforeSerialization
-   BeforeSerializeProperty
-   AfterSerializeProperty
-   AfterSerialization

-   BeforeDeserialization
-   BeforeDeserializeProperty
-   AfterDeserializeProperty
-   AfterDeserialization

