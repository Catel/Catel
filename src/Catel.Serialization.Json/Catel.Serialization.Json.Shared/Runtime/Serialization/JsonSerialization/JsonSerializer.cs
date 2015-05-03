// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Json
{
    using System;
    using System.IO;
    using Data;
    using IoC;
    using JsonSerialization;
    using Logging;
    using Newtonsoft.Json;

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
        }
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the specified model to the json writer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonWriter">The json writer.</param>
        public void Serialize(ModelBase model, JsonWriter jsonWriter)
        {
            using (var context = GetContext(model, null, jsonWriter, SerializationContextMode.Serialization))
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
            // TODO: check if null

            var model = (ModelBase)TypeFactory.CreateInstance(modelType);

            using (var context = GetContext(model, jsonReader, null, SerializationContextMode.Deserialization))
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
            // TODO: read everything into memory

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
            var jsonReader = serializationContext.JsonReader;

            var finalMembervalue = jsonReader.ReadAsInt32();
            if (finalMembervalue != null)
            {
                return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, finalMembervalue);
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

            return GetContext(model, jsonReader, jsonWriter, contextMode);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <param name="jsonWriter">The json writer.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>ISerializationContext&lt;JsonSerializationContextInfo&gt;.</returns>
        protected virtual ISerializationContext<JsonSerializationContextInfo> GetContext(ModelBase model, JsonReader jsonReader, JsonWriter jsonWriter, SerializationContextMode contextMode)
        {
            var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            jsonSerializer.ContractResolver = new CatelJsonContractResolver();
            jsonSerializer.Converters.Add(new CatelJsonConverter(this));

            var contextInfo = new JsonSerializationContextInfo(jsonSerializer, jsonReader, jsonWriter);

            return new SerializationContext<JsonSerializationContextInfo>(model, contextInfo, contextMode);
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