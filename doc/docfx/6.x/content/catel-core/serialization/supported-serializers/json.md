---
title: "Json" 
description: ""
---
The `JsonSerializer` is implemented in a separate assembly because it uses Json.Net under the hood.

## Customizing configuration

The `JsonSerializationConfiguration` enables support for customizing the configuration used while (de)serializing. Below is an example

### Using Bson instead of Json

To use Bson instead of Json, use the `JsonSerializationConfiguration` class:

```
var configuration = new JsonSerializationConfiguration
{
    UseBson = true
};

jsonSerializer.Serialize(myObject, configuration);
```

### Specifying the culture to use during (de)serialization

To specify the culture to use during (de)serialization, set the `Culture` property on the configuration class:

```
var configuration = new SerializationConfiguration
{
    Culture = new CultureInfo("nl-NL")
};


jsonSerializer.Serialize(myObject, configuration);
```

## Preserve references (and support circular references)

By default the `JsonSerializer` supports circular references. It does so by adding additional property values to the json. Below is a json object with support for circular references:

```
{  
   "$graphid":1,
   "Name":"1",
   "CircularModel":{  
      "$graphid":2,
      "Name":"2",
      "CircularModel":{  
         "$graphrefid":1
      }
   }
}
```

or

```
{
    "$graphid": 1,
    "Collection1": [1,
    2,
    3,
    4,
    5],
    "$Collection1_$graphid": 2,
    "Collection2": [1,
    2,
    3,
    4,
    5],
    "$Collection2_$graphrefid": 2
}
```

To disable the support for reference preservation, use the code below:

```
var jsonSerializer = dependencyResolver.Resolve<IJsonSerializer>();
jsonSerializer.PreserveReferences = false;
```

## Support complex dynamic types

Catel can support complex dynamic types. For this to be supported in json, the objects need additional type information in order to restore the type info again during deserialization. Below is a json object with the type information stored inside the json:

```
{  
   "$typename":"Catel.Test.Data.IniFile",
   "FileName":"MyIniFile",
   "IniEntryCollection":[  
      {  
         "$typename":"Catel.Test.Data.IniEntry",
         "Group":"Group 0",
         "Key":"Key 0",
         "Value":"Value 0",
         "IniEntryType":0
      },
      {  
         "$typename":"Catel.Test.Data.IniEntry",
         "Group":"Group 1",
         "Key":"Key 1",
         "Value":"Value 1",
         "IniEntryType":1
      },
      {  
         "$typename":"Catel.Test.Data.IniEntry",
         "Group":"Group 2",
         "Key":"Key 2",
         "Value":"Value 2",
         "IniEntryType":0
      }
   ]
}
```

To disable the type information in json, use the code below:

```
var jsonSerializer = dependencyResolver.Resolve<IJsonSerializer>();
jsonSerializer.WriteTypeInfo = false;
```

