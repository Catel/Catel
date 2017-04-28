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
        /// <param name="objectAdapter">The object adapter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dataContractSerializerFactory" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="xmlNamespaceManager" /> is <c>null</c>.</exception>
        public XmlSerializer(ISerializationManager serializationManager, IDataContractSerializerFactory dataContractSerializerFactory,
            IXmlNamespaceManager xmlNamespaceManager, ITypeFactory typeFactory, IObjectAdapter objectAdapter)
            : base(serializationManager, typeFactory, objectAdapter)
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
        /// Gets or sets the default fallback optimalization mode if it's not specified via <see cref="XmlSerializationConfiguration"/>.
        /// <para />
        /// The default value is <see cref="XmlSerializerOptimalizationMode.Performance"/>.
        /// </summary>
        /// <value>The optimalization mode.</value>
        public XmlSerializerOptimalizationMode OptimalizationMode { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void Serialize(object model, ISerializationContext<XmlSerializationContextInfo> context)
        {
            var customXmlSerializable = model as ICustomXmlSerializable;
            if (customXmlSerializable != null)
            {
                customXmlSerializable.Serialize(context.Context.Element);
                return;
            }

            base.Serialize(model, context);
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected override object Deserialize(object model, ISerializationContext<XmlSerializationContextInfo> context)
        {
            var customXmlSerializable = model as ICustomXmlSerializable;
            if (customXmlSerializable != null)
            {
                customXmlSerializable.Deserialize(context.Context.Element);
                return customXmlSerializable;
            }

            return base.Deserialize(model, context);
        }

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
                WarmupMember(type, fieldToSerialize.Key, MemberType.Field);
            }

            var catelPropertiesToSerialize = SerializationManager.GetCatelPropertiesToSerialize(type);
            foreach (var propertyToSerialize in catelPropertiesToSerialize)
            {
                WarmupMember(type, propertyToSerialize.Key, MemberType.Property);
            }

            var regularPropertiesToSerialize = SerializationManager.GetRegularPropertiesToSerialize(type);
            foreach (var propertyToSerialize in regularPropertiesToSerialize)
            {
                WarmupMember(type, propertyToSerialize.Key, MemberType.Property);
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
                            Log.Warning("Failed to retrieve the field info of '{0}.{1}' during warmup", type.GetSafeFullName(false), memberName);
                            return;
                        }

                        memberRepresentedType = fieldInfo.FieldType;
                        break;

                    case MemberType.Property:
                        var propertyInfo = type.GetPropertyEx(memberName);
                        if (propertyInfo == null)
                        {
                            Log.Warning("Failed to retrieve the property info of '{0}.{1}' during warmup", type.GetSafeFullName(false), memberName);
                            return;
                        }

                        memberRepresentedType = propertyInfo.PropertyType;
                        break;
                }

                var xmlName = memberName;
                if (propertyDataManager.IsPropertyNameMappedToXmlElement(type, memberName))
                {
                    xmlName = propertyDataManager.MapPropertyNameToXmlElementName(type, memberName);
                }

                _dataContractSerializerFactory.GetDataContractSerializer(type, memberRepresentedType, xmlName, null, null);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to warmup member '{0}.{1}'. This member might cause problems during serialization", type.GetSafeFullName(false), memberName);
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

            if (memberValue.MemberGroup == SerializationMemberGroup.SimpleRootObject)
            {
                WriteXmlElement(context, element, memberValue.Name, memberValue, memberValue.MemberType);
                return;
            }

            if (ShouldSerializeAsDictionary(memberValue))
            {
                // TODO: For now only support top-level dictionaries
                if (context.Depth == 0)
                {
                    var collection = ConvertDictionaryToCollection(memberValue.Value);
                    if (collection != null)
                    {
                        Serialize(collection, context.Context, context.Configuration);
                    }
                    return;
                }
            }

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
                if (memberValue.MemberGroup == SerializationMemberGroup.SimpleRootObject)
                {
                    var value = GetObjectFromXmlElement(context, element.Element(RootObjectName), memberValue, modelType);
                    return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                }

                if (memberValue.MemberGroup == SerializationMemberGroup.Dictionary)
                {
                    var value = GetObjectFromXmlElement(context, element, memberValue, modelType);
                    return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                }

                if (memberValue.MemberGroup == SerializationMemberGroup.Collection)
                {
                    var value = GetObjectFromXmlElement(context, element, memberValue, modelType);
                    return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                }

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
                    var elementName = memberValue.Name;

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
                Log.Debug(ex, "Failed to deserialize '{0}.{1}'", memberValue.ModelTypeName, memberValue.Name);
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
                    if (property.IsDecoratedWithAttribute<XmlIgnoreAttribute>())
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

            OptimizeXDocument(document, context);

            if (ShouldSerializeAsCollection(context.ModelType))
            {
                // Because we have 'Items\Items' for collections, remote the root
                document = new XDocument(document.Root.FirstNode);
            }

            document.Save(stream);
        }

        /// <summary>
        /// Gets the name of the xml element.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="model">The model.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>System.String.</returns>
        protected string GetXmlElementName(Type modelType, object model, string memberName)
        {
            if (ShouldSerializeAsCollection(modelType))
            {
                return CollectionName;
            }

            XmlRootAttribute xmlRootAttribute;
            if (modelType.TryGetAttribute(out xmlRootAttribute))
            {
                return xmlRootAttribute.ElementName;
            }

            if (!string.IsNullOrWhiteSpace(memberName))
            {
                var propertyDataManager = PropertyDataManager.Default;
                if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, memberName))
                {
                    return propertyDataManager.MapPropertyNameToXmlElementName(modelType, memberName);
                }
            }

            return modelType.Name;
        }

        /// <summary>
        /// Gets the XML optimalization mode. First, the value will be retrieved from the <c>context.Configuration</c> value if
        /// it's of type <c>XmlSerializationConfiguration</c>. Otherwise the <see cref="OptimalizationMode"/> will be used.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual XmlSerializerOptimalizationMode GetXmlOptimalizationMode(ISerializationContext<XmlSerializationContextInfo> context)
        {
            XmlSerializerOptimalizationMode? optimalizationMode = null;

            var xmlSerializationConfiguration = context.Configuration as XmlSerializationConfiguration;
            if (xmlSerializationConfiguration != null)
            {
                optimalizationMode = xmlSerializationConfiguration.OptimalizationMode;
            }
            else
            {
                optimalizationMode = OptimalizationMode;
            }

            return optimalizationMode.Value;
        }

        /// <summary>
        /// Optimizes the xml document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="context">The context.</param>
        protected virtual void OptimizeXDocument(XDocument document, ISerializationContext<XmlSerializationContextInfo> context)
        {
            var optimalizationMode = GetXmlOptimalizationMode(context);
            if (optimalizationMode == XmlSerializerOptimalizationMode.Performance)
            {
                return;
            }

            OptimizeXElement(document.Root, optimalizationMode);
        }

        /// <summary>
        /// Optimizes the xml element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="optimalizationMode">The optimalization mode.</param>
        protected virtual void OptimizeXElement(XElement element, XmlSerializerOptimalizationMode optimalizationMode)
        {
            if (optimalizationMode == XmlSerializerOptimalizationMode.Performance)
            {
                return;
            }

            var agressive = (optimalizationMode == XmlSerializerOptimalizationMode.PrettyXmlAgressive);
            if (agressive)
            {
                // Important: children first
                foreach (var child in element.Elements())
                {
                    OptimizeXElement(child, optimalizationMode);
                }
            }

            var rootNamespaceAttributes = (from attribute in element.Attributes()
                                           where attribute.IsNamespaceDeclaration
                                           select attribute).ToList();

            foreach (var rootNamespaceAttribute in rootNamespaceAttributes)
            {
                rootNamespaceAttribute.Remove();
            }

            if (agressive)
            {
                // Clear xmlns namespaces
                if (!string.IsNullOrEmpty(element.Name.NamespaceName))
                {
                    element.Name = element.Name.LocalName;
                }
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The serialization context.
        /// </returns>
        protected override ISerializationContext<XmlSerializationContextInfo> GetContext(object model, Type modelType, Stream stream, 
            SerializationContextMode contextMode, ISerializationConfiguration configuration)
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

            var isNewDocument = document == null;
            if (isNewDocument)
            {
                var rootName = "root";
                if (model != null)
                {
                    rootName = _rootNameCache.GetFromCacheOrFetch(modelType, () =>
                    {
                        return GetXmlElementName(modelType, model, null);
                    });
                }

                document = new XDocument(new XElement(rootName));
            }

            var contextInfo = new XmlSerializationContextInfo(document.Root, model);
            var context = new SerializationContext<XmlSerializationContextInfo>(model, modelType, contextInfo, contextMode, configuration);

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

            return StringToObjectHelper.ToRightType(memberValue.MemberType, value);
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
            var xmlName = element.Name.LocalName;

            var propertyTypeToDeserialize = memberValue.MemberType;

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

            var typeAttribute = element.Attribute("type"); // .GetAttribute("type", "http://schemas.catelproject.com");
            var attributeValue = (typeAttribute != null) ? typeAttribute.Value : null;
            if (!string.IsNullOrEmpty(attributeValue))
            {
                var typeToDeserialize = TypeCache.GetTypeWithoutAssembly(attributeValue, allowInitialization: false);
                if (typeToDeserialize != null && propertyTypeToDeserialize != typeToDeserialize)
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'",
                        memberValue.Name, memberValue.MemberType.FullName, attributeValue);

                    propertyTypeToDeserialize = typeToDeserialize;
                }
                else
                {
                    Log.Warning("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'. Unfortunately the type cannot be found so the deserialization will probably fail.",
                        memberValue.Name, memberValue.MemberType.FullName, attributeValue);
                }
            }

            var isDeserialized = false;

            if (propertyTypeToDeserialize == typeof(string) && ShouldSerializeUsingParseAndToString(memberValue, false))
            {
                var tempValue = memberValue.Value;
                memberValue.Value = element.Value;

                var parsedValue = DeserializeUsingObjectParse(context, memberValue);
                if (parsedValue != null)
                {
                    value = parsedValue;

                    isDeserialized = true;
                }
                else
                {
                    memberValue.Value = tempValue;
                }
            }

            if (!isDeserialized && ShouldSerializeModelAsCollection(propertyTypeToDeserialize))
            {
                var collection = value as IList;
                if (collection == null)
                {
                    collection = CreateModelInstance(propertyTypeToDeserialize) as IList;
                }

                if (collection == null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>("Cannot deserialize type '{0}', it should implement IList in order to be deserialized", propertyTypeToDeserialize.GetSafeFullName(false));
                }

                var realCollectionType = collection.GetType();
                var childElementType = realCollectionType.GetCollectionElementType();
                if (childElementType == null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>("Cannot deserialize type '{0}', could not determine the element type of the collection", propertyTypeToDeserialize.GetSafeFullName(false));
                }

                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(propertyTypeToDeserialize, childElementType, xmlName, null, null);

                var childElements = element.Elements();
                foreach (var childElement in childElements)
                {
                    using (var xmlReader = childElement.CreateReader())
                    {
                        var childValue = serializer.ReadObject(xmlReader, false);
                        if (childValue != null)
                        {
                            collection.Add(childValue);
                        }
                    }
                }

                value = collection;

                isDeserialized = true;
            }

            if (!isDeserialized)
            {
                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToDeserialize, xmlName, null, null);

                using (var xmlReader = element.CreateReader())
                {
                    value = serializer.ReadObject(xmlReader, false);
                }
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
                if (memberValue.Value == null)
                {
                    xmlWriter.WriteStartElement(elementName);

#if XAMARIN
                    xmlWriter.WriteAttributeString("xmlns", namespacePrefix, defaultNamespace, "http://schemas.catelproject.com");
#endif

                    xmlWriter.WriteAttributeString(namespacePrefix, "IsNull", null, "true");
                    xmlWriter.WriteEndElement();
                }
                else
                {
                    var memberTypeToSerialize = memberValue.GetBestMemberType();
                    var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, memberTypeToSerialize, elementName, null, null);

                    ReferenceInfo referenceInfo = null;
                    var serializeElement = true;

                    if (memberValue.MemberGroup != SerializationMemberGroup.Collection)
                    {
                        var isClassType = memberTypeToSerialize.IsClassType();
                        if (isClassType)
                        {
                            var referenceManager = context.ReferenceManager;
                            referenceInfo = referenceManager.GetInfo(memberValue.Value);

                            if (!referenceInfo.IsFirstUsage)
                            {
                                // Note: we don't want to call GetSafeFullName if we don't have to
                                if (LogManager.IsDebugEnabled ?? false)
                                {
                                    Log.Debug("Existing reference detected for element type '{0}' with id '{1}', only storing id", memberTypeToSerialize.GetSafeFullName(false), referenceInfo.Id);
                                }

                                //serializer.WriteStartObject(xmlWriter, memberValue.Value);
                                xmlWriter.WriteStartElement(elementName);

                                xmlWriter.WriteAttributeString(namespacePrefix, GraphRefId, null, referenceInfo.Id.ToString());

                                //serializer.WriteEndObject(xmlWriter);
                                xmlWriter.WriteEndElement();

                                serializeElement = false;
                            }
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

                        if (memberTypeToSerialize != memberValue.MemberType)
                        {
                            var memberTypeToSerializerName = TypeHelper.GetTypeName(memberTypeToSerialize.FullName);
                            xmlWriter.WriteAttributeString(namespacePrefix, "type", null, memberTypeToSerializerName);
                        }

                        // In special cases, we need to write our own collection items. One case is where a custom ModelBase
                        // implements IList and gets inside a StackOverflow
                        var serialized = false;
                        if (ShouldSerializeModelAsCollection(memberValue.GetBestMemberType()))
                        {
                            var collection = memberValue.Value as IEnumerable;
                            if (collection != null)
                            {
                                foreach (var item in collection)
                                {
                                    var subItemElementName = GetXmlElementName(item.GetType(), item, null);
                                    xmlWriter.WriteStartElement(subItemElementName);

                                    serializer.WriteObjectContent(xmlWriter, item);

                                    xmlWriter.WriteEndElement();
                                }

                                serialized = true;
                            }
                        }

                        if (!serialized)
                        {
                            serializer.WriteObjectContent(xmlWriter, memberValue.Value);
                        }

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
            return "http://schemas.catelproject.com";
        }
        #endregion
    }
}