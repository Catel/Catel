---
title: "Serialization" 
description: ""
---
## Serializing a model

The code below shows how to save an object (which can, of course, be a complex graph of nested objects):

```
var myObject = new MyObject();
Â 
using (var fileStream = File.Create(@"C:\myobject.dob"))
{
    myObject.Save(fileStream);
}
```

Looks too easy, but this really is the only thing you need to do. You can specify the serialization mode in the several available overloads of theÂ SaveÂ method.

Loading is as easy as saving, as you can see in the following code:

```
MyObject myObject = null;
Â 
using (var fileStream = File.Open(@"C:\myobject.dob", FileMode.Open))
{
    myObject = ModelBase.Load<MyObject>(fileStream, SerializationMode.Xml);    
}
```

{{% notice info %}}
Note that for a model to support the `Save` and `Load` methods, it must derive from `SavableModelBase`
{{% /notice %}}

## Warming up serialization

The first time a serializerÂ needs to serialize an object, it needs to perform some reflection to gather all the required information. This can have a negative impact on performance for the end-user during serialization. This load cannot be prevented, but it is possible to warmup the serializer at any time when it is convenient (for example, during startup of the application).

### Warming up specific types

This code will warm up all the specified types:

```
var typesToWarmup = new type[] {  typeof(Settings) };
Â 
var xmlSerializer = SerializationFactory.GetXmlSerializer();
xmlSerializer.Warmup(typesToWarmup);
Â 
var binarySerializer = SerializationFactory.GetBinarySerializer();
binarySerializer.Warmup(typesToWarmup);
```

### Warming up automatically

This code will warm up all types implementing the *ModelBaseÂ *class:

```
var xmlSerializer = SerializationFactory.GetXmlSerializer();
xmlSerializer.Warmup();
Â 
var binarySerializer = SerializationFactory.GetBinarySerializer();
binarySerialzier.Warmup();
```

{{% notice warning %}}
Note that warming up for all types **might** take a serious amount of time and **might** increase the memory footprint of your application depending on the number of models
{{% /notice %}}

### Warming up using multiple threads

By default, Catel will optimize the initialization and dispatch them to different threads. Using extensive testing, the Catel team discovered that approximately 1000 types / thread is the ideal load balancing (otherwise the spawning of the threads is more expensive than it handling it on the same thread). If this behavior needs to be customized, simply provide the number of types per thread. IfÂ *-1* is specified, all types will be warmed up in a single thread.

The code example below shows how to initialize all types deriving from *ModelBase* on a single thread:

```
var xmlSerializer = SerializationFactory.GetXmlSerializer();
xmlSerialzier.Warmup(null, -1);
```

## Backwards compatibility for binary serialization

This example shows how an â€œoldâ€ (standard .NET) data class that uses custom binary serialization can easily be converted to aÂ *ModelBase*Â to use theÂ *ModelBase*Â even for all your existing classes.

Declare a newÂ *ModelBase*Â class (remember theÂ *â€˜dataobjectâ€™*Â code snippet). If the new class is in a new assembly, or has a new name or namespace, use theÂ *RedirectType*Â attribute to let theÂ *ModelBase*Â know that when it finds the old type, it shouldÂ deserialize that type into the new type.

Then, by default, theÂ *ModelBase*Â class will try to deserialize the old object. If it fails to do so, it will fall back on the default values provided by the property declarations. However, it is also possible to override theÂ *GetDataFromSerializationInfo*Â method:

```
/// <summary>
/// Retrieves the actual data from the serialization info.
/// </summary>
/// <remarks>
/// This method should only be implemented if backwards compatibility should be implemented for
/// a class that did not previously implement the ModelBase class.
/// </remarks>
protected override void GetDataFromSerializationInfo(SerializationInfo info)
{
    // Check if deserialization succeeded
    if (DeserializationSucceeded)
    {
        return;
    }

    // Deserialization did not succeed for any reason, so retrieve the values manually
    // Luckily there is a helper class (SerializationHelper) 
    // that eases the deserialization of "old" style objects
    FirstName = SerializationHelper.GetString(info, "FirstName", FirstNameProperty.GetDefaultValue());
    LastName = SerializationHelper.GetString(info, "LastName", LastNameProperty.GetDefaultValue());
}
```

