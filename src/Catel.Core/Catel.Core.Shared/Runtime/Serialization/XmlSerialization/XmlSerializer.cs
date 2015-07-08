// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Catel.Caching;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// The xml serializer.
    /// </summary>
    public class XmlSerializer : SerializerBase<XmlSerializationContextInfo>, IXmlSerializer
    {
        #region Enums
        private enum MemberType
        {
            Field,

            Property
        }
        #endregion

        #region Constants
        private const string GraphId = "graphid";
        private const string GraphRefId = "graphrefid";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly CacheStorage<Type, HashSet<string>> _ignoredMembersCache = new CacheStorage<Type, HashSet<string>>();
        private readonly CacheStorage<Type, string> _rootNameCache = new CacheStorage<Type, string>();
        private readonly IDataContractSerializerFactory _dataContractSerializerFactory;
        private readonly IXmlNamespaceManager _xmlNamespaceManager;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializer" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="dataContractSerializerFactory">The data contract serializer factory.</param>
        /// <param name="xmlNamespaceManager">The XML namespace manager.</param>
        /// <param name="typeFactory">The type factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContractSerializerFactory" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="xmlNamespaceManager" /> is <c>null</c>.</exception>
        public XmlSerializer(ISerializationManager serializationManager, IDataContractSerializerFactory dataContractSerializerFactory,
            IXmlNamespaceManager xmlNamespaceManager, ITypeFactory typeFactory)
            : base(serializationManager, typeFactory)
        {
            Argument.IsNotNull("dataContractSerializerFactory", dataContractSerializerFactory);
            Argument.IsNotNull("xmlNamespaceManager", xmlNamespaceManager);

            _dataContractSerializerFactory = dataContractSerializerFactory;
            _xmlNamespaceManager = xmlNamespaceManager;

            OptimalizationMode = XmlSerializerOptimalizationMode.Performance;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the optimalization mode.
        /// <para />
        /// The default value is <see cref="XmlSerializerOptimalizationMode.Performance"/>.
        /// </summary>
        /// <value>The optimalization mode.</value>
        public XmlSerializerOptimalizationMode OptimalizationMode { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Warms up the specified type.
        /// </summary>
        /// <param name="type">The type to warmup.</param>
        protected override void Warmup(Type type)
        {
            if (type == null)
            {
                return;
            }

            var fieldsToSerialize = SerializationManager.GetFieldsToSerialize(type);
            foreach (var fieldToSerialize in fieldsToSerialize)
            {
                WarmupMember(type, fieldToSerialize, MemberType.Field);
            }

            var propertiesToSerialize = SerializationManager.GetPropertiesToSerialize(type);
            foreach (var propertyToSerialize in propertiesToSerialize)
            {
                WarmupMember(type, propertyToSerialize, MemberType.Property);
            }
        }

        private void WarmupMember(Type type, string memberName, MemberType memberType)
        {
            var propertyDataManager = PropertyDataManager.Default;

            try
            {
                Type memberRepresentedType = null;
                switch (memberType)
                {
                    case MemberType.Field:
                        var fieldInfo = type.GetFieldEx(memberName);
                        if (fieldInfo == null)
                        {
                            Log.Warning("Failed to retrieve the field info of '{0}.{1}' during warmup", type.GetSafeFullName(), memberName);
                            return;
                        }

                        memberRepresentedType = fieldInfo.FieldType;
                        break;

                    case MemberType.Property:
                        var propertyInfo = type.GetPropertyEx(memberName);
                        if (propertyInfo == null)
                        {
                            Log.Warning("Failed to retrieve the property info of '{0}.{1}' during warmup", type.GetSafeFullName(), memberName);
                            return;
                        }

                        memberRepresentedType = propertyInfo.PropertyType;
                        break;
                }

                string xmlName = memberName;
                if (propertyDataManager.IsPropertyNameMappedToXmlElement(type, memberName))
                {
                    xmlName = propertyDataManager.MapPropertyNameToXmlElementName(type, memberName);
                }

                _dataContractSerializerFactory.GetDataContractSerializer(type, memberRepresentedType, xmlName, null, null);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to warmup member '{0}.{1}'. This member might cause problems during serialization", type.GetSafeFullName(), memberName);
            }
        }

        /// <summary>
        /// Called before the serializer starts serializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeSerialization(ISerializationContext<XmlSerializationContextInfo> context)
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
        protected override void BeforeDeserialization(ISerializationContext<XmlSerializationContextInfo> context)
        {
            base.BeforeDeserialization(context);

            var element = context.Context.Element;

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
        protected override void SerializeMember(ISerializationContext<XmlSerializationContextInfo> context, MemberValue memberValue)
        {
            var modelType = context.ModelType;
            var element = context.Context.Element;

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
        protected override SerializationObject DeserializeMember(ISerializationContext<XmlSerializationContextInfo> context, MemberValue memberValue)
        {
            var modelType = context.ModelType;
            var element = context.Context.Element;

            try
            {
                var propertyDataManager = PropertyDataManager.Default;
                if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, memberValue.Name))
                {
                    var mappedPropertyName = propertyDataManager.MapPropertyNameToXmlAttributeName(modelType, memberValue.Name);

                    //Log.Debug("Deserializing property {0}.{1} as xml attribute '{2}'", modelType.FullName, memberValue.Name, mappedPropertyName);

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

                    //Log.Debug("Deserializing property {0}.{1} as xml element '{2}'", modelType.FullName, memberValue.Name, elementName);

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
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to deserialize '{0}.{1}'", memberValue.ModelType.GetSafeFullName(), memberValue.Name);
            }

            return SerializationObject.FailedToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name);
        }

        /// <summary>
        /// Determines whether the specified member on the specified model should be ignored by the serialization engine.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">Name of the member.</param>
        /// <returns><c>true</c> if the member should be ignored, <c>false</c> otherwise.</returns>
        protected override bool ShouldIgnoreMember(object model, string propertyName)
        {
            var ignoredMembers = _ignoredMembersCache.GetFromCacheOrFetch(model.GetType(), () =>
            {
                var modelType = model.GetType();
                var ignoredProperties = new HashSet<string>();

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
        protected override void AppendContextToStream(ISerializationContext<XmlSerializationContextInfo> context, Stream stream)
        {
            var element = context.Context.Element;
            var document = new XDocument(element);

            OptimizeXDocument(document);

            document.Save(stream);
        }

        /// <summary>
        /// Optimizes the xml document.
        /// </summary>
        /// <param name="document">The document.</param>
        protected virtual void OptimizeXDocument(XDocument document)
        {
            if (OptimalizationMode == XmlSerializerOptimalizationMode.Performance)
            {
                return;
            }

            var rootNamespaces = (from attribute in document.Root.Attributes()
                                  where attribute.IsNamespaceDeclaration
                                  select attribute.Value).ToList();

            var obsoleteNamespaceMarkers = (from descendant in document.Root.Descendants()
                                            from attribute in descendant.Attributes()
                                            where attribute.IsNamespaceDeclaration && rootNamespaces.Contains(attribute.Value)
                                            select attribute).ToList();

            foreach (var obsoleteNamespaceMarker in obsoleteNamespaceMarkers)
            {
                obsoleteNamespaceMarker.Remove();
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        protected override ISerializationContext<XmlSerializationContextInfo> GetContext(object model, Stream stream, SerializationContextMode contextMode)
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
                var rootName = "root";
                if (model != null)
                {
                    var modelType = model.GetType();
                    rootName = _rootNameCache.GetFromCacheOrFetch(modelType, () =>
                    {
                        XmlRootAttribute xmlRootAttribute;
                        if (AttributeHelper.TryGetAttribute(modelType, out xmlRootAttribute))
                        {
                            return xmlRootAttribute.ElementName;
                        }

                        return rootName = model.GetType().Name;
                    });
                }
                document = new XDocument(new XElement(rootName));
            }

            var contextInfo = new XmlSerializationContextInfo(document.Root, model);
            var context = new SerializationContext<XmlSerializationContextInfo>(model, contextInfo, contextMode);

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
        private object GetObjectFromXmlElement(ISerializationContext<XmlSerializationContextInfo> context, XElement element, MemberValue memberValue, Type modelType)
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
                if (typeToDeserialize != null && propertyTypeToDeserialize != typeToDeserialize)
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'",
                        memberValue.Name, memberValue.Type.FullName, attributeValue);

                    propertyTypeToDeserialize = typeToDeserialize;
                }
                else
                {
                    Log.Warning("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'. Unfortunately the type cannot be found so the deserialization will probably fail.",
                        memberValue.Name, memberValue.Type.FullName, attributeValue);
                }
            }

            var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToDeserialize, xmlName, null, null);

            using (var xmlReader = element.CreateReader())
            {
                value = serializer.ReadObject(xmlReader, false);
            }

            // Fix for CTL-555
            var graphIdAttribute = element.Attribute(GraphId);
            if (graphIdAttribute != null)
            {
                var graphId = int.Parse(graphIdAttribute.Value);

                var referenceManager = context.ReferenceManager;
                referenceManager.RegisterManually(graphId, value);
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
        private void WriteXmlElement(ISerializationContext<XmlSerializationContextInfo> context, XElement element, string elementName, MemberValue memberValue, Type modelType)
        {
            var contextInfo = context.Context;
            var namespacePrefix = GetNamespacePrefix();
            var stringBuilder = new StringBuilder();
            var xmlWriterSettings = new XmlWriterSettings();
            XmlNamespace xmlNamespace = null;

            xmlWriterSettings.OmitXmlDeclaration = true;
            xmlWriterSettings.CheckCharacters = false;
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
            xmlWriterSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

#if XAMARIN
            var defaultNamespace = "http://www.w3.org/2000/xmlns/";
#endif

            using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                var memberTypeToSerialize = memberValue.Value != null ? memberValue.Value.GetType() : typeof(object);
                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, memberTypeToSerialize, elementName, null, memberValue.Value);

                if (memberValue.Value == null)
                {
                    xmlWriter.WriteStartElement(elementName);

#if XAMARIN
                    xmlWriter.WriteAttributeString("xmlns", namespacePrefix, defaultNamespace, "http://catel.codeplex.com"); 
#endif

                    xmlWriter.WriteAttributeString(namespacePrefix, "IsNull", null, "true");
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    ReferenceInfo referenceInfo = null;
                    bool serializeElement = true;
                    var isClassType = memberTypeToSerialize.IsClassType();
                    if (isClassType)
                    {
                        var referenceManager = context.ReferenceManager;
                        referenceInfo = referenceManager.GetInfo(memberValue.Value);

                        if (!referenceInfo.IsFirstUsage)
                        {
                            Log.Debug("Existing reference detected for element type '{0}' with id '{1}', only storing id", memberTypeToSerialize.GetSafeFullName(), referenceInfo.Id);

                            //serializer.WriteStartObject(xmlWriter, memberValue.Value);
                            xmlWriter.WriteStartElement(elementName);

                            xmlWriter.WriteAttributeString(namespacePrefix, GraphRefId, null, referenceInfo.Id.ToString());

                            //serializer.WriteEndObject(xmlWriter);
                            xmlWriter.WriteEndElement();

                            serializeElement = false;
                        }
                    }

                    if (serializeElement)
                    {
                        //var xmlSerializer = new System.Xml.Serialization.XmlSerializer(memberTypeToSerialize, namespacePrefix);
                        //xmlSerializer.Serialize(xmlWriter, memberValue.Value);

                        xmlWriter.WriteStartElement(elementName);

                        xmlNamespace = _xmlNamespaceManager.GetNamespace(memberTypeToSerialize, namespacePrefix);
                        if (xmlNamespace != null)
                        {
                            xmlWriter.WriteAttributeString("xmlns", xmlNamespace.Prefix, null, xmlNamespace.Uri);
                        }

                        if (referenceInfo != null)
                        {
                            xmlWriter.WriteAttributeString(namespacePrefix, GraphId, null, referenceInfo.Id.ToString());
                        }

                        if (memberTypeToSerialize != memberValue.Type)
                        {
                            var memberTypeToSerializerName = TypeHelper.GetTypeName(memberTypeToSerialize.FullName);
                            xmlWriter.WriteAttributeString(namespacePrefix, "type", null, memberTypeToSerializerName);
                        }

                        serializer.WriteObjectContent(xmlWriter, memberValue.Value);

                        xmlWriter.WriteEndElement();
                    }
                }
            }

            EnsureNamespaceInXmlDocument(element, xmlNamespace);

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
        private void AddReferenceId(ISerializationContext context, XElement element, object model)
        {
            var referenceManager = context.ReferenceManager;
            var referenceInfo = referenceManager.GetInfo(model);

            element.Add(new XAttribute(GraphId, referenceInfo.Id));
        }

        /// <summary>
        /// Ensures the catel namespace in the xml document.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="xmlNamespace">The XML namespace. Can be <c>null</c>.</param>
        private void EnsureNamespaceInXmlDocument(XElement element, XmlNamespace xmlNamespace)
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

                        if (xmlNamespace != null)
                        {
                            var dynamicNamespaceName = XNamespace.Xmlns + xmlNamespace.Prefix;
                            var dynamicNamespace = new XAttribute(dynamicNamespaceName, xmlNamespace.Uri);

                            documentRoot.Add(dynamicNamespace);
                        }
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