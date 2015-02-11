﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.xml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.Net;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Logging;
    using Runtime.Serialization;

    public partial class ModelBase
    {
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            // As requested by the documentation, we return null
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            var serializer = SerializationFactory.GetXmlSerializer();
            var contextInfo = new XmlSerializationContextInfo(reader, this);

            serializer.Deserialize(this, contextInfo);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var type = GetType();

            var element = new XElement(type.Name);
            var serializer = SerializationFactory.GetXmlSerializer();
            serializer.Serialize(this, new XmlSerializationContextInfo(element, this));

            // The serializer gives us the full element, but we only need the actual content. According to
            // http://stackoverflow.com/questions/3793/best-way-to-get-innerxml-of-an-xelement, this method is the fastest:
            var reader = element.CreateReader();
            reader.MoveToContent();
            var elementContent = reader.ReadInnerXml();

            writer.WriteRaw(elementContent);
        }
    }
}