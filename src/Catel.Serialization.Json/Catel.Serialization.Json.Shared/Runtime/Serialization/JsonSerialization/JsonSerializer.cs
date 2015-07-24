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
            using (var context = GetContext(model, null, jsonWriter, SerializationContextMode.Serialization, null, null))
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

            if (modelType.IsCollection())
            {
                jsonArray = JArray.Load(jsonReader);
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

            var model = TypeFactory.CreateInstance(modelType);

            using (var context = GetContext(model, jsonReader, null, SerializationContextMode.Deserialization, jsonProperties, jsonArray))
            {
                base.Deserialize(model, context.Context);
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

            if (context.ModelType.IsCollection())
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

                if (WriteTypeInfo)
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

            if (context.ModelType.IsCollection())
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
            var isRootCollection = (context.Depth == 0 && ShouldSerializeAsCollection(memberValue));
            if (!isRootCollection)
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

                            var idPropertyName = string.Format("${0}_{1}", memberValue.Name, referenceInfo.IsFirstUsage ? GraphId : GraphRefId);

                            jsonWriter.WritePropertyName(idPropertyName);
                            jsonSerializer.Serialize(jsonWriter, referenceInfo.Id);

                            if (!referenceInfo.IsFirstUsage)
                            {
                                return;
                            }
                        }
                    }
                }

                jsonWriter.WritePropertyName(memberValue.Name);
            }

            if (ReferenceEquals(memberValue.Value, null) || ShouldExternalSerializerHandleMember(memberValue))
            {
                jsonSerializer.Serialize(jsonWriter, memberValue.Value);
            }
            else if (ShouldSerializeAsCollection(memberValue))
            {
                jsonWriter.WriteStartArray();

                foreach (var item in (IList)memberValue.Value)
                {
                    Serialize(item, jsonWriter);
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
            if (context.ModelType.IsCollection())
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
                    var graphRefIdPropertyName = string.Format("${0}_{1}", memberValue.Name, GraphRefId);
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

                if (jsonProperties.ContainsKey(memberValue.Name))
                {
                    var jsonProperty = jsonProperties[memberValue.Name];
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
                                    // Serialize ourselves
                                    finalMemberValue = Deserialize(valueType, serializationContext.JsonReader);
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
                            if (PreserveReferences)
                            {
                                var graphIdPropertyName = string.Format("${0}_{1}", memberValue.Name, GraphId);
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

            if (ShouldSerializeAsCollection(memberValue))
            {
                var collection = (IList)Activator.CreateInstance(memberValue.MemberType);

                var jArray = context.Context.JsonArray;
                if (jArray != null)
                {
                    var collectionItemType = memberValue.MemberType.GetGenericArgumentsEx()[0];

                    foreach (var item in jArray.Children())
                    {
                        var deserializedItem = Deserialize(collectionItemType, item.CreateReader());
                        collection.Add(deserializedItem);
                    }

                    memberValue.Value = collection;
                }

                return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, collection);
            }

            return SerializationObject.FailedToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>ISerializationContext{SerializationInfo}.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">contextMode</exception>
        protected override ISerializationContext<JsonSerializationContextInfo> GetContext(object model, Stream stream, SerializationContextMode contextMode)
        {
            JsonReader jsonReader = null;
            JsonWriter jsonWriter = null;

            switch (contextMode)
            {
                case SerializationContextMode.Serialization:
                    jsonWriter = new JsonTextWriter(new StreamWriter(stream));
                    break;

                case SerializationContextMode.Deserialization:
                    jsonReader = new JsonTextReader(new StreamReader(stream));
                    break;

                default:
                    throw new ArgumentOutOfRangeException("contextMode");
            }

            return GetContext(model, jsonReader, jsonWriter, contextMode, null, null);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <param name="jsonWriter">The json writer.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="jsonProperties">The json properties.</param>
        /// <param name="jsonArray">The json array.</param>
        /// <returns>ISerializationContext&lt;JsonSerializationContextInfo&gt;.</returns>
        protected virtual ISerializationContext<JsonSerializationContextInfo> GetContext(object model, JsonReader jsonReader, JsonWriter jsonWriter, SerializationContextMode contextMode,
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

            var context = new SerializationContext<JsonSerializationContextInfo>(model, contextInfo, contextMode);
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