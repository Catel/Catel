// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System.IO;
    using System.Xml.Linq;
    using Data;

    /// <summary>
    /// The xml serializer to serialize the <see cref="ModelBase"/> and derived classes.
    /// </summary>
    public class XmlSerializer : SerializerBase<XElement>, IXmlSerializer
    {
        protected override void SerializeProperty(ISerializationContext<XElement> context, PropertyValue propertyValue)
        {
            var element = context.Context;

            var childElement = XmlHelper.ConvertToXml(propertyValue.Name, propertyValue.PropertyData.Type, propertyValue.Value);
            element.Add(childElement);
        }

        protected override object DeserializeProperty(ISerializationContext<XElement> context, PropertyValue propertyValue)
        {
            var element = context.Context;

            return XmlHelper.ConvertToObject(element, propertyValue.PropertyData.Type, propertyValue.PropertyData.GetDefaultValue);
        }

        protected override ISerializationContext<XElement> GetContext(ModelBase model, Stream stream)
        {
            var document = XDocument.Load(stream);
            return new SerializationContext<XElement>(model, document.Root);
        }
    }
}