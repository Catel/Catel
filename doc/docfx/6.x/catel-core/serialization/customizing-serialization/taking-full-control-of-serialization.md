---
title: "Taking full control of serialization" 
---
It's possible that full control is needed (either for performance or custom serialization formats). To ensure the best performance possible, this is implemented with a special interface for each serialization engine. If one of the engines detects such a special interface, it will skip all the plumbing and directly call the interface with the right context.

## Binary serialization

Custom binary serialization is not (yet) supported

## Xml serialization

To gain the best performance possible using the xml serializer, a model should implement the *ICustomXmlSerializable* interface.

```
public class CustomXmlSerializationModel : ModelBase, ICustomXmlSerializable
{
    public string FirstName
    {
        get { return GetValue<string>(FirstNameProperty); }
        set { SetValue(FirstNameProperty, value); }
    }

    public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);

    void ICustomXmlSerializable.Serialize(XElement xmlElement)
    {
        xmlElement.Add(new XElement("FirstName")
        {
            Value = FirstName
        });
    }

    void ICustomXmlSerializable.Deserialize(XElement xmlElement)
    {
        FirstName = xmlElement.Element("FirstName").Value;
    }
}
```

## Json serialization

To gain the best performance possible using the json serializer, a model should implement the *ICustomJsonSerializable* interface.

```
public class CustomJsonSerializationModel : ModelBase, ICustomJsonSerializable
{
Â Â Â  public string FirstName
Â Â Â  {
Â Â Â Â Â Â Â  get { return GetValue<string>(FirstNameProperty); }
Â Â Â Â Â Â Â  set { SetValue(FirstNameProperty, value); }
Â Â Â  }

Â Â Â  public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);

Â Â Â  void ICustomJsonSerializable.Serialize(JsonWriter jsonWriter)
Â Â Â  {
Â Â Â Â Â Â Â  jsonWriter.WriteStartObject();
Â Â Â Â Â Â Â  jsonWriter.WritePropertyName("FirstName");
Â Â Â Â Â Â Â  jsonWriter.WriteValue(FirstName);
Â Â Â Â Â Â Â  jsonWriter.WriteEndObject();
Â Â Â  }

Â Â Â  void ICustomJsonSerializable.Deserialize(JsonReader jsonReader)
Â Â Â  {
Â Â Â Â Â Â Â  // Note: this is probably not the fastest way to deserialize, but it's used to show the possibilities of the engine
Â Â Â Â Â Â Â  var jsonObject = JObject.Load(jsonReader);
Â Â Â Â Â Â Â  var jsonProperties = jsonObject.Properties().ToDictionary(x => x.Name, x => x);
Â Â Â Â Â Â Â  FirstName = (string)jsonProperties["FirstName"].Value;
Â Â Â  }
}
```

