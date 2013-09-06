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
        private const string GraphId = "graphid";
        private const string GraphRefId = "graphrefid";

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
        /// Called before the serializer starts serializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeSerialization(ISerializationContext<XElement> context)
        {
            base.BeforeSerialization(context);

            var referenceManager = context.ReferenceManager;
            if (referenceManager.Count == 0)
            {
                Log.Debug("Reference manager contains no objects yet, adding initial reference which is the first model in the graph");

                referenceManager.GetInfo(context.Model);
            }
        }

        /// <summary>
        /// Called before the serializer starts deserializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeDeserialization(ISerializationContext<XElement> context)
        {
            base.BeforeDeserialization(context);

            var element = context.Context;

            var graphIdAttribute = element.Attribute(GraphId);
            if (graphIdAttribute != null)
            {
                var graphId = int.Parse(graphIdAttribute.Value);

                var referenceManager = context.ReferenceManager;
                var referenceInfo = referenceManager.GetInfoById(graphId);
                if (referenceInfo != null)
                {
                    Log.Warning("Trying to register custom object in graph with graph id '{0}', but it seems it is already registered", graphId);
                    return;
                }

                referenceManager.RegisterManually(graphId, context.Model);
            }
        }

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

                WriteXmlElement(context, element, elementName, memberValue, modelType);
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
                            var value = GetObjectFromXmlElement(context, childElement, memberValue, modelType);
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

            bool isNewDocument = document == null;
            if (isNewDocument)
            {
                var rootName = (model != null) ? model.GetType().Name : "root";
                document = new XDocument(new XElement(rootName));
            }

            var context = new SerializationContext<XElement>(model, document.Root, contextMode);

            if (isNewDocument)
            {
                AddReferenceId(context, document.Root, model);
            }

            return context;
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
        /// <param name="context">The context.</param>
        /// <param name="element">The element.</param>
        /// <param name="memberValue">The member value.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>Object or <c>null</c>.</returns>
        /// <remarks>Note that this method can cause exceptions. The caller will handle them.</remarks>
        private object GetObjectFromXmlElement(ISerializationContext context, XElement element, MemberValue memberValue, Type modelType)
        {
            object value = null;
            string xmlName = element.Name.LocalName;

            var propertyTypeToDeserialize = memberValue.Type;

            var isNullAttribute = element.Attribute("IsNull");
            var isNull = (isNullAttribute != null) ? StringToObjectHelper.ToBool(isNullAttribute.Value) : false;
            if (isNull)
            {
                return null;
            }

            var graphRefIdAttribute = element.Attribute(GraphRefId);
            if (graphRefIdAttribute != null)
            {
                var graphId = int.Parse(graphRefIdAttribute.Value);

                var referenceManager = context.ReferenceManager;
                var referenceInfo = referenceManager.GetInfoById(graphId);
                if (referenceInfo == null)
                {
                    Log.Error("Expected to find graph object with id '{0}' in ReferenceManager, but it was not found. Defaulting value for member '{1}' to null", graphId, element.Name);
                    return null;
                }

                return referenceInfo.Instance;
            }

            var typeAttribute = element.Attribute("type"); // .GetAttribute("type", "http://catel.codeplex.com");
            var attributeValue = (typeAttribute != null) ? typeAttribute.Value : null;
            if (!string.IsNullOrEmpty(attributeValue))
            {
                var typeToDeserialize = TypeCache.GetTypeWithoutAssembly(attributeValue);
                if (typeToDeserialize != null)
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'",
                        memberValue.Name, memberValue.Type.FullName, attributeValue);

                    propertyTypeToDeserialize = typeToDeserialize;
                }
            }

            var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToDeserialize, xmlName);

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
        /// <param name="context">The context.</param>
        /// <param name="element">The element.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="memberValue">The member value.</param>
        /// <param name="modelType">Type of the model.</param>
        private void WriteXmlElement(ISerializationContext context, XElement element, string elementName, MemberValue memberValue, Type modelType)
        {
            var stringBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings();

            var namespacePrefix = GetNamespacePrefix();

            xmlWriterSettings.OmitXmlDeclaration = true;
            using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                var memberTypeToSerialize = memberValue.Value != null ? memberValue.Value.GetType() : typeof(object);
                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, memberTypeToSerialize, elementName, memberValue.Value);

                if (memberValue.Value == null)
                {
                    xmlWriter.WriteStartElement(elementName);
                    xmlWriter.WriteAttributeString(namespacePrefix, "IsNull", null, "true");
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    ReferenceInfo referenceInfo = null;
                    bool serializeElement = true;
                    var isClassType = TypeHelper.IsClassType(memberTypeToSerialize);
                    if (isClassType)
                    {
                        var referenceManager = context.ReferenceManager;
                        referenceInfo = referenceManager.GetInfo(memberValue.Value);

                        if (!referenceInfo.IsFirstUsage)
                        {
                            Log.Debug("Existing reference detected for element type '{0}' with id '{1}', only storing id", memberTypeToSerialize.GetSafeFullName(), referenceInfo.Id);

                            serializer.WriteStartObject(xmlWriter, memberValue.Value);

                            xmlWriter.WriteAttributeString(namespacePrefix, GraphRefId, null, referenceInfo.Id.ToString());

                            serializer.WriteEndObject(xmlWriter);

                            serializeElement = false;
                        }
                    }

                    if (serializeElement)
                    {
                        serializer.WriteStartObject(xmlWriter, memberValue.Value);

                        if (referenceInfo != null)
                        {
                            xmlWriter.WriteAttributeString(namespacePrefix, GraphId, null, referenceInfo.Id.ToString());
                        }

                        xmlWriter.WriteAttributeString(namespacePrefix, "type", null, memberTypeToSerialize.FullName);

                        serializer.WriteObjectContent(xmlWriter, memberValue.Value);

                        serializer.WriteEndObject(xmlWriter);
                    }
                }
            }

            EnsureNamespaceInXmlDocument(element);

            var childContent = stringBuilder.ToString();
            var childElement = XElement.Parse(childContent);
            element.Add(childElement);
        }

        /// <summary>
        /// Adds the reference unique identifier as attribute.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="element">The element.</param>
        /// <param name="model">The model.</param>
        private void AddReferenceId(ISerializationContext context, XElement element, IModel model)
        {
            var referenceManager = context.ReferenceManager;
            var referenceInfo = referenceManager.GetInfo(model);

            element.Add(new XAttribute(GraphId, referenceInfo.Id));
        }

        /// <summary>
        /// Ensures the catel namespace in the xml document.
        /// </summary>
        /// <param name="element">The element.</param>
        private void EnsureNamespaceInXmlDocument(XElement element)
        {
            var catelNamespaceUrl = GetNamespaceUrl();
            string ns1 = element.GetPrefixOfNamespace(catelNamespaceUrl);
            if (ns1 == null)
            {
                var document = element.Document;
                if (document != null)
                {
                    var documentRoot = document.Root;
                    if (documentRoot != null)
                    {
                        var catelNamespaceName = XNamespace.Xmlns + GetNamespacePrefix();
                        var catelNamespace = new XAttribute(catelNamespaceName, catelNamespaceUrl);

                        documentRoot.Add(catelNamespace);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the namespace prefix.
        /// </summary>
        /// <returns>The namespace prefix..</returns>
        protected virtual string GetNamespacePrefix()
        {
            return "ctl";
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <returns>The namespace.</returns>
        protected virtual string GetNamespaceUrl()
        {
            return "http://catel.codeplex.com";
        }
        #endregion
    }
}