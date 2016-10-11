// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomSerializationModels.json.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization.TestModels
{
    using System.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class CustomJsonSerializationModel : CustomSerializationModelBase, ICustomJsonSerializable
    {
        void ICustomJsonSerializable.Serialize(JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("FirstName");
            jsonWriter.WriteValue(FirstName);
            jsonWriter.WriteEndObject();

            IsCustomSerialized = true;
        }

        void ICustomJsonSerializable.Deserialize(JsonReader jsonReader)
        {
            var jsonObject = JObject.Load(jsonReader);
            var jsonProperties = jsonObject.Properties().ToDictionary(x => x.Name, x => x);

            FirstName = (string)jsonProperties["FirstName"].Value;

            IsCustomDeserialized = true;
        }
    }

    public class CustomJsonSerializationModelWithNesting : ModelBase
    {
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);


        public CustomJsonSerializationModel NestedModel
        {
            get { return GetValue<CustomJsonSerializationModel>(NestedModelProperty); }
            set { SetValue(NestedModelProperty, value); }
        }
        
        public static readonly PropertyData NestedModelProperty = RegisterProperty("NestedModel", typeof(CustomJsonSerializationModel), null);
    }
}