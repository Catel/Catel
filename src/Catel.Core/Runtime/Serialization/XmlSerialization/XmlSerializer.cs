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
    using System.Runtime.Serialization;

    /// <summary>
    /// The xml serializer.
    /// </summary>
    public class XmlSerializer : SerializerBase<XmlSerializationContextInfo>, IXmlSerializer
    {
        private enum MemberType
        {
            Field,

            Property
        }

        private const string XmlIsNull = "IsNull";
        private const string XmlType = "type";
        private const string XmlGraphId = "graphid";
        private const string XmlGraphRefId = "graphrefid";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly CacheStorage<Type, HashSet<string>> _ignoredMembersCache = new CacheStorage<Type, HashSet<string>>();
        private readonly CacheStorage<Type, string> _rootNameCache = new CacheStorage<Type, string>();
        private readonly IDataContractSerializerFactory _dataContractSerializerFactory;
        private readonly IXmlNamespaceManager _xmlNamespaceManager;

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
            IXmlNamespaceManager xmlNamespaceManager, ITypeFactory typeFactory, Catel.Runtime.Serialization.IObjectAdapter objectAdapter)
            : base(serializationManager, typeFactory, objectAdapter)
        {
            _dataContractSerializerFactory = dataContractSerializerFactory;
            _xmlNamespaceManager = xmlNamespaceManager;
        }

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void Serialize(object model, ISerializationContext<XmlSerializationContextInfo> context)
        {
            if (context.Context.AllowCustomXmlSerialization)
            {
                var customXmlSerializable = model as ICustomXmlSerializable;
                if (customXmlSerializable is not null)
                {
                    var xmlWriter = context.Context.XmlWriter;
                    if (xmlWriter is not null)
                    {
                        if (context.Context.IsRootObject)
                        {
                            var rootName = GetObjectRootName(context);

                            xmlWriter.WriteStartElement(rootName);

                            EnsureNamespaceInXmlWriter(context, xmlWriter, null);
                        }

                        customXmlSerializable.Serialize(xmlWriter);

                        if (context.Context.IsRootObject)
                        {
                            xmlWriter.WriteEndElement();
                        }
                    }
                    return;
                }
            }

            base.Serialize(model, context);
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected override object? Deserialize(object model, ISerializationContext<XmlSerializationContextInfo> context)
        {
            if (context.Context.AllowCustomXmlSerialization)
            {
                var customXmlSerializable = model as ICustomXmlSerializable;
                if (customXmlSerializable is not null)
                {
                    var xmlReader = context.Context.XmlReader;
                    if (xmlReader is not null)
                    {
                        customXmlSerializable.Deserialize(xmlReader);

                        return customXmlSerializable;
                    }
                }
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
                Type? memberRepresentedType = null;

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

                if (memberRepresentedType is not null)
                {
                    // Cache
                    _dataContractSerializerFactory.GetDataContractSerializer(type, memberRepresentedType, xmlName, null, null);
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to warmup member '{type.GetSafeFullName(false)}.{memberName}'. This member might cause problems during serialization");
            }
        }

        /// <summary>
        /// Called before the serializer starts serializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeSerialization(ISerializationContext<XmlSerializationContextInfo> context)
        {
            base.BeforeSerialization(context);

            if (WriteDocumentStartIfRequired(context))
            {
                var referenceManager = context.ReferenceManager;
                if (referenceManager.Count == 0)
                {
                    //Log.Debug("Reference manager contains no objects yet, adding initial reference which is the first model in the graph");

                    referenceManager.GetInfo(context.Model, true);
                }
            }
        }

        protected override void AfterSerialization(ISerializationContext<XmlSerializationContextInfo> context)
        {
            base.AfterSerialization(context);

            if (context.Context.IsRootObject && !ShouldSerializeAsCollection(context.ModelType))
            {
                var xmlWriter = context.Context.XmlWriter;
                if (xmlWriter is null)
                {
                    throw Log.ErrorAndCreateException<CatelException>("Xml writer is required");
                }

                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// Called before the serializer starts deserializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeDeserialization(ISerializationContext<XmlSerializationContextInfo> context)
        {
            base.BeforeDeserialization(context);

            var xmlReader = context.Context.XmlReader;
            if (xmlReader is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml reader is required");
            }

            if (xmlReader.NodeType == XmlNodeType.None)
            {
                xmlReader.Read();
                xmlReader.MoveToContent();
            }

            var namespacePrefix = GetNamespacePrefix();

            var graphIdAttributeValue = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlGraphId);
            if (!string.IsNullOrWhiteSpace(graphIdAttributeValue))
            {
                var graphId = int.Parse(graphIdAttributeValue);

                var referenceManager = context.ReferenceManager;
                var referenceInfo = referenceManager.GetInfoById(graphId);
                if (referenceInfo is not null)
                {
                    Log.Warning($"Trying to register custom object in graph with graph id '{graphId.ToString()}', but it seems it is already registered");
                    return;
                }

                referenceManager.RegisterManually(graphId, context.Model);
            }
        }

        /// <summary>
        /// Serializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="membersToSerialize">The members to serialize.</param>
        protected override void SerializeMembers(ISerializationContext<XmlSerializationContextInfo> context, List<MemberValue> membersToSerialize)
        {
            if (membersToSerialize.Count == 0)
            {
                return;
            }

            var modelType = context.ModelType;

            var propertyDataManager = PropertyDataManager.Default;

            // Important: we need to respect the xml order (since it's a forward-only writer). We will do this in 
            // 2 steps:
            // 1: attributes
            // 2: elements

            var orderedMembersToSerializer = new List<MemberValue>();

            foreach (var memberToSerialize in membersToSerialize)
            {
                if (propertyDataManager.IsPropertyNameMappedToXmlAttribute(modelType, memberToSerialize.Name))
                {
                    // Insert at the beginning
                    orderedMembersToSerializer.Insert(0, memberToSerialize);
                }
                else
                {
                    // Insert at the end
                    orderedMembersToSerializer.Add(memberToSerialize);
                }
            }

            // Now we can do sorted serialization (attributes first)
            using (GetCurrentSerializationScopeManager(context.Configuration))
            {
                var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

                foreach (var member in orderedMembersToSerializer)
                {
                    if (StartMemberSerialization(context, member, serializerModifiers))
                    {
                        EndMemberSerialization(context, member);
                    }
                }
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
                    if (collection is not null)
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

                //Log.Debug("Serializing property {0}.{1} as xml attribute '{2}'", modelType.FullName, memberValue.Name, attributeName);

                WriteXmlAttribute(context, attributeName, memberValue);
            }
            else
            {
                var elementName = memberValue.Name;

                if (propertyDataManager.IsPropertyNameMappedToXmlElement(modelType, memberValue.Name))
                {
                    elementName = propertyDataManager.MapPropertyNameToXmlElementName(modelType, memberValue.Name);
                }

                //Log.Debug("Serializing property {0}.{1} as xml element '{2}'", modelType.FullName, memberValue.Name, elementName);

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
            if (xmlReader is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml reader is required");
            }

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

                    if (ShouldSerializeAsDictionary(memberToDeserialize.GetBestMemberType()))
                    {
                        // Fixed name for items inside dictionaries
                        memberName = "Items";
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
                if (finalMemberValue is not null)
                {
                    deserializedMemberValues.Add(finalMemberValue);
                }
            }

            if (xmlReader.NodeType == XmlNodeType.None)
            {
                xmlReader.Read();
            }
            else if (xmlReader.IsEmptyElement)
            {
                // No reading required
            }
            else if (!ShouldSerializeAsCollection(modelType) && !ShouldSerializeAsDictionary(modelType))
            {
                xmlReader.MoveToContent();
                xmlReader.Read();
            }

            // Step 2: elements
            if (elementMembers.Count > 0)
            {
                while (true)
                {
                    if (!xmlReader.MoveToNextContentElement(startMember))
                    {
                        // End of object
                        break;
                    }

                    var localName = xmlReader.LocalName;
                    if (elementMembers.TryGetValue(localName, out var elementMemberValue))
                    {
                        StartMemberDeserialization(context, elementMemberValue);

                        var serializationObject = DeserializeMember(context, elementMemberValue);

                        var finalMemberValue = EndMemberDeserialization(context, elementMemberValue, serializationObject, serializerModifiers);
                        if (finalMemberValue is not null)
                        {
                            deserializedMemberValues.Add(finalMemberValue);
                        }
                    }
                    else
                    {
                        //Log.Debug($"Ignoring member '{localName}'");

                        xmlReader.Read();
                    }
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
            if (xmlReader is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml reader is required");
            }

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
            if (xmlWriter is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml writer is required");
            }

            xmlWriter.Flush();
        }

        /// <summary>
        /// Gets the name of the xml element.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="model">The model.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>System.String.</returns>
        protected string GetXmlElementName(Type modelType, object model, string? memberName)
        {
            string xmlElementName = string.Empty;

            if (ShouldSerializeAsCollection(modelType))
            {
                xmlElementName = CollectionName;
            }

            else if (modelType.TryGetAttribute<XmlRootAttribute>(out var xmlRootAttribute))
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
            SerializationContextMode contextMode, ISerializationConfiguration? configuration)
        {
            XmlSerializationContextInfo? contextInfo = null;
            var xmlConfiguration = configuration as XmlSerializationConfiguration;

            try
            {
                if (stream.Length != 0)
                {
                    var xmlReaderSettings = xmlConfiguration?.ReaderSettings ?? new XmlReaderSettings
                    {
                        CheckCharacters = false,
                        IgnoreComments = true,
                    };

#pragma warning disable IDISP001 // Dispose created.
                    var xmlReader = XmlReader.Create(stream, xmlReaderSettings);

                    contextInfo = new XmlSerializationContextInfo(xmlReader, model);
#pragma warning restore IDISP001 // Dispose created.
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load document from stream, falling back to empty document");
            }

            var isNewDocument = contextInfo is null;
            if (isNewDocument)
            {
                var xmlWriterSettings = xmlConfiguration?.WriterSettings ?? new XmlWriterSettings
                {
                    OmitXmlDeclaration = false,
                    CheckCharacters = false,
                    ConformanceLevel = ConformanceLevel.Document,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    Indent = true
                };

#pragma warning disable IDISP001 // Dispose created.
                var xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);

#pragma warning disable IDISP003 // Dispose previous before re-assigning.
                contextInfo = new XmlSerializationContextInfo(xmlWriter, model);
#pragma warning restore IDISP003 // Dispose previous before re-assigning.
#pragma warning restore IDISP001 // Dispose created.
            }

            var context = new SerializationContext<XmlSerializationContextInfo>(model, modelType, contextInfo!, contextMode, configuration);
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
        private object? GetObjectFromXmlElement(ISerializationContext<XmlSerializationContextInfo> context, MemberValue memberValue, Type modelType)
        {
            object? value = null;

            var namespacePrefix = GetNamespacePrefix();
            var xmlReader = context.Context.XmlReader;
            if (xmlReader is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml reader is required");
            }

            var xmlName = xmlReader.LocalName;

            var propertyTypeToDeserialize = memberValue.MemberType;

            // #1642: make sure to get all the known types
            var knownTypesHashSet = context.Context.KnownTypes;
            var knownTypes = _dataContractSerializerFactory.GetKnownTypes(modelType, propertyTypeToDeserialize, knownTypesHashSet.ToList());
            knownTypesHashSet.AddRange(knownTypes);

            var isNullAttributeValue = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlIsNull);
            var isNull = !string.IsNullOrWhiteSpace(isNullAttributeValue) ? StringToObjectHelper.ToBool(isNullAttributeValue) : false;
            if (isNull)
            {
                // Enforce read so we move to the next thing
                xmlReader.Read();

                return null;
            }

            // Fix for CTL-555, note that we'll use this method at the end of the method, once we've read the model
            var graphIdAttributeValue = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlGraphId);

            var graphRefIdAttributeValue = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlGraphRefId);
            if (!string.IsNullOrWhiteSpace(graphRefIdAttributeValue))
            {
                var graphId = int.Parse(graphRefIdAttributeValue);

                var referenceManager = context.ReferenceManager;
                var referenceInfo = referenceManager.GetInfoById(graphId);
                if (referenceInfo is null)
                {
                    Log.Error($"Expected to find graph object with id '{graphId.ToString()}' in ReferenceManager, but it was not found. Defaulting value for member '{xmlName}' to null");
                    return null;
                }

                // Enforce read so we move to the next thing
                xmlReader.Read();

                return referenceInfo.Instance;
            }

            var typeAttributeValue = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlType);
            if (!string.IsNullOrEmpty(typeAttributeValue))
            {
                var typeToDeserialize = TypeCache.GetTypeWithoutAssembly(typeAttributeValue, allowInitialization: false);
                if (typeToDeserialize is not null && propertyTypeToDeserialize != typeToDeserialize)
                {
                    //Log.Debug($"Property type for property '{memberValue.Name}' is '{memberValue.MemberType.FullName}' but found type info that it should be deserialized as '{typeAttributeValue}'");

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
                memberValue.Value = xmlReader.ReadElementContentAsString();

                var parsedValue = DeserializeUsingObjectParse(context, memberValue);
                if (parsedValue is not null)
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
                    throw Log.ErrorAndCreateException<NotSupportedException>($"Cannot deserialize type '{propertyTypeToDeserialize.GetSafeFullName(false)}', it should implement IList in order to be deserialized");
                }

                var realCollectionType = collection.GetType();
                var childElementType = realCollectionType.GetCollectionElementType();
                if (childElementType is null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>($"Cannot deserialize type '{propertyTypeToDeserialize.GetSafeFullName(false)}', could not determine the element type of the collection");
                }

                var serializer = GetDataContractSerializer(context, propertyTypeToDeserialize, childElementType, xmlName);

                // Only read collection nodes when there are nodes available
                if (!xmlReader.IsEmptyElement)
                {
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

                        object? childValue = null;

                        // Step 1: check for graph attributes
                        var collectionItemGraphRefIdAttribute = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlGraphRefId);
                        var collectionItemGraphIdAttribute = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlGraphId);

                        if (!string.IsNullOrWhiteSpace(collectionItemGraphRefIdAttribute))
                        {
                            var graphId = int.Parse(collectionItemGraphRefIdAttribute);

                            var referenceManager = context.ReferenceManager;
                            var referenceInfo = referenceManager.GetInfoById(graphId);
                            if (referenceInfo is null)
                            {
                                Log.Error("Expected to find graph object with id '{0}' in ReferenceManager, but it was not found. Defaulting value for member '{1}' to null", graphId.ToString(), xmlName);
                            }
                            else
                            {
                                childValue = referenceInfo?.Instance;

                                // Enforce read so we move to the next thing
                                xmlReader.Read();
                            }
                        }

                        // Step 2: deserialize anyway
                        if (childValue is null)
                        {
                            childValue = ReadXmlObject(context, xmlReader, serializer, namespacePrefix, xmlName, modelType);
                        }

                        if (childValue is not null)
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
                }

                value = collection;

                // Exit the collection
                xmlReader.Read();

                isDeserialized = true;
            }

            // Fallback to .net serialization
            if (!isDeserialized)
            {
                // Try to be super fast, otherwise fall back to DataContractSerializer
                if (propertyTypeToDeserialize == typeof(string))
                {
                    value = xmlReader.ReadElementContentAsString();

                    isDeserialized = true;
                }
                else if (propertyTypeToDeserialize.IsPrimitiveEx())
                {
                    // Note: verified the source code of XmlReader, it reads as string, then uses
                    // XmlConvert to convert to the right type, so the performance should be similar
                    var stringValue = xmlReader.ReadElementContentAsString();
                    if (!string.IsNullOrWhiteSpace(stringValue))
                    {
                        value = StringToObjectHelper.ToRightType(propertyTypeToDeserialize, stringValue);

                        isDeserialized = true;
                    }
                }

                if (!isDeserialized)
                {
                    var serializer = GetDataContractSerializer(context, modelType, propertyTypeToDeserialize, xmlName);

                    value = ReadXmlObject(context, xmlReader, serializer, namespacePrefix, xmlName, modelType);

                    isDeserialized = true;
                }
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

        protected virtual object? ReadXmlObject(ISerializationContext<XmlSerializationContextInfo> context, XmlReader xmlReader,
            DataContractSerializer serializer, string namespacePrefix, string xmlName, Type modelType)
        {
            object? value = null;

            // Special case if we have an abstract item, we might have a specific type specified
            var typeName = GetSpecialAttributeValue(xmlReader, namespacePrefix, XmlType);
            if (!string.IsNullOrEmpty(typeName))
            {
                var type = TypeCache.GetType(typeName);
                if (type is null)
                {
                    Log.Warning($"Could not find type '{typeName}', deserialization will probably fail");
                }
                else
                {
                    var tempSerializer = GetDataContractSerializer(context, modelType, type, xmlName);
                    value = tempSerializer.ReadObject(xmlReader, false);
                }
            }

            if (value is null)
            {
                // Fallback to default deserialization, will fail for abstract types
                value = serializer.ReadObject(xmlReader, false);
            }

            return value;
        }

        private string? GetSpecialAttributeValue(XmlReader xmlReader, string namespacePrefix, string attributeName)
        {
            // Backwards compatibility
            return xmlReader.GetAttribute($"{namespacePrefix}:{attributeName}") ?? xmlReader.GetAttribute(attributeName);
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
            if (xmlWriter is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml writer is required");
            }

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

            var xmlWriter = context.Context.XmlWriter;
            if (xmlWriter is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml writer is required");
            }

            if (memberValue.Value is null)
            {
                xmlWriter.WriteStartElement(elementName);
                xmlWriter.WriteAttributeString(namespacePrefix, XmlIsNull, null, "true");
                xmlWriter.WriteEndElement();
            }
            else
            {
                var memberTypeToSerialize = memberValue.GetBestMemberType();

                var referenceManager = context.ReferenceManager;
                ReferenceInfo? referenceInfo = null;
                var serializeElement = true;

                if (memberValue.MemberGroup != SerializationMemberGroup.Collection)
                {
                    if (memberTypeToSerialize.IsClassType())
                    {
                        referenceInfo = referenceManager.GetInfo(memberValue.Value, true);
                        if (referenceInfo is not null)
                        {
                            if (WriteXmlElementAsGraphReference(xmlWriter, referenceInfo, memberTypeToSerialize, elementName, namespacePrefix))
                            {
                                serializeElement = false;
                            }
                        }
                    }
                }

                if (serializeElement)
                {
                    // Sometimes we are very late with the document start (e.g. when serialization a dictionary), but at this stage we should really
                    // force the document start
                    var writeElementWrapper = !WriteDocumentStartIfRequired(context, true);
                    if (writeElementWrapper)
                    {
                        xmlWriter.WriteStartElement(elementName);
                    }

                    AddObjectMetadata(xmlWriter, memberTypeToSerialize, memberValue.MemberType, referenceInfo, namespacePrefix);

                    // In special cases, we need to write our own collection items. One case is where a custom ModelBase
                    // implements IList and gets inside a StackOverflow
                    var serialized = false;
                    if (ShouldSerializeModelAsCollection(memberValue.GetBestMemberType()))
                    {
                        var collection = memberValue.Value as IEnumerable;
                        if (collection is not null)
                        {
                            if (modelType.IsArrayEx())
                            {
                                // Get array specific serializer
                                memberTypeToSerialize = modelType.GetElementTypeEx() ?? typeof(object);
                            }

                            var collectionElementType = typeof(object);

                            if (memberTypeToSerialize.IsGenericTypeEx())
                            {
                                collectionElementType = memberTypeToSerialize.GetGenericArgumentsEx().FirstOrDefault() ?? typeof(object);
                            }

                            var originalSerializer = GetDataContractSerializer(context, modelType, memberTypeToSerialize, elementName);
                            var actualSerializer = originalSerializer;
                            var actualSerializerElementType = collectionElementType;

                            foreach (var item in collection)
                            {
                                var itemType = item.GetType();
                                if (itemType != actualSerializerElementType)
                                {
                                    // Get a new serializer, and cache it as long as the item type is the same
                                    actualSerializer = GetDataContractSerializer(context, modelType, itemType, elementName);
                                    actualSerializerElementType = itemType;
                                }

                                var subItemElementName = GetXmlElementName(itemType, item, null);
                                referenceInfo = referenceManager.GetInfo(item, true);

                                if (!WriteXmlElementAsGraphReference(xmlWriter, referenceInfo, itemType, subItemElementName, namespacePrefix))
                                {
                                    xmlWriter.WriteStartElement(subItemElementName);

                                    AddObjectMetadata(xmlWriter, itemType, collectionElementType, referenceInfo, namespacePrefix);

                                    actualSerializer.WriteObjectContent(xmlWriter, item);

                                    xmlWriter.WriteEndElement();
                                }
                            }

                            serialized = true;
                        }
                    }

                    if (!serialized)
                    {
                        // Try to be super fast, otherwise fall back to .net serializer (DataContractSerializer)
                        if (memberTypeToSerialize == typeof(string))
                        {
                            var stringValue = memberValue.Value as string;
                            if (!string.IsNullOrEmpty(stringValue))
                            {
                                xmlWriter.WriteString(stringValue);
                            }
                        }
                        else if (memberTypeToSerialize.IsPrimitiveEx())
                        {
                            // We won't handle custom structs ourselves
                            var stringValue = ObjectToStringHelper.ToString(memberValue.Value);
                            xmlWriter.WriteString(stringValue);
                        }
                        else
                        {
                            var serializer = GetDataContractSerializer(context, modelType, memberTypeToSerialize, elementName);
                            serializer.WriteObjectContent(xmlWriter, memberValue.Value);
                        }
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }

        private bool WriteDocumentStartIfRequired(ISerializationContext<XmlSerializationContextInfo> context, bool skipCollectionCheck = false)
        {
            var xmlWriter = context.Context.XmlWriter;
            if (xmlWriter is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Xml writer is required");
            }

            if (xmlWriter.WriteState != WriteState.Start)
            {
                return false;
            }

            // Collections are special, don't write document start if this is a root collection (and otherwise we already have a document start)
            if (!skipCollectionCheck && ShouldSerializeAsCollection(context.ModelType))
            {
                return false;
            }

            var rootName = GetObjectRootName(context);

            xmlWriter.WriteStartElement(rootName);

            EnsureNamespaceInXmlWriter(context, xmlWriter, null);

            AddReferenceId(context, context.Model);

            return true;
        }

        private string GetObjectRootName(ISerializationContext<XmlSerializationContextInfo> context)
        {
            var model = context.Model;
            var rootName = "root";

            if (model is not null)
            {
                rootName = _rootNameCache.GetFromCacheOrFetch(context.ModelType, () =>
                {
                    return GetXmlElementName(context.ModelType, model, null);
                });
            }

            return rootName;
        }

        private void AddObjectMetadata(XmlWriter xmlWriter, Type? memberTypeToSerialize, Type? actualMemberType,
            ReferenceInfo? referenceInfo, string namespacePrefix)
        {
            if (referenceInfo is not null)
            {
                xmlWriter.WriteAttributeString(namespacePrefix, XmlGraphId, null, referenceInfo.Id.ToString());
            }

            if (memberTypeToSerialize is not null)
            {
                if (memberTypeToSerialize != actualMemberType)
                {
                    var memberTypeToSerializerName = TypeHelper.GetTypeName(memberTypeToSerialize.FullName ?? memberTypeToSerialize.Name);
                    xmlWriter.WriteAttributeString(namespacePrefix, XmlType, null, memberTypeToSerializerName);
                }
            }
        }

        private bool WriteXmlElementAsGraphReference(XmlWriter xmlWriter, ReferenceInfo? referenceInfo, Type memberTypeToSerialize, string elementName, string namespacePrefix)
        {
            if (referenceInfo is null)
            {
                return false;
            }

            var isClassType = memberTypeToSerialize.IsClassType();
            if (!isClassType)
            {
                return false;
            }

            if (!referenceInfo.IsFirstUsage)
            {
                // Note: we don't want to call GetSafeFullName if we don't have to
                //if (LogManager.IsDebugEnabled ?? false)
                //{
                //    Log.Debug($"Existing reference detected for element type '{memberTypeToSerialize.GetSafeFullName(false)}' with id '{referenceInfo.Id}', only storing id");
                //}

                xmlWriter.WriteStartElement(elementName);
                xmlWriter.WriteAttributeString(namespacePrefix, XmlGraphRefId, null, referenceInfo.Id.ToString());
                xmlWriter.WriteEndElement();

                return true;
            }

            return false;
        }

        private DataContractSerializer GetDataContractSerializer(ISerializationContext<XmlSerializationContextInfo> context,
            Type propertyTypeToDeserialize, Type childElementType, string xmlName)
        {
            var additionalKnownTypes = context.Context.KnownTypes;

            var serializer = _dataContractSerializerFactory.GetDataContractSerializer(propertyTypeToDeserialize, childElementType, xmlName, null, additionalKnownTypes.ToList());

            // We might have added more known types in the serializer
            additionalKnownTypes.AddRange(serializer.KnownTypes);

            return serializer;
        }

        /// <summary>
        /// Adds the reference unique identifier as attribute.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        private void AddReferenceId(ISerializationContext<XmlSerializationContextInfo> context, object model)
        {
            var xmlWriter = context.Context.XmlWriter!;
            var referenceManager = context.ReferenceManager;
            var referenceInfo = referenceManager.GetInfo(model, true);
            var namespacePrefix = GetNamespacePrefix();

            AddObjectMetadata(xmlWriter, null, null, referenceInfo, namespacePrefix);
        }

        /// <summary>
        /// Ensures the catel namespace in the xml document.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="xmlWriter">The xml writer.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        private void EnsureNamespaceInXmlWriter(ISerializationContext<XmlSerializationContextInfo> context, XmlWriter xmlWriter, XmlNamespace? xmlNamespace)
        {
            var catelNamespacePrefix = GetNamespacePrefix();
            var catelNamespaceUrl = GetNamespaceUrl();

            xmlWriter.WriteAttributeString(catelNamespacePrefix, "http://www.w3.org/2000/xmlns/", catelNamespaceUrl);

            if (!context.ModelType.IsCollection() && !context.ModelType.IsArrayEx())
            {
                // Only write when this is not a collection, otherwise the internal DataContractSerializer will
                // throw an exception about "'xmlns:i' is a duplicate attribute name.". There is a downside that
                // the i: namespace will be written several times, but that's better than not having objects
                // serialized (due to the exception)
                xmlWriter.WriteAttributeString("i", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");
            }

            if (xmlNamespace is not null)
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
    }
}
