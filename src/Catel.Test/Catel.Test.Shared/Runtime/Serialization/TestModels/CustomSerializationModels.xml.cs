
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomSerializationModels.xml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization.TestModels
{
    using Catel.Data;
    using System.Xml.Linq;
    using Catel.Runtime.Serialization.Xml;

    public class CustomXmlSerializationModel : CustomSerializationModelBase, ICustomXmlSerializable
    {
        void ICustomXmlSerializable.Serialize(XElement xmlElement)
        {
            xmlElement.Add(new XElement("FirstName")
            {
                Value = FirstName
            });

            IsCustomSerialized = true;
        }

        void ICustomXmlSerializable.Deserialize(XElement xmlElement)
        {
            FirstName = xmlElement.Element("FirstName").Value;

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

        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);


        public CustomXmlSerializationModel NestedModel
        {
            get { return GetValue<CustomXmlSerializationModel>(NestedModelProperty); }
            set { SetValue(NestedModelProperty, value); }
        }
        
        public static readonly PropertyData NestedModelProperty = RegisterProperty("NestedModel", typeof(CustomXmlSerializationModel), null);
    }
}