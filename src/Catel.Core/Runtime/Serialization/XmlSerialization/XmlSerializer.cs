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

    using Collections;
    using Caching;
    using Data;
    using IoC;
    using Logging;
    using Reflection;

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
        private const string XmlIsNull = "IsNull";
        private const string XmlType = "type";
        private const string XmlGraphId = "graphid";
        private const string XmlGraphRefId = "graphrefid";

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
        }
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
                customXmlSerializable.Serialize(context.Context.XmlWriter);
                return;
            }

            var rootName = "root";
            if (model != null)
            {
                rootName = _rootNameCache.GetFromCacheOrFetch(context.ModelType, () =>
                {
                    return GetXmlElementName(context.ModelType, model, null);
                });
            }

            var xmlWriter = context.Context.XmlWriter;

            var isRootObject = (xmlWriter.WriteState == WriteState.Start);
            if (isRootObject)
            {
                xmlWriter.WriteStartElement(rootName);

                EnsureNamespaceInXmlWriter(xmlWriter, null);

                AddReferenceId(context, context.Model);
            }

            base.Serialize(model, context);

            if (isRootObject)
            {
                xmlWriter.WriteEndElement();
            }
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
                customXmlSerializable.Deserialize(context.Context.XmlReader);
                return customXmlSerializable;
            }

            var xmlReader = context.Context.XmlReader;
            if (xmlReader.NodeType == XmlNodeType.None)
            {
                xmlReader.Read();
            }

            return base.Deserialize(model, context);
        }

        /// <summary>
        /// Warms up the specified type.
        /// </summary>
        /// <param name="type">The type to warmup.</param>
        protected override void Warmup(Type type)
        {
            if (type is null)
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
                        if (fieldInfo is null)
                        {
                            Log.Warning("Failed to retrieve the field info of '{0}.{1}' during warmup", type.GetSafeFullName(false), memberName);
                            return;
                        }

                        memberRepresentedType = fieldInfo.FieldType;
                        break;

                    case MemberType.Property:
                        var propertyInfo = type.GetPropertyEx(memberName);
                        if (propertyInfo is null)
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

                referenceManager.GetInfo(context.Model, true);
            }
        }

        /// <summary>
        /// Called before the serializer starts deserializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeDeserialization(ISerializationContext<XmlSerializationContextInfo> context)
        {
            base.BeforeDeserialization(context);

            var namespacePrefix = GetNamespacePrefix();
            var xmlReader = context.Context.XmlReader;

            var graphIdAttributeValue = xmlReader.GetAttribute($"{namespacePrefix}:{XmlGraphId}");
            if (!string.IsNullOrWhiteSpace(graphIdAttributeValue))
            {
                var graphId = int.Parse(graphIdAttributeValue);

                var referenceManager = context.ReferenceManager;
                var referenceInfo = referenceManager.GetInfoById(graphId);
                if (referenceInfo != null)
                {
                    Log.Warning($"Trying to register custom object in graph with graph id '{graphId}', but it seems it is already registered");
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

            if (memberValue.MemberGroup == SerializationMemberGroup.SimpleRootObject)
            {
                WriteXmlElement(context, memberValue.Name, memberValue, memberValue.MemberType);
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
                        // Note: since we are not getting into a WriteXml call, get known types for the current context here
                        var knownTypesHashSet = context.Context.KnownTypes;
                        var knownTypes = _dataContractSerializerFactory.GetKnownTypes(modelType, collection.GetType(), knownTypesHashSet.ToList());
                        knownTypesHashSet.AddRange(knownTypes);

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

                WriteXmlAttribute(context, attributeName, memberValue);
            }
            else
            {
                var elementName = memberValue.Name;

                if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, memberValue.Name))
                {
                    elementName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, memberValue.Name);
                }

                Log.Debug("Serializing property {0}.{1} as xml element '{2}'", modelType.FullName, memberValue.Name, elementName);

                WriteXmlElement(context, elementName, memberValue, modelType);
            }
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The deserialized list of member values.</returns>
        protected override List<MemberValue> DeserializeMembers(ISerializationContext<XmlSerializationContextInfo> context)
        {
            var deserializedMemberValues = new List<MemberValue>();
            var xmlReader = context.Context.XmlReader;
            var modelType = context.ModelType;

            var propertyDataManager = PropertyDataManager.Default;
            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType).Reverse();
            var membersToDeserialize = GetSerializableMembers(context, context.Model);

            // Important: we need to respect the xml order (since it's a forward-only reader). We will do this in 
            // 2 steps:
            // 1: attributes
            // 2: elements

            var startMember = xmlReader.LocalName;

            var attributeMembers = new Dictionary<string, MemberValue>();
            var elementMembers = new Dictionary<string, MemberValue>();

            foreach (var memberToDeserialize in membersToDeserialize)
            {
                var memberName = memberToDeserialize.Name;

                if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, memberToDeserialize.Name))
                {
                    attributeMembers.Add(memberName, memberToDeserialize);
                }
                else
                {
                    if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, memberToDeserialize.Name))
                    {
                        memberName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, memberToDeserialize.Name);
                    }

                    elementMembers.Add(memberName, memberToDeserialize);
                }
            }

            // Step 1: attributes
            foreach (var attributeMember in attributeMembers)
            {
                StartMemberDeserialization(context, attributeMember.Value);

                var serializationObject = DeserializeMember(context, attributeMember.Value);

                var finalMemberValue = EndMemberDeserialization(context, attributeMember.Value, serializationObject, serializerModifiers);
                if (finalMemberValue != null)
                {
                    deserializedMemberValues.Add(finalMemberValue);
                }
            }

            xmlReader.MoveToContent();
            xmlReader.Read();

            // Step 2: elements
            while (true)
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement &&
                    xmlReader.LocalName.Equals(startMember))
                {
                    // We've hit the end of the object
                    break;
                }

                if (xmlReader.NodeType != XmlNodeType.Element &&
                    xmlReader.NodeType != XmlNodeType.EndElement)
                {
                    xmlReader.MoveToContent();
                    continue;
                }

                var localName = xmlReader.LocalName;
                if (elementMembers.TryGetValue(localName, out var elementMemberValue))
                {
                    StartMemberDeserialization(context, elementMemberValue);

                    var serializationObject = DeserializeMember(context, elementMemberValue);

                    var finalMemberValue = EndMemberDeserialization(context, elementMemberValue, serializationObject, serializerModifiers);
                    if (finalMemberValue != null)
                    {
                        deserializedMemberValues.Add(finalMemberValue);
                    }
                }
                else
                {
                    Log.Debug($"Ignoring member '{localName}'");
                }
            }

            return deserializedMemberValues;
        }

        /// <summary>
        /// Deserializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The <see cref="SerializationObject"/> representing the deserialized value or result.</returns>
        protected override SerializationObject DeserializeMember(ISerializationContext<XmlSerializationContextInfo> context, MemberValue memberValue)
        {
            // Note: super important assumption. Since we are using a forward-only reader, we will only read the currently
            // available node in the xml reader

            var modelType = context.ModelType;
            var xmlReader = context.Context.XmlReader;

            try
            {
                if (memberValue.MemberGroup == SerializationMemberGroup.SimpleRootObject)
                {
                    xmlReader.MoveToContent();

                    var value = GetObjectFromXmlElement(context, memberValue, modelType);
                    return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                }

                if (memberValue.MemberGroup == SerializationMemberGroup.Dictionary)
                {
                    var value = GetObjectFromXmlElement(context, memberValue, modelType);
                    return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                }

                if (memberValue.MemberGroup == SerializationMemberGroup.Collection)
                {
                    var value = GetObjectFromXmlElement(context, memberValue, modelType);
                    return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                }

                var propertyDataManager = PropertyDataManager.Default;
                if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, memberValue.Name))
                {
                    var mappedPropertyName = propertyDataManager.MapPropertyNameToXmlAttributeName(modelType, memberValue.Name);

                    //Log.Debug("Deserializing property {0}.{1} as xml attribute '{2}'", modelType.FullName, memberValue.Name, mappedPropertyName);

                    var attributeValue = xmlReader.GetAttribute(mappedPropertyName);
                    if (!string.IsNullOrWhiteSpace(attributeValue))
                    {
                        var value = StringToObjectHelper.ToRightType(memberValue.MemberType, attributeValue);
                        return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
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

                    var value = GetObjectFromXmlElement(context, memberValue, modelType);
                    return SerializationObject.SucceededToDeserialize(modelType, memberValue.MemberGroup, memberValue.Name, value);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex, $"Failed to deserialize '{memberValue.ModelTypeName}.{memberValue.Name}'");
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
            var xmlWriter = context.Context.XmlWriter;
            xmlWriter.Flush();

            //var element = context.Context.Element;
            //var document = new XDocument(element);

            //OptimizeXDocument(document, context);

            //if (ShouldSerializeAsCollection(context.ModelType))
            //{
            //    // Because we have 'Items\Items' for collections, remote the root
            //    document = new XDocument(document.Root.FirstNode);
            //}

            //document.Save(stream);
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
            string xmlElementName = null;

            if (ShouldSerializeAsCollection(modelType))
            {
                xmlElementName = CollectionName;
            }

            else if (modelType.TryGetAttribute(out XmlRootAttribute xmlRootAttribute))
            {
                xmlElementName = xmlRootAttribute.ElementName;
            }

            else if (!string.IsNullOrWhiteSpace(memberName))
            {
                var propertyDataManager = PropertyDataManager.Default;
                if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, memberName))
                {
                    xmlElementName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, memberName);
                }
            }
            else
            {
                xmlElementName = modelType.Name;
            }

            // Fix for https://github.com/Catel/Catel/issues/1073
            if (!string.IsNullOrWhiteSpace(xmlElementName))
            {
                xmlElementName = XmlConvert.EncodeName(xmlElementName);
            }

            return xmlElementName;
        }

        /// <summary>
        /// Gets the XML optimalization mode. First, the value will be retrieved from the <c>context.Configuration</c> value if
        /// it's of type <c>XmlSerializationConfiguration</c>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual XmlSerializerOptimalizationMode GetXmlOptimalizationMode(ISerializationContext<XmlSerializationContextInfo> context)
        {
            var optimalizationMode = XmlSerializerOptimalizationMode.Performance;

            var xmlSerializationConfiguration = context.Configuration as XmlSerializationConfiguration;
            if (xmlSerializationConfiguration != null)
            {
                optimalizationMode = xmlSerializationConfiguration.OptimalizationMode;
            }

            return optimalizationMode;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model, can be <c>null</c> for value types.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// ISerializationContext{SerializationInfo}.
        /// </returns>
        [ObsoleteEx(ReplacementTypeOrMember = "GetSerializationContextInfo", TreatAsErrorFromVersion = "5.6", RemoveInVersion = "6.0")]
        protected override ISerializationContext<XmlSerializationContextInfo> GetContext(object model, Type modelType, Stream stream,
            SerializationContextMode contextMode, ISerializationConfiguration configuration)
        {
            return GetSerializationContextInfo(model, modelType, stream, contextMode, configuration);
        }

        /// <summary>
        /// Gets the serializer specific serialization context info.
        /// </summary>
        /// <param name="model">The model, can be <c>null</c> for value types.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The serialization context.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        protected override ISerializationContext<XmlSerializationContextInfo> GetSerializationContextInfo(object model, Type modelType, Stream stream,
            SerializationContextMode contextMode, ISerializationConfiguration configuration)
        {
            XmlSerializationContextInfo contextInfo = null;

            try
            {
                if (stream.Length != 0)
                {
                    var xmlReader = XmlReader.Create(stream);

                    contextInfo = new XmlSerializationContextInfo(xmlReader, model);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load document from stream, falling back to empty document");
            }

            var isNewDocument = contextInfo is null;
            if (isNewDocument)
            {
                var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    CheckCharacters = false,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                });

                contextInfo = new XmlSerializationContextInfo(xmlWriter, model);
            }

            var context = new SerializationContext<XmlSerializationContextInfo>(model, modelType, contextInfo, contextMode, configuration);
            return context;
        }

        /// <summary>
        /// Gets the object from XML element.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <returns>Object or <c>null</c>.</returns>
        /// <remarks>Note that this method can cause exceptions. The caller will handle them.</remarks>
        private object GetObjectFromXmlElement(ISerializationContext<XmlSerializationContextInfo> context, MemberValue memberValue, Type modelType)
        {
            object value = null;

            var namespacePrefix = GetNamespacePrefix();
            var xmlReader = context.Context.XmlReader;
            var xmlName = xmlReader.LocalName;

            var propertyTypeToDeserialize = memberValue.MemberType;

            var isNullAttributeValue = xmlReader.GetAttribute($"{namespacePrefix}:{XmlIsNull}");
            var isNull = !string.IsNullOrWhiteSpace(isNullAttributeValue) ? StringToObjectHelper.ToBool(isNullAttributeValue) : false;
            if (isNull)
            {
                return null;
            }

            // Fix for CTL-555, note that we'll use this method at the end of the method, once we've read the model
            var graphIdAttributeValue = xmlReader.GetAttribute($"{namespacePrefix}:{XmlGraphId}");

            var graphRefIdAttributeValue = xmlReader.GetAttribute($"{namespacePrefix}:{XmlGraphRefId}");
            if (!string.IsNullOrWhiteSpace(graphRefIdAttributeValue))
            {
                var graphId = int.Parse(graphRefIdAttributeValue);

                var referenceManager = context.ReferenceManager;
                var referenceInfo = referenceManager.GetInfoById(graphId);
                if (referenceInfo is null)
                {
                    Log.Error($"Expected to find graph object with id '{graphId}' in ReferenceManager, but it was not found. Defaulting value for member '{xmlName}' to null");
                    return null;
                }

                return referenceInfo.Instance;
            }

            var typeAttributeValue = xmlReader.GetAttribute($"{namespacePrefix}:{XmlType}");
            if (!string.IsNullOrEmpty(typeAttributeValue))
            {
                var typeToDeserialize = TypeCache.GetTypeWithoutAssembly(typeAttributeValue, allowInitialization: false);
                if (typeToDeserialize != null && propertyTypeToDeserialize != typeToDeserialize)
                {
                    Log.Debug($"Property type for property '{memberValue.Name}' is '{memberValue.MemberType.FullName}' but found type info that it should be deserialized as '{typeAttributeValue}'");

                    propertyTypeToDeserialize = typeToDeserialize;
                }
                else
                {
                    Log.Warning($"Property type for property '{memberValue.Name}' is '{memberValue.MemberType.FullName}' but found type info that it should be deserialized as '{typeAttributeValue}'. Unfortunately the type cannot be found so the deserialization will probably fail.");
                }
            }

            var isDeserialized = false;

            if (propertyTypeToDeserialize == typeof(string) && ShouldSerializeUsingParseAndToString(memberValue, false))
            {
                var tempValue = memberValue.Value;
                memberValue.Value = xmlReader.ReadContentAsString();

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

            // Serialization dictionaries as collections
            if (!isDeserialized && ShouldSerializeAsDictionary(propertyTypeToDeserialize))
            {
                // Force deserialization as List<>
                propertyTypeToDeserialize = typeof(List<SerializableKeyValuePair>);
            }

            // Special case if a Catel ModelBase should be serialized as collection
            if (!isDeserialized && ShouldSerializeModelAsCollection(propertyTypeToDeserialize))
            {
                var collection = value as IList;
                if (collection is null)
                {
                    collection = CreateModelInstance(propertyTypeToDeserialize) as IList;
                }

                if (collection is null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>("Cannot deserialize type '{0}', it should implement IList in order to be deserialized", propertyTypeToDeserialize.GetSafeFullName(false));
                }

                var realCollectionType = collection.GetType();
                var childElementType = realCollectionType.GetCollectionElementType();
                if (childElementType is null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>("Cannot deserialize type '{0}', could not determine the element type of the collection", propertyTypeToDeserialize.GetSafeFullName(false));
                }

                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(propertyTypeToDeserialize, childElementType, xmlName, null, null);

                var startMember = xmlReader.LocalName;

                xmlReader.MoveToContent();
                xmlReader.Read();

                while (true)
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement &&
                        xmlReader.LocalName.Equals(startMember))
                    {
                        // We've hit the end of the collection
                        break;
                    }

                    if (xmlReader.NodeType != XmlNodeType.Element &&
                        xmlReader.NodeType != XmlNodeType.EndElement)
                    {
                        xmlReader.MoveToContent();
                        continue;
                    }

                    object childValue = null;

                    // Step 1: check for graph attributes
                    var collectionItemGraphRefIdAttribute = xmlReader.GetAttribute($"{namespacePrefix}:{XmlGraphRefId}");
                    var collectionItemGraphIdAttribute = xmlReader.GetAttribute($"{namespacePrefix}:{XmlGraphId}");

                    if (!string.IsNullOrWhiteSpace(collectionItemGraphRefIdAttribute))
                    {
                        var graphId = int.Parse(collectionItemGraphRefIdAttribute);

                        var referenceManager = context.ReferenceManager;
                        var referenceInfo = referenceManager.GetInfoById(graphId);
                        if (referenceInfo is null)
                        {
                            Log.Error("Expected to find graph object with id '{0}' in ReferenceManager, but it was not found. Defaulting value for member '{1}' to null", graphId, xmlName);
                        }

                        childValue = referenceInfo?.Instance;
                    }

                    // Step 2: deserialize anyway
                    if (childValue is null)
                    {
                        childValue = serializer.ReadObject(xmlReader, false);
                    }

                    if (childValue != null)
                    {
                        collection.Add(childValue);

                        if (!string.IsNullOrWhiteSpace(collectionItemGraphIdAttribute))
                        {
                            var graphId = int.Parse(collectionItemGraphIdAttribute);

                            var referenceManager = context.ReferenceManager;
                            referenceManager.RegisterManually(graphId, childValue);
                        }
                    }
                }

                value = collection;

                // Exit the collection
                xmlReader.Read();

                isDeserialized = true;
            }

            // Fallback to .net serialization
            if (!isDeserialized)
            {
                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, propertyTypeToDeserialize, xmlName, null, null);

                value = serializer.ReadObject(xmlReader, false);
            }

            // Fix for CTL-555
            if (!string.IsNullOrWhiteSpace(graphIdAttributeValue))
            {
                var graphId = int.Parse(graphIdAttributeValue);

                var referenceManager = context.ReferenceManager;
                referenceManager.RegisterManually(graphId, value);
            }

            return value;
        }

        /// <summary>
        /// Writes the XML attribute to the xml element.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="memberValue">The member value.</param>
        private void WriteXmlAttribute(ISerializationContext<XmlSerializationContextInfo> context, string attributeName, MemberValue memberValue)
        {
            var xmlWriter = context.Context.XmlWriter;
            var attributeValue = ObjectToStringHelper.ToString(memberValue.Value);

            xmlWriter.WriteAttributeString(attributeName, attributeValue);
        }

        /// <summary>
        /// Writes the XML element to the xml element.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="memberValue">The member value.</param>
        /// <param name="modelType">Type of the model.</param>
        private void WriteXmlElement(ISerializationContext<XmlSerializationContextInfo> context, string elementName, MemberValue memberValue, Type modelType)
        {
            var namespacePrefix = GetNamespacePrefix();

#if XAMARIN
            var defaultNamespace = "http://www.w3.org/2000/xmlns/";
#endif

            var xmlWriter = context.Context.XmlWriter;

            if (memberValue.Value is null)
            {
                xmlWriter.WriteStartElement(elementName);

#if XAMARIN
                xmlWriter.WriteAttributeString("xmlns", namespacePrefix, defaultNamespace, "http://schemas.catelproject.com");
#endif

                xmlWriter.WriteAttributeString(namespacePrefix, XmlIsNull, null, "true");
                xmlWriter.WriteEndElement();
            }
            else
            {
                var memberTypeToSerialize = memberValue.GetBestMemberType();
                var additionalKnownTypes = context.Context.KnownTypes;
                var serializer = _dataContractSerializerFactory.GetDataContractSerializer(modelType, memberTypeToSerialize, elementName, null, additionalKnownTypes.ToList());

                // We might have added more known types in the serializer
                additionalKnownTypes.AddRange(serializer.KnownTypes);

                var referenceManager = context.ReferenceManager;
                ReferenceInfo referenceInfo = null;
                var serializeElement = true;

                if (memberValue.MemberGroup != SerializationMemberGroup.Collection)
                {
                    if (memberTypeToSerialize.IsClassType())
                    {
                        referenceInfo = referenceManager.GetInfo(memberValue.Value, true);

                        if (WriteXmlElementAsGraphReference(xmlWriter, referenceInfo, memberTypeToSerialize,
                                elementName, namespacePrefix))
                        {
                            serializeElement = false;
                        }
                    }
                }

                if (serializeElement)
                {
                    xmlWriter.WriteStartElement(elementName);

                    AddObjectMetadata(xmlWriter, memberTypeToSerialize, memberValue.MemberType, referenceInfo, namespacePrefix);

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
                                var itemType = item.GetType();

                                var subItemElementName = GetXmlElementName(itemType, item, null);
                                referenceInfo = referenceManager.GetInfo(item, true);

                                if (!WriteXmlElementAsGraphReference(xmlWriter, referenceInfo, itemType,
                                        subItemElementName, namespacePrefix))
                                {
                                    xmlWriter.WriteStartElement(subItemElementName);

                                    AddObjectMetadata(xmlWriter, itemType, itemType, referenceInfo, namespacePrefix);

                                    serializer.WriteObjectContent(xmlWriter, item);

                                    xmlWriter.WriteEndElement();
                                }
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

        private void AddObjectMetadata(XmlWriter xmlWriter, Type memberTypeToSerialize, Type actualMemberType,
            ReferenceInfo referenceInfo, string namespacePrefix)
        {
            if (referenceInfo != null)
            {
                xmlWriter.WriteAttributeString(namespacePrefix, XmlGraphId, null, referenceInfo.Id.ToString());
            }

            if (memberTypeToSerialize != actualMemberType)
            {
                var memberTypeToSerializerName = TypeHelper.GetTypeName(memberTypeToSerialize.FullName);
                xmlWriter.WriteAttributeString(namespacePrefix, XmlType, null, memberTypeToSerializerName);
            }
        }

        private bool WriteXmlElementAsGraphReference(XmlWriter xmlWriter, ReferenceInfo referenceInfo, Type memberTypeToSerialize, string elementName, string namespacePrefix)
        {
            var isClassType = memberTypeToSerialize.IsClassType();
            if (!isClassType)
            {
                return false;
            }

            if (!referenceInfo.IsFirstUsage)
            {
                // Note: we don't want to call GetSafeFullName if we don't have to
                if (LogManager.IsDebugEnabled ?? false)
                {
                    Log.Debug($"Existing reference detected for element type '{memberTypeToSerialize.GetSafeFullName(false)}' with id '{referenceInfo.Id}', only storing id");
                }

                //serializer.WriteStartObject(xmlWriter, memberValue.Value);
                xmlWriter.WriteStartElement(elementName);

                xmlWriter.WriteAttributeString(namespacePrefix, XmlGraphRefId, null, referenceInfo.Id.ToString());

                //serializer.WriteEndObject(xmlWriter);
                xmlWriter.WriteEndElement();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the reference unique identifier as attribute.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        private void AddReferenceId(ISerializationContext<XmlSerializationContextInfo> context, object model)
        {
            var xmlWriter = context.Context.XmlWriter;
            var referenceManager = context.ReferenceManager;
            var referenceInfo = referenceManager.GetInfo(model, true);
            var namespacePrefix = GetNamespacePrefix();

            AddObjectMetadata(xmlWriter, null, null, referenceInfo, namespacePrefix);
        }

        /// <summary>
        /// Ensures the catel namespace in the xml document.
        /// </summary>
        /// <param name="xmlWriter">The xml writer.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        private void EnsureNamespaceInXmlWriter(XmlWriter xmlWriter, XmlNamespace xmlNamespace)
        {
            var catelNamespacePrefix = GetNamespacePrefix();
            var catelNamespaceUrl = GetNamespaceUrl();

            xmlWriter.WriteAttributeString(catelNamespacePrefix, "http://www.w3.org/2000/xmlns/", catelNamespaceUrl);
            xmlWriter.WriteAttributeString("i", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");

            if (xmlNamespace != null)
            {
                xmlWriter.WriteAttributeString(xmlNamespace.Prefix, "http://www.w3.org/2000/xmlns/", xmlNamespace.Uri);
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
