// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Catel.Caching;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Reflection;

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

        private readonly CacheStorage<Type, List<string>> _ignoredMembersCache = new CacheStorage<Type, List<string>>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializer" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="dataContractSerializerFactory">The data contract serializer factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContractSerializerFactory" /> is <c>null</c>.</exception>
        public XmlSerializer(ISerializationManager serializationManager, IDataContractSerializerFactory dataContractSerializerFactory)
            : base(serializationManager)
        {
            Argument.IsNotNull(() => dataContractSerializerFactory);

            _dataContractSerializerFactory = dataContractSerializerFactory;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected override void SerializeMember(ISerializationContext<XElement> context, MemberValue memberValue)
        {
            var modelType = context.ModelType;
            var element = context.Context;

            var propertyDataManager = PropertyDataManager.Default;
            if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, memberValue.Name))
            {
                var attributeName = propertyDataManager.MapPropertyNameToXmlAttributeName(modelType, memberValue.Name);

                Log.Debug("Serializing property {0}.{1} as xml attribute '{2}'", modelType.FullName, memberValue.Name, attributeName);

                WriteXmlAttribute(element, attributeName, memberValue);
            }
            else
            {
                var elementName = memberValue.Name;

                if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, memberValue.Name))
                {
                    elementName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, memberValue.Name);
                }

                Log.Debug("Serializing property {0}.{1} as xml element '{2}'", modelType.FullName, memberValue.Name, elementName);

                WriteXmlElement(element, elementName, memberValue, modelType);
            }
        }

        /// <summary>
        /// Deserializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The <see cref="SerializationObject"/> representing the deserialized value or result.</returns>
        protected override SerializationObject DeserializeMember(ISerializationContext<XElement> context, MemberValue memberValue)
        {
            var modelType = context.ModelType;
            var element = context.Context;

            try
            {
                var propertyDataManager = PropertyDataManager.Default;
                if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, memberValue.Name))
                {
                    var mappedPropertyName = propertyDataManager.MapPropertyNameToXmlAttributeName(modelType, memberValue.Name);

                    Log.Debug("Deserializing property {0}.{1} as xml attribute '{2}'", modelType.FullName, memberValue.Name, mappedPropertyName);

                    foreach (var childAttribute in element.Attributes())
                    {
                        if (string.Equals(mappedPropertyName, childAttribute.Name.LocalName))
                        {
                            var value = GetObjectFromXmlAttribute(childAttribute, memberValue);
                            return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                        }
                    }
                }
                else
                {
                    string elementName = memberValue.Name;

                    if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, memberValue.Name))
                    {
                        elementName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, memberValue.Name);
                    }

                    Log.Debug("Deserializing property {0}.{1} as xml element '{2}'", modelType.FullName, memberValue.Name, elementName);

                    foreach (var childElement in element.Elements())
                    {
                        if (string.Equals(elementName, childElement.Name.LocalName))
                        {
                            var value = GetObjectFromXmlElement(childElement, memberValue, modelType);
                            return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Swallow
            }

            return SerializationObject.FailedToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name);
        }

        /// <summary>
        /// Determines whether the specified member on the specified model should be ignored by the serialization engine.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">Name of the member.</param>
        /// <returns><c>true</c> if the member should be ignored, <c>false</c> otherwise.</returns>
        protected override bool ShouldIgnoreMember(ModelBase model, string propertyName)
        {
            var ignoredMembers = _ignoredMembersCache.GetFromCacheOrFetch(model.GetType(), () =>
            {
                var modelType = model.GetType();
                var ignoredProperties = new List<string>();

                var properties = modelType.GetPropertiesEx();
                foreach (var property in properties)
                {
                    if (AttributeHelper.IsDecoratedWithAttribute<XmlIgnoreAttribute>(property))
                    {
                        ignoredProperties.Add(property.Name);
                    }
                }

                return ignoredProperties;
            });

            if (ignoredMembers.Contains(propertyName))
            {
                return true;
            }

            return base.ShouldIgnoreMember(model, propertyName);
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
        /// <remarks>
        /// Note that this method can cause exceptions. The caller will handle them.
        /// </remarks>
        /// <param name="attribute">The attribute.</param>
        /// <param name="memberValue">The property data.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlAttribute(XAttribute attribute, MemberValue memberValue)
        {
            var value = attribute.Value;

            return StringToObjectHelper.ToRightType(memberValue.Type, value);
        }

        /// <summary>
        /// Gets the object from XML element.
        /// </summary>
        /// <remarks>
        /// Note that this method can cause exceptions. The caller will handle them.
        /// </remarks>
        /// <param name="element">The element.</param>
        /// <param name="memberValue">The member value.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlElement(XElement element, MemberValue memberValue, Type modelType)
        {
            object value = null;
            string xmlName = element.Name.LocalName;

            var propertyTypeToDeserialize = memberValue.Type;

            var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToDeserialize, xmlName);

            // TODO: check for null attribute?
            var attribute = element.Attribute("type"); // .GetAttribute("type", "http://catel.codeplex.com");
            var attributeValue = (attribute != null) ? attribute.Value : null;
            if (!string.IsNullOrEmpty(attributeValue))
            {
                Log.Debug("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'",
                          memberValue.Name, memberValue.Type.FullName, attributeValue);

                var actualTypeToDeserialize = (from t in serializer.KnownTypes
                                               where t.FullName == attributeValue
                                               select t).FirstOrDefault();
                if (actualTypeToDeserialize != null)
                {
                    serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, actualTypeToDeserialize, xmlName);
                }
                else
                {
                    Log.Warning("Could not find type '{0}', falling back to original type '{1}'", attributeValue, memberValue.Type.FullName);
                }
            }

            using (var xmlReader = element.CreateReader())
            {
                value = serializer.ReadObject(xmlReader, false);
            }

            return value;
        }

        /// <summary>
        /// Writes the XML attribute to the xml element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="memberValue">The member value.</param>
        private void WriteXmlAttribute(XElement element, string attributeName, MemberValue memberValue)
        {
            var attributeValue = ObjectToStringHelper.ToString(memberValue.Value);

            var attribute = new XAttribute(attributeName, attributeValue);
            element.Add(attribute);
        }

        /// <summary>
        /// Writes the XML element to the xml element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="memberValue">The member value.</param>
        /// <param name="modelType">Type of the model.</param>
        private void WriteXmlElement(XElement element, string elementName, MemberValue memberValue, Type modelType)
        {
            // TODO: Should we handle null differently? Add an attribute?
            if (memberValue.Value == null)
            {
                return;
            }

            var memberType = memberValue.Type;
            var memberTypeToSerialize = memberValue.Value.GetType();

            var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, memberTypeToSerialize, elementName, memberValue.Value);

            var stringBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = true;
            using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                if (memberType != memberTypeToSerialize)
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but registered as '{2}', adding type info for deserialization",
                        memberValue.Name, memberTypeToSerialize.FullName, memberType.FullName);

                    serializer.WriteStartObject(xmlWriter, memberValue.Value);

                    xmlWriter.WriteAttributeString("ctl", "type", null, memberTypeToSerialize.FullName);

                    serializer.WriteObjectContent(xmlWriter, memberValue.Value);

                    serializer.WriteEndObject(xmlWriter);
                }
                else
                {
                    serializer.WriteObject(xmlWriter, memberValue.Value);
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