namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System.Xml;
    using Catel.Data;
    using Catel.Runtime.Serialization.Xml;

    public class CustomXmlSerializationModel : CustomSerializationModelBase, ICustomXmlSerializable
    {
        void ICustomXmlSerializable.Serialize(XmlWriter xmlWriter)
        {
            xmlWriter.WriteElementString("FirstName", FirstName);

            IsCustomSerialized = true;
        }

        void ICustomXmlSerializable.Deserialize(XmlReader xmlReader)
        {
            xmlReader.MoveToContent();
            xmlReader.Read();
            xmlReader.MoveToContent();

            if (xmlReader.LocalName == "FirstName")
            {
                FirstName = xmlReader.ReadElementContentAsString();
            }

            IsCustomDeserialized = true;
        }
    }

    public class CustomXmlSerializationModelWithNesting : ModelBase
    {
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);


        public CustomXmlSerializationModel NestedModel
        {
            get { return GetValue<CustomXmlSerializationModel>(NestedModelProperty); }
            set { SetValue(NestedModelProperty, value); }
        }

        public static readonly IPropertyData NestedModelProperty = RegisterProperty<CustomXmlSerializationModel>("NestedModel");
    }
}
