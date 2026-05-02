---
title: "Customizing the serialization for specific models" 
description: ""
weight: 20
---
Catel has a default behavior for what gets serialized. It can be tweaked by including / excluding fields and properties by using the *IncludeInSerialization* and *ExcludeFromSerialization* attributes. But sometimes one needs more specific customization of the serialization for a specific type. This customization is possible via theÂ *ISerializerModifier*.

## Creating the modifier

To customize the serialization of a specific model type, one needs to implement theÂ *ISerializerModifier* interface. The example belows shows how to encrypt theÂ *Password* property on theÂ *Person* model class.

```
public class PersonSerializerModifier : SerializerModifierBase<Person>
{
    public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
    {
        if (string.Equals(memberValue.Name, "Password"))
        {
            memberValue.Value = EncryptionHelper.Encrypt(memberValue.Value);
        }
    }
Â 
    public override void DeserializeMember(ISerializationContext context, MemberValue memberValue)
    {
        if (string.Equals(memberValue.Name, "Password"))
        {
            memberValue.Value = EncryptionHelper.Decrypt(memberValue.Value);
        }
    }
}
```

## Registering the modifier

To register a modifier for a specific class, define theÂ *SerializerModifier* attribute:

```
[SerializerModifier(typeof(PersonSerializerModifier))]
public class Person : ModelBase
{
    // .. class contents
}
```

{{% notice warning %}}
Note that modifiers are inherited from base classes. When serializing, the modifiers defined on the most derived classes will be called last. When deserializing, the modifies defined on the most derived classes will be called first.
{{% /notice %}}

