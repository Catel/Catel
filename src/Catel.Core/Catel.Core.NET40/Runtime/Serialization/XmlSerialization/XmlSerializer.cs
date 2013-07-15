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
        /// <summary>
        /// Serializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected override void SerializeProperty(ISerializationContext<XElement> context, PropertyValue propertyValue)
        {
            var element = context.Context;

            var childElement = XmlHelper.ConvertToXml(propertyValue.Name, propertyValue.PropertyData.Type, propertyValue.Value);
            element.Add(childElement);
        }

        /// <summary>
        /// Deserializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The deserialized property value.</returns>
        protected override object DeserializeProperty(ISerializationContext<XElement> context, PropertyValue propertyValue)
        {
            var element = context.Context;

            return XmlHelper.ConvertToObject(element, propertyValue.PropertyData.Type, propertyValue.PropertyData.GetDefaultValue);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The serialization context.</returns>
        protected override ISerializationContext<XElement> GetContext(ModelBase model, Stream stream)
        {
            var document = XDocument.Load(stream);
            return new SerializationContext<XElement>(model, document.Root);
        }
    }
}