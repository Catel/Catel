// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Data;
    using IoC;
    using JsonSerialization;
    using Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Reflection;
    using Scoping;

    /// <summary>
    /// The binary serializer.
    /// </summary>
    public class JsonSerializer : SerializerBase<JsonSerializationContextInfo>, IJsonSerializer
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The graph identifier.
        /// </summary>
        public const string GraphId = "$graphid";

        /// <summary>
        /// The graph reference identifier.
        /// </summary>
        public const string GraphRefId = "$graphrefid";

        /// <summary>
        /// The type name.
        /// </summary>
        public const string TypeName = "$typename";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="objectAdapter">The object adapter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        public JsonSerializer(ISerializationManager serializationManager, ITypeFactory typeFactory, IObjectAdapter objectAdapter)
            : base(serializationManager, typeFactory, objectAdapter)
        {
            PreserveReferences = true;
            WriteTypeInfo = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether references should be preserved.
        /// <para />
        /// This will add additional <c>$graphid</c> and <c>$graphrefid</c> properties to each json object.
        /// </summary>
        /// <value><c>true</c> if references should be preserved; otherwise, <c>false</c>.</value>
        public bool PreserveReferences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether type information should be written to the json output.
        /// </summary>
        /// <value><c>true</c> if type info should be written; otherwise, <c>false</c>.</value>
        public bool WriteTypeInfo { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the specified model to the json writer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonWriter">The json writer.</param>
        public void Serialize(object model, JsonWriter jsonWriter)
        {
            Argument.IsNotNull("model", model);

            using (var context = GetContext(model, model.GetType(), null, jsonWriter, SerializationContextMode.Serialization, null, null))
            {
                base.Serialize(model, context.Context);
            }
        }

        /// <summary>
        /// Deserializes the specified model from the json reader.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <returns>The model.</returns>
        public object Deserialize(Type modelType, JsonReader jsonReader)
        {
            Dictionary<string, JProperty> jsonProperties = null;
            JArray jsonArray = null;

            if (ShouldSerializeAsCollection(modelType, null))
            {
                jsonArray = JArray.Load(jsonReader);
            }
            else if (ShouldExternalSerializerHandleMember(modelType, null))
            {
                return jsonReader.Value;
            }
            else
            {
                var jsonObject = JObject.Load(jsonReader);
                jsonProperties = jsonObject.Properties().ToDictionary(x => x.Name, x => x);

                if (PreserveReferences)
                {
                    if (jsonProperties.ContainsKey(GraphRefId))
                    {
                        var graphId = (int)jsonProperties[GraphRefId].Value;

                        var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
                        using (var scopeManager = ScopeManager<ReferenceManager>.GetScopeManager(scopeName))
                        {
                            var referenceManager = scopeManager.ScopeObject;

                            var referenceInfo = referenceManager.GetInfoById(graphId);
                            if (referenceInfo != null)
                            {
                                return referenceInfo.Instance;
                            }
                        }
                    }
                }

                if (jsonProperties.ContainsKey(TypeName))
                {
                    var modelTypeOverrideValue = (string)jsonProperties[TypeName].Value;
                    var modelTypeOverride = TypeCache.GetTypeWithoutAssembly(modelTypeOverrideValue);
                    if (modelTypeOverride == null)
                    {
                        Log.Warning("Object was serialized as '{0}', but the type is not available. Using original type '{1}'", modelTypeOverrideValue, modelType.GetSafeFullName());
                    }
                    else
                    {
                        modelType = modelTypeOverride;
                    }
                }
            }

            var model = CreateModelInstance(modelType);

            using (var context = GetContext(model, modelType, jsonReader, null, SerializationContextMode.Deserialization, jsonProperties, jsonArray))
            {
                model = base.Deserialize(model, context.Context);
            }

            return model;
        }

        /// <summary>
        /// Warms up the specified type.
        /// </summary>
        /// <param name="type">The type to warmup.</param>
        protected override void Warmup(Type type)
        {
            // No additional warmup required by the json serializer
        }

        /// <summary>
        /// Befores the serialization.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeSerialization(ISerializationContext<JsonSerializationContextInfo> context)
        {
            var jsonWriter = context.Context.JsonWriter;

            if (ShouldSerializeAsCollection(context.ModelType, context.Model))
            {
                // Nothing required
            }
            else
            {
                jsonWriter.WriteStartObject();

                if (PreserveReferences)
                {
                    var referenceManager = context.ReferenceManager;
                    var referenceInfo = referenceManager.GetInfo(context.Model);

                    jsonWriter.WritePropertyName(GraphId);
                    jsonWriter.WriteValue(referenceInfo.Id);
                }

                if (WriteTypeInfo && context.ModelType != typeof(SerializableKeyValuePair))
                {
                    jsonWriter.WritePropertyName(TypeName);
                    jsonWriter.WriteValue(context.ModelType.GetSafeFullName());
                }
            }

            base.BeforeSerialization(context);
        }

        /// <summary>
        /// Afters the serialization.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void AfterSerialization(ISerializationContext<JsonSerializationContextInfo> context)
        {
            base.AfterSerialization(context);

            var jsonWriter = context.Context.JsonWriter;

            if (ShouldSerializeAsCollection(context.ModelType, context.Model))
            {
                // Nothing required
            }
            else
            {
                jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Serializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected override void SerializeMember(ISerializationContext<JsonSerializationContextInfo> context, MemberValue memberValue)
        {
            var serializationContext = context.Context;
            var jsonSerializer = serializationContext.JsonSerializer;
            var jsonWriter = serializationContext.JsonWriter;

            // Only write property names when this is not the root and not a collection
            var isRootDictionary = IsRootDictionary(context, memberValue);
            var isRootCollection = IsRootCollection(context, memberValue);

            if (!isRootDictionary && !isRootCollection)
            {
                // Write reference id *before* serializing, otherwise we might get into a circular loop
                if (PreserveReferences)
                {
                    var value = memberValue.Value;
                    if (value != null)
                    {
                        // Ignore basic types (value types, strings, etc) and ModelBase (already gets the graph id written)
                        var memberType = memberValue.ActualMemberType;
                        if (!memberType.IsBasicType()) // && !(value is ModelBase))
                        {
                            var referenceManager = context.ReferenceManager;
                            var referenceInfo = referenceManager.GetInfo(memberValue.Value);

                            var idPropertyName = string.Format("${0}_{1}", memberValue.NameForSerialization, referenceInfo.IsFirstUsage ? GraphId : GraphRefId);

                            jsonWriter.WritePropertyName(idPropertyName);
                            jsonSerializer.Serialize(jsonWriter, referenceInfo.Id);

                            if (!referenceInfo.IsFirstUsage)
                            {
                                return;
                            }
                        }
                    }
                }

                jsonWriter.WritePropertyName(memberValue.NameForSerialization);
            }

            if (ReferenceEquals(memberValue.Value, null) || ShouldExternalSerializerHandleMember(memberValue))
            {
                jsonSerializer.Serialize(jsonWriter, memberValue.Value);
            }
            else if (ShouldSerializeAsDictionary(memberValue))
            {
                // Serialize as an object with properties
                var sourceDictionary = memberValue.Value as IDictionary;
                if (sourceDictionary != null)
                {
                    jsonWriter.WriteStartObject();

                    foreach (var key in sourceDictionary.Keys)
                    {
                        var stringKey = key as string;
                        if (stringKey == null)
                        {
                            stringKey = ObjectToStringHelper.ToString(key);
                        }

                        jsonWriter.WritePropertyName(stringKey);

                        var item = sourceDictionary[key];
                        if (item != null)
                        {
                            var itemType = item.GetType();
                            if (ShouldExternalSerializerHandleMember(itemType, null))
                            {
                                jsonSerializer.Serialize(jsonWriter, item);
                            }
                            else
                            {
                                Serialize(item, jsonWriter);
                            }
                        }
                    }

                    jsonWriter.WriteEndObject();
                }
            }
            else if (ShouldSerializeAsCollection(memberValue))
            {
                jsonWriter.WriteStartArray();

                foreach (var item in (IList)memberValue.Value)
                {
                    // Note: we don't support null values for now
                    if (item != null)
                    {
                        var itemType = item.GetType();
                        if (ShouldExternalSerializerHandleMember(itemType, null))
                        {
                            jsonSerializer.Serialize(jsonWriter, item);
                        }
                        else
                        {
                            Serialize(item, jsonWriter);
                        }
                    }
                }

                jsonWriter.WriteEndArray();
            }
            else
            {
                Serialize(memberValue.Value, jsonWriter);
            }
        }

        /// <summary>
        /// Befores the deserialization.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeDeserialization(ISerializationContext<JsonSerializationContextInfo> context)
        {
            var serializationContext = context.Context;
            if (ShouldSerializeAsDictionary(context.ModelType, context.Model))
            {
                if (serializationContext.JsonProperties == null)
                {
                    var jsonObject = JObject.Load(context.Context.JsonReader);
                    serializationContext.JsonProperties = jsonObject.Properties().ToDictionary(x => x.Name, x => x);
                }
            }
            else if (ShouldSerializeAsCollection(context.ModelType, context.Model))
            {
                if (serializationContext.JsonArray == null)
                {
                    var jsonArray = JArray.Load(context.Context.JsonReader);
                    serializationContext.JsonArray = jsonArray;
                }
            }
            else
            {
                if (serializationContext.JsonProperties == null)
                {
                    var jsonObject = JObject.Load(context.Context.JsonReader);
                    serializationContext.JsonProperties = jsonObject.Properties().ToDictionary(x => x.Name, x => x);
                }

                if (PreserveReferences)
                {
                    var properties = serializationContext.JsonProperties;
                    if (properties.ContainsKey(GraphId))
                    {
                        var graphIdProperty = properties[GraphId];
                        if (graphIdProperty != null)
                        {
                            var graphId = (int)graphIdProperty.Value;

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
                }
            }

            base.BeforeDeserialization(context);
        }

        /// <summary>
        /// Deserializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The <see cref="SerializationObject"/> representing the deserialized value or result.</returns>
        protected override SerializationObject DeserializeMember(ISerializationContext<JsonSerializationContextInfo> context, MemberValue memberValue)
        {
            var serializationContext = context.Context;

            var jsonProperties = serializationContext.JsonProperties;
            if (jsonProperties != null)
            {
                if (PreserveReferences)
                {
                    var graphRefIdPropertyName = string.Format("${0}_{1}", memberValue.NameForSerialization, GraphRefId);
                    if (jsonProperties.ContainsKey(graphRefIdPropertyName))
                    {
                        var graphId = (int)jsonProperties[graphRefIdPropertyName].Value;
                        var referenceManager = context.ReferenceManager;
                        var referenceInfo = referenceManager.GetInfoById(graphId);
                        if (referenceInfo == null)
                        {
                            Log.Error("Expected to find graph object with id '{0}' in ReferenceManager, but it was not found. Defaulting value for member '{1}' to null", graphId, memberValue.Name);
                            return null;
                        }

                        return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, referenceInfo.Instance);
                    }
                }

                if (memberValue.MemberGroup == SerializationMemberGroup.Dictionary)
                {
                    var dictionary = CreateModelInstance(memberValue.MemberType) as IDictionary;

                    var keyType = typeof(object);
                    var valueType = typeof(object);

                    if (memberValue.MemberType.IsGenericTypeEx())
                    {
                        var genericArguments = memberValue.MemberType.GetGenericArgumentsEx();
                        if (genericArguments.Length == 2)
                        {
                            keyType = genericArguments[0];
                            valueType = genericArguments[1];
                        }
                    }

                    foreach (var jsonPropertyKeyValuePair in jsonProperties)
                    {
                        var jsonProperty = jsonPropertyKeyValuePair.Value;

                        object deserializedItem = null;

                        object key = jsonProperty.Name;
                        if (keyType != typeof(object))
                        {
                            key = StringToObjectHelper.ToRightType(keyType, jsonProperty.Name);
                        }

                        var typeToDeserialize = valueType;
                        if (jsonProperty.Value != null)
                        {
                            if (jsonProperty.Value.Type != JTokenType.Object)
                            {
                                switch (jsonProperty.Value.Type)
                                {
                                    case JTokenType.Integer:
                                        typeToDeserialize = typeof(int);
                                        break;

                                    case JTokenType.Float:
                                        typeToDeserialize = typeof(float);
                                        break;

                                    case JTokenType.String:
                                        typeToDeserialize = typeof(string);
                                        break;

                                    case JTokenType.Boolean:
                                        typeToDeserialize = typeof(bool);
                                        break;

                                    case JTokenType.Date:
                                        typeToDeserialize = typeof(DateTime);
                                        break;

                                    case JTokenType.Guid:
                                        typeToDeserialize = typeof(Guid);
                                        break;

                                    case JTokenType.Uri:
                                        typeToDeserialize = typeof(Uri);
                                        break;

                                    case JTokenType.TimeSpan:
                                        typeToDeserialize = typeof(TimeSpan);
                                        break;
                                }
                            }
                        }

                        var shouldValueTypeBeHandledByExternalSerializer = ShouldExternalSerializerHandleMember(typeToDeserialize, null);
                        if (shouldValueTypeBeHandledByExternalSerializer)
                        {
                            deserializedItem = jsonProperty.Value.ToObject(valueType, serializationContext.JsonSerializer);
                        }
                        else
                        {
                            deserializedItem = Deserialize(valueType, jsonProperty.Value.CreateReader());
                        }

                        dictionary[key] = deserializedItem;
                    }

                    return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, dictionary);
                }

                if (jsonProperties.ContainsKey(memberValue.NameForSerialization))
                {
                    var jsonProperty = jsonProperties[memberValue.NameForSerialization];
                    var jsonValue = jsonProperty.Value;
                    if (jsonValue != null)
                    {
                        object finalMemberValue = null;
                        var valueType = memberValue.MemberType;
                        if (valueType.IsEnumEx())
                        {
                            var enumName = Enum.GetName(valueType, (int)jsonValue);
                            if (!string.IsNullOrWhiteSpace(enumName))
                            {
                                finalMemberValue = Enum.Parse(valueType, enumName, false);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (ShouldExternalSerializerHandleMember(memberValue))
                                {
                                    finalMemberValue = jsonValue.ToObject(valueType, serializationContext.JsonSerializer);
                                }
                                else if (ShouldSerializeAsCollection(memberValue))
                                {
                                    finalMemberValue = Deserialize(valueType, jsonProperty.Value.CreateReader());
                                }
                                else
                                {
                                    if (jsonValue.HasValues)
                                    {
                                        // Serialize ourselves
                                        finalMemberValue = Deserialize(valueType, jsonValue.CreateReader());
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // As a fallback, interpret as a string (might be a modifier)
                                finalMemberValue = (string)jsonValue;
                            }
                        }

                        if (finalMemberValue != null)
                        {
                            if (PreserveReferences && finalMemberValue.GetType().IsClassType())
                            {
                                var graphIdPropertyName = string.Format("${0}_{1}", memberValue.NameForSerialization, GraphId);
                                if (jsonProperties.ContainsKey(graphIdPropertyName))
                                {
                                    var graphId = (int)jsonProperties[graphIdPropertyName].Value;

                                    var referenceManager = context.ReferenceManager;
                                    referenceManager.RegisterManually(graphId, finalMemberValue);
                                }
                            }

                            return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, finalMemberValue);
                        }
                    }
                }
            }

            var shouldSerializeAsCollection = ShouldSerializeAsCollection(memberValue);
            if (shouldSerializeAsCollection)
            {
                var collection = new List<object>();

                var jArray = context.Context.JsonArray;
                if (jArray != null)
                {
                    var collectionItemType = typeof(object);
                    if (memberValue.MemberType.IsGenericTypeEx())
                    {
                        collectionItemType = memberValue.MemberType.GetGenericArgumentsEx()[0];
                    }

                    var shouldBeHandledByExternalSerializer = ShouldExternalSerializerHandleMember(collectionItemType, null);

                    foreach (var item in jArray.Children())
                    {
                        object deserializedItem = null;

                        if (shouldBeHandledByExternalSerializer)
                        {
                            deserializedItem = item.ToObject(collectionItemType, serializationContext.JsonSerializer);
                        }
                        else
                        {
                            deserializedItem = Deserialize(collectionItemType, item.CreateReader());
                        }

                        collection.Add(deserializedItem);
                    }
                }

                return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, collection);
            }

            return SerializationObject.FailedToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>ISerializationContext{SerializationInfo}.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">contextMode</exception>
        protected override ISerializationContext<JsonSerializationContextInfo> GetContext(object model, Type modelType, Stream stream, SerializationContextMode contextMode)
        {
            JsonReader jsonReader = null;
            JsonWriter jsonWriter = null;

            switch (contextMode)
            {
                case SerializationContextMode.Serialization:
                    jsonWriter = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8));
                    break;

                case SerializationContextMode.Deserialization:
                    jsonReader = new JsonTextReader(new StreamReader(stream, Encoding.UTF8));
                    break;

                default:
                    throw new ArgumentOutOfRangeException("contextMode");
            }

            return GetContext(model, modelType, jsonReader, jsonWriter, contextMode, null, null);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <param name="jsonWriter">The json writer.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="jsonProperties">The json properties.</param>
        /// <param name="jsonArray">The json array.</param>
        /// <returns>ISerializationContext&lt;JsonSerializationContextInfo&gt;.</returns>
        protected virtual ISerializationContext<JsonSerializationContextInfo> GetContext(object model, Type modelType, JsonReader jsonReader, JsonWriter jsonWriter, SerializationContextMode contextMode,
            Dictionary<string, JProperty> jsonProperties, JArray jsonArray)
        {
            var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            jsonSerializer.ContractResolver = new CatelJsonContractResolver();
            jsonSerializer.Converters.Add(new CatelJsonConverter(this));

            var contextInfo = new JsonSerializationContextInfo(jsonSerializer, jsonReader, jsonWriter);
            if (jsonProperties != null)
            {
                contextInfo.JsonProperties = jsonProperties;
            }

            if (jsonArray != null)
            {
                contextInfo.JsonArray = jsonArray;
            }

            var context = new SerializationContext<JsonSerializationContextInfo>(model, modelType, contextInfo, contextMode);
            return context;
        }

        /// <summary>
        /// Appends the context to stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        protected override void AppendContextToStream(ISerializationContext<JsonSerializationContextInfo> context, Stream stream)
        {
            var jsonWriter = context.Context.JsonWriter;
            if (jsonWriter != null)
            {
                jsonWriter.Flush();
            }
        }
        #endregion
    }
}