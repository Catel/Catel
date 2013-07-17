// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Catel.Data;
    using Catel.Logging;

    /// <summary>
    /// The xml serializer to serialize the <see cref="ModelBase"/> and derived classes.
    /// </summary>
    public class XmlSerializer : SerializerBase<XElement>, IXmlSerializer
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IDataContractSerializerFactory _dataContractSerializerFactory;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializer" /> class.
        /// </summary>
        /// <param name="dataContractSerializerFactory">The data contract serializer factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContractSerializerFactory"/> is <c>null</c>.</exception>
        public XmlSerializer(IDataContractSerializerFactory dataContractSerializerFactory)
        {
            Argument.IsNotNull(() => dataContractSerializerFactory);

            _dataContractSerializerFactory = dataContractSerializerFactory;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected override void SerializeProperty(ISerializationContext<XElement> context, PropertyValue propertyValue)
        {
            var modelType = context.ModelType;
            var element = context.Context;

            var propertyDataManager = PropertyDataManager.Default;
            if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, propertyValue.Name))
            {
                Log.Debug("Serializing property {0}.{1} as xml attribute", modelType.FullName, propertyValue.Name);

                WriteXmlAttribute(element, propertyValue, modelType);
            }

            if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, propertyValue.Name))
            {
                Log.Debug("Serializing property {0}.{1} as xml element", modelType.FullName, propertyValue.Name);

                WriteXmlElement(element, propertyValue, modelType);
            }
        }

        /// <summary>
        /// Deserializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The <see cref="SerializationObject"/> representing the deserialized value or result.</returns>
        protected override SerializationObject DeserializeProperty(ISerializationContext<XElement> context, PropertyValue propertyValue)
        {
            var modelType = context.ModelType;
            var element = context.Context;

            var propertyDataManager = PropertyDataManager.Default;
            if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, propertyValue.Name))
            {
                var mappedPropertyName = propertyDataManager.MapPropertyNameToXmlAttributeName(modelType, propertyValue.Name);

                Log.Debug("Deserializing property {0}.{1} as xml attribute '{2}'", modelType.FullName, propertyValue.Name, mappedPropertyName);

                foreach (var childAttribute in element.Attributes())
                {
                    if (string.Equals(mappedPropertyName, childAttribute.Name.LocalName))
                    {
                        var value = GetObjectFromXmlAttribute(childAttribute, propertyValue.PropertyData);
                        return SerializationObject.SucceededToDeserialize(modelType, propertyValue.Name, value);
                    }
                }
            }

            if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, propertyValue.Name))
            {
                var mappedPropertyName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, propertyValue.Name);

                Log.Debug("Deserializing property {0}.{1} as xml element '{2}'", modelType.FullName, propertyValue.Name, mappedPropertyName);

                foreach (var childElement in element.Elements())
                {
                    if (string.Equals(mappedPropertyName, childElement.Name.LocalName))
                    {
                        var value = GetObjectFromXmlElement(childElement, propertyValue, modelType);
                        return SerializationObject.SucceededToDeserialize(modelType, propertyValue.Name, value);
                    }
                }
            }

            return SerializationObject.FailedToDeserialize(modelType, propertyValue.Name);
        }

        /// <summary>
        /// Appends the context to stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void AppendContextToStream(ISerializationContext<XElement> context, Stream stream)
        {
            var document = new XDocument(context.Context);
            document.Save(stream);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        protected override ISerializationContext<XElement> GetContext(ModelBase model, Stream stream, SerializationContextMode contextMode)
        {
            XDocument document = null;

            try
            {
                if (stream.Length != 0)
                {
                    document = XDocument.Load(stream);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load document from stream, falling back to empty document");
            }

            if (document == null)
            {
                var rootName = (model != null) ? model.GetType().Name : "root";
                document = new XDocument(new XElement(rootName));
            }

            return new SerializationContext<XElement>(model, document.Root, contextMode);
        }

        /// <summary>
        /// Gets the object from XML attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="propertyData">The property data.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlAttribute(XAttribute attribute, PropertyData propertyData)
        {
            var value = attribute.Value;

            try
            {
                return StringToObjectHelper.ToRightType(propertyData.Type, value);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to convert '{0}' to type '{1}', returning default value", value, propertyData.Type.FullName);

                return propertyData.GetDefaultValue();
            }
        }

        /// <summary>
        /// Gets the object from XML element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlElement(XElement element, PropertyValue propertyValue, Type modelType)
        {
            object value = null;
            string xmlName = element.Name.LocalName;

            try
            {
                var propertyTypeToDeserialize = propertyValue.PropertyData.Type;

                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToDeserialize, xmlName);

                // TODO: check for null attribute?
                var attribute = element.Attribute("type"); // .GetAttribute("type", "http://catel.codeplex.com");
                var attributeValue = (attribute != null) ? attribute.Value : null;
                if (!string.IsNullOrEmpty(attributeValue))
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'", 
                              propertyValue.Name, propertyValue.PropertyData.Type.FullName, attributeValue);

                    var actualTypeToDeserialize = (from t in serializer.KnownTypes
                                                   where t.FullName == attributeValue
                                                   select t).FirstOrDefault();
                    if (actualTypeToDeserialize != null)
                    {
                        serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, actualTypeToDeserialize, xmlName);
                    }
                    else
                    {
                        Log.Warning("Could not find type '{0}', falling back to original type '{1}'", attributeValue, propertyValue.PropertyData.Type.FullName);
                    }
                }

                using (var xmlReader = element.CreateReader())
                {
                    value = serializer.ReadObject(xmlReader, false);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to deserialize property '{0}' stored as '{1}', using default value", propertyValue.Name, xmlName);

                value = propertyValue.PropertyData.GetDefaultValue();
            }

            return value;
        }

        /// <summary>
        /// Writes the XML attributes to the xml writer.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="modelType">Type of the model.</param>
        private void WriteXmlAttribute(XElement element, PropertyValue propertyValue, Type modelType)
        {
            var propertyDataManager = PropertyDataManager.Default;

            var attributeName = propertyDataManager.MapPropertyNameToXmlAttributeName(modelType, propertyValue.Name);
            var attributeValue = ObjectToStringHelper.ToString(propertyValue.Value);

            var attribute = new XAttribute(attributeName, attributeValue);
            element.Add(attribute);
        }

        /// <summary>
        /// Writes the XML elements to the xml writer.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="modelType">Type of the model.</param>
        private void WriteXmlElement(XElement element, PropertyValue propertyValue, Type modelType)
        {
            // TODO: Should we handle null differently? Add an attribute?
            if (propertyValue.Value == null)
            {
                return;
            }

            var propertyDataManager = PropertyDataManager.Default;
            var elementName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, propertyValue.Name);

            var propertyType = propertyValue.PropertyData.Type;
            var propertyTypeToSerialize = propertyValue.Value.GetType();

            var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToSerialize, elementName, propertyValue.Value);

            var stringBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = true;
            using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                if (propertyType != propertyTypeToSerialize)
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but registered as '{2}', adding type info for deserialization", 
                              propertyValue.Name, propertyTypeToSerialize.FullName, propertyType.FullName);

                    serializer.WriteStartObject(xmlWriter, propertyValue.Value);

                    xmlWriter.WriteAttributeString("ctl", "type", null, propertyTypeToSerialize.FullName);

                    serializer.WriteObjectContent(xmlWriter, propertyValue.Value);

                    serializer.WriteEndObject(xmlWriter);
                }
                else
                {
                    serializer.WriteObject(xmlWriter, propertyValue.Value);
                }
            }

            string ns1 = element.GetPrefixOfNamespace("http://catel.codeplex.com");
            if (ns1 == null)
            {
                var document = element.Document;
                if (document != null)
                {
                    var documentRoot = document.Root;
                    if (documentRoot != null)
                    {
                        var catelNamespaceName = XNamespace.Xmlns + "ctl";
                        var catelNamespaceUrl = "http://catel.codeplex.com";
                        var catelNamespace = new XAttribute(catelNamespaceName, catelNamespaceUrl);

                        documentRoot.Add(catelNamespace);
                    }
                }
            }

            var childContent = stringBuilder.ToString();
            var childElement = XElement.Parse(childContent);
            element.Add(childElement);
        }
        #endregion
    }
}