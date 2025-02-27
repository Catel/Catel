﻿namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class CustomJsonSerializationModel : CustomSerializationModelBase, ICustomJsonSerializable
    {
        public CustomJsonSerializationModel(ISerializer serializer) 
            : base(serializer)
        {
            
        }

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
        public CustomJsonSerializationModelWithNesting(ISerializer serializer)
            : base(serializer)
        {
            
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);


        public CustomJsonSerializationModel NestedModel
        {
            get { return GetValue<CustomJsonSerializationModel>(NestedModelProperty); }
            set { SetValue(NestedModelProperty, value); }
        }

        public static readonly IPropertyData NestedModelProperty = RegisterProperty<CustomJsonSerializationModel>("NestedModel");
    }

    public class CustomJsonSerializationModelWithEnum : ModelBase
    {
        public CustomJsonSerializationModelWithEnum(ISerializer serializer)
            : base(serializer)
        {
            
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);

        [SerializeEnumAsString]
        public CustomSerializationEnum EnumWithAttribute
        {
            get { return GetValue<CustomSerializationEnum>(EnumWithAttributeProperty); }
            set { SetValue(EnumWithAttributeProperty, value); }
        }

        public static readonly IPropertyData EnumWithAttributeProperty = RegisterProperty<CustomSerializationEnum>(nameof(EnumWithAttribute));



        public CustomSerializationEnum EnumWithoutAttribute
        {
            get { return GetValue<CustomSerializationEnum>(EnumWithoutAttributeProperty); }
            set { SetValue(EnumWithoutAttributeProperty, value); }
        }

        public static readonly IPropertyData EnumWithoutAttributeProperty = RegisterProperty<CustomSerializationEnum>(nameof(EnumWithoutAttribute));

    }

    public enum CustomSerializationEnum
    {
        Default = 0,
        FirstFalue = 1,
        SecondValue = 2,
        ThirdValue = 3
    }
}
