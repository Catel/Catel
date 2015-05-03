// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Json
{
    using System;
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
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        public JsonSerializer(ISerializationManager serializationManager, ITypeFactory typeFactory)
            : base(serializationManager, typeFactory)
        {
            SupportCircularReferences = true;
            WriteTypeInfo = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether circular references should be supported.
        /// <para />
        /// This will add additional <c>$graphid</c> and <c>$graphrefid</c> properties to each json object.
        /// </summary>
        /// <value><c>true</c> if circular references should be supported; otherwise, <c>false</c>.</value>
        public bool SupportCircularReferences { get; set; }

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
        public void Serialize(ModelBase model, JsonWriter jsonWriter)
        {
            using (var context = GetContext(model, null, jsonWriter, SerializationContextMode.Serialization, null))
            {
                base.Serialize(model, context.Context);
            }
        }

        /// <summary>
        /// Deserializes the specified model from the json reader.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <returns>ModelBase.</returns>
        public ModelBase Deserialize(Type modelType, JsonReader jsonReader)
        {
            var jsonObject = JObject.Load(jsonReader);
            var jsonProperties = jsonObject.Properties().ToList();

            if (SupportCircularReferences)
            {
                var graphRefIdProperty = (from property in jsonProperties
                                          where string.Equals(property.Name, GraphRefId)
                                          select property).FirstOrDefault();
                if (graphRefIdProperty != null)
                {
                    var graphId = (int)graphRefIdProperty.Value;

                    var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
                    using (var scopeManager = ScopeManager<ReferenceManager>.GetScopeManager(scopeName))
                    {
                        var referenceManager = scopeManager.ScopeObject;

                        var referenceInfo = referenceManager.GetInfoById(graphId);
                        if (referenceInfo != null)
                        {
                            return (ModelBase)referenceInfo.Instance;
                        }
                    }
                }
            }

            var typeNameProperty = (from property in jsonProperties
                                    where string.Equals(property.Name, TypeName)
                                    select property).FirstOrDefault();
            if (typeNameProperty != null)
            {
                var modelTypeOverrideValue = (string) typeNameProperty.Value;
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

            var model = (ModelBase)TypeFactory.CreateInstance(modelType);

            using (var context = GetContext(model, jsonReader, null, SerializationContextMode.Deserialization, jsonProperties))
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
            jsonWriter.WriteStartObject();

            if (SupportCircularReferences)
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
            jsonWriter.WriteEndObject();
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

            jsonWriter.WritePropertyName(memberValue.Name);
            jsonSerializer.Serialize(jsonWriter, memberValue.Value);
        }

        /// <summary>
        /// Befores the deserialization.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeDeserialization(ISerializationContext<JsonSerializationContextInfo> context)
        {
            var serializationContext = context.Context;
            if (serializationContext.JsonProperties == null)
            {
                var jsonObject = JObject.Load(context.Context.JsonReader);
                context.Context.JsonProperties = jsonObject.Properties().ToList();
            }

            if (SupportCircularReferences)
            {
                var properties = serializationContext.JsonProperties;
                var graphIdProperty = (from property in properties
                                       where string.Equals(property.Name, GraphId)
                                       select property).FirstOrDefault();
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

            var jsonProperty = jsonProperties.FirstOrDefault(x => string.Equals(x.Name, memberValue.Name));
            if (jsonProperty != null)
            {
                var jsonValue = jsonProperty.Value;
                if (jsonValue != null)
                {
                    object finalMemberValue = null;
                    var valueType = memberValue.Type;

                    if (memberValue.Type.IsEnumEx())
                    {
                        var enumName = Enum.GetName(valueType, (int)jsonValue);
                        if (!string.IsNullOrWhiteSpace(enumName))
                        {
                            finalMemberValue = Enum.Parse(valueType, enumName, false);
                        }
                    }
                    // TODO: support other types?
                    else
                    {
                        finalMemberValue = jsonValue.ToObject(valueType, serializationContext.JsonSerializer);
                    }

                    if (finalMemberValue != null)
                    {
                        return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, finalMemberValue);
                    }
                }
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
        protected override ISerializationContext<JsonSerializationContextInfo> GetContext(ModelBase model, Stream stream, SerializationContextMode contextMode)
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

            return GetContext(model, jsonReader, jsonWriter, contextMode, null);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <param name="jsonWriter">The json writer.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="jsonProperties">The json properties.</param>
        /// <returns>ISerializationContext&lt;JsonSerializationContextInfo&gt;.</returns>
        protected virtual ISerializationContext<JsonSerializationContextInfo> GetContext(ModelBase model, JsonReader jsonReader, JsonWriter jsonWriter, SerializationContextMode contextMode, IEnumerable<JProperty> jsonProperties)
        {
            var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            jsonSerializer.ContractResolver = new CatelJsonContractResolver();
            jsonSerializer.Converters.Add(new CatelJsonConverter(this));

            var contextInfo = new JsonSerializationContextInfo(jsonSerializer, jsonReader, jsonWriter);
            if (jsonProperties != null)
            {
                contextInfo.JsonProperties = jsonProperties.ToList();
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