// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.xml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Logging;
    using Runtime.Serialization;

    public partial class ModelBase
    {
#if NET
        /// <summary>
        /// Gets XML schema for this class.
        /// <para />
        /// Implemented to support WCF serialization for all types deriving from this type.
        /// </summary>
        /// <param name="schemaSet">The schema set.</param>
        /// <returns>System.Xml.XmlQualifiedName.</returns>
        public static XmlQualifiedName GetModelBaseXmlSchema(XmlSchemaSet schemaSet)
        {
            return XmlSchemaManager.GetXmlSchema(typeof(ModelBase), schemaSet);
        }
#endif

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

            var type = GetType();

            var settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            var newReader = XmlReader.Create(reader, settings);

            if (!string.Equals(type.Name, newReader.LocalName, StringComparison.OrdinalIgnoreCase))
            {
                if (!newReader.Read())
                {
                    return;
                }
            }

            bool isAtXmlRoot = string.IsNullOrEmpty(newReader.LocalName) || string.Equals(newReader.LocalName, "xml", StringComparison.OrdinalIgnoreCase);
            if (isAtXmlRoot)
            {
                newReader.MoveToContent();
            }

            // Read attributes
            if (newReader.MoveToFirstAttribute())
            {
                do
                {
                    if (PropertyDataManager.IsXmlAttributeNameMappedToProperty(type, newReader.LocalName))
                    {
                        var propertyName = PropertyDataManager.MapXmlAttributeNameToPropertyName(type, reader.LocalName);
                        ReadValueFromXmlNode(newReader, propertyName);
                    }
                } while (newReader.MoveToNextAttribute());
            }

            // This might be the node itself or a wrapping node (in case of web services), 
            // so check if that is true and step into child element
            if (string.IsNullOrEmpty(newReader.LocalName) || !PropertyDataManager.IsXmlElementNameMappedToProperty(type, newReader.LocalName))
            {
                newReader.Read();
                newReader.MoveToElement();
            }

            while (newReader.NodeType != XmlNodeType.EndElement)
            {
                if (string.IsNullOrEmpty(reader.LocalName))
                {
                    Log.Debug("reader.LocalName is null or empty, trying to skip current node");
                    reader.Skip();

                    if (string.IsNullOrEmpty(reader.LocalName))
                    {
                        Log.Warning("reader.LocalName is null or empty, cannot read empty xml tag");
                        continue;
                    }
                }

                if (PropertyDataManager.IsXmlElementNameMappedToProperty(type, reader.LocalName))
                {
                    var propertyName = PropertyDataManager.MapXmlElementNameToPropertyName(type, reader.LocalName);
                    ReadValueFromXmlNode(newReader, propertyName);
                }
                else
                {
                    newReader.Skip();
                }

                while (newReader.NodeType == XmlNodeType.Whitespace)
                {
                    newReader.Skip();
                }
            }

            FinishDeserialization();
        }

        /// <summary>
        /// Reads the value from the XML node.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="reader"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <remarks>This method does not check whether the property exists. This is the responsibility of the caller.</remarks>
        private void ReadValueFromXmlNode(XmlReader reader, string propertyName)
        {
            Argument.IsNotNull("reader", reader);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var propertyData = PropertyDataManager.GetPropertyData(GetType(), propertyName);

            object value = null;

            switch (reader.NodeType)
            {
                case XmlNodeType.Attribute:
                    value = GetObjectFromXmlAttribute(reader, propertyData);
                    break;

                case XmlNodeType.Element:
                    value = GetObjectFromXmlElement(reader, propertyName);
                    break;

                default:
                    string error = string.Format("Xml node type '{0}' with local name '{1}' is not supported", reader.NodeType, ObjectToStringHelper.ToString(reader.LocalName));
                    Log.Error(error);
                    throw new NotSupportedException(error);
            }

            if (value != null)
            {
                SetValue(propertyData, value, false, false);
            }
        }

        /// <summary>
        /// Gets the object from XML attribute.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyData">The property data.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlAttribute(XmlReader reader, PropertyData propertyData)
        {
            reader.ReadAttributeValue();
            var value = reader.Value;

            try
            {
                return StringToObjectHelper.ToRightType(propertyData.Type, value);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to convert '{0}' to type '{1}', returning null", value, propertyData.Type.FullName);
                return null;
            }
        }

        /// <summary>
        /// Gets the object from XML element.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyName">Name of the property as known in the class.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlElement(XmlReader reader, string propertyName)
        {
            object value = null;
            string xmlName = reader.LocalName;
            var type = GetType();

            if (IsPropertyRegistered(propertyName))
            {
                try
                {
                    var propertyData = GetPropertyData(propertyName);
                    var propertyTypeToDeserialize = propertyData.Type;

                    var serializer = SerializationHelper.GetDataContractSerializer(type, propertyTypeToDeserialize, xmlName);

                    string attributeValue = reader.GetAttribute("type", "http://catel.codeplex.com");
                    if (!string.IsNullOrEmpty(attributeValue))
                    {
                        Log.Debug("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'",
                            propertyData.Name, propertyData.Type.FullName, attributeValue);

                        var actualTypeToDeserialize = (from t in serializer.KnownTypes
                                                       where t.FullName == attributeValue
                                                       select t).FirstOrDefault();
                        if (actualTypeToDeserialize != null)
                        {
                            serializer = SerializationHelper.GetDataContractSerializer(type, actualTypeToDeserialize, xmlName);
                        }
                        else
                        {
                            Log.Warning("Could not find type '{0}', falling back to original type '{1}'", attributeValue, propertyData.Type.FullName);
                        }
                    }

                    value = serializer.ReadObject(reader, false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to deserialize property '{0}' stored as '{1}', using default value", propertyName, xmlName);
                }
            }

            return value;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var type = GetType();

            string ns1 = writer.LookupPrefix("http://catel.codeplex.com");
            if (ns1 == null)
            {
                writer.WriteAttributeString("xmlns", "ctl", null, "http://catel.codeplex.com");
            }

            // 1st, write all attributes
            WriteXmlAttributes(writer, type);

            // 2nd, write all elements
            WriteXmlElements(writer, type);
        }

        /// <summary>
        /// Writes the XML attributes to the xml writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="type">The type.</param>
        private void WriteXmlAttributes(XmlWriter writer, Type type)
        {
            IEnumerable<KeyValuePair<string, object>> propertiesAsAttributes;

            lock (_propertyValuesLock)
            {
                propertiesAsAttributes = (from propertyValue in _propertyBag.GetAllProperties()
                                          where PropertyDataManager.IsPropertyNameMappedToXmlAttribute(type, propertyValue.Key)
                                          select propertyValue);
            }

            foreach (var propertyAsAttribute in propertiesAsAttributes)
            {
                var attributeName = PropertyDataManager.MapPropertyNameToXmlAttributeName(type, propertyAsAttribute.Key);
                writer.WriteAttributeString(attributeName, propertyAsAttribute.Value.ToString());
            }
        }

        /// <summary>
        /// Writes the XML elements to the xml writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="type">The type.</param>
        private void WriteXmlElements(XmlWriter writer, Type type)
        {
            IEnumerable<KeyValuePair<string, object>> propertiesAsElements;

            lock (_propertyValuesLock)
            {
                propertiesAsElements = (from propertyValue in _propertyBag.GetAllProperties()
                                        where PropertyDataManager.IsPropertyNameMappedToXmlElement(type, propertyValue.Key)
                                        select propertyValue);
            }

            foreach (var propertyAsElement in propertiesAsElements)
            {
                var propertyInfo = GetPropertyInfo(propertyAsElement.Key);
                if (propertyInfo == null)
                {
                    continue;
                }

                if (propertyAsElement.Value == null)
                {
                    continue;
                }

                string serializationPropertyName = propertyAsElement.Key;
                serializationPropertyName = PropertyDataManager.MapPropertyNameToXmlElementName(type, serializationPropertyName);

                var propertyType = propertyInfo.PropertyType;
                var propertyTypeToSerialize = propertyAsElement.Value.GetType();

                var serializer = SerializationHelper.GetDataContractSerializer(GetType(), propertyTypeToSerialize, serializationPropertyName, propertyAsElement.Value);

                if (propertyType != propertyTypeToSerialize)
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but registered as '{2}', adding type info for deserialization",
                              propertyInfo.Name, propertyTypeToSerialize.FullName, propertyType.FullName);

                    serializer.WriteStartObject(writer, propertyAsElement.Value);

                    writer.WriteAttributeString("ctl", "type", null, propertyTypeToSerialize.FullName);

                    serializer.WriteObjectContent(writer, propertyAsElement.Value);

                    serializer.WriteEndObject(writer);
                }
                else
                {
                    serializer.WriteObject(writer, propertyAsElement.Value);
                }
            }
        }
    }
}