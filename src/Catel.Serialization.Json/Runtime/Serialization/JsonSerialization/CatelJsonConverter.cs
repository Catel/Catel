// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelJsonConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Json
{
    using System;
    using Data;
    using Newtonsoft.Json;
    using Reflection;
    using Scoping;

    /// <summary>
    /// Converts Catel models manually using the Catel serializer.
    /// </summary>
    public class CatelJsonConverter : JsonConverter
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ISerializationConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatelJsonConverter" /> class.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="configuration">The configuration.</param>
        public CatelJsonConverter(IJsonSerializer jsonSerializer, ISerializationConfiguration configuration)
        {
            _jsonSerializer = jsonSerializer;
            _configuration = configuration;
        }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var serialize = true;

            if (_jsonSerializer.PreserveReferences)
            {
                var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
                using (var scopeManager = ScopeManager<ReferenceManager>.GetScopeManager(scopeName))
                {
                    var referenceManager = scopeManager.ScopeObject;

                    var referenceInfo = referenceManager.GetInfo(value);
                    if (referenceInfo != null && !referenceInfo.IsFirstUsage)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName(Json.JsonSerializer.GraphRefId);
                        writer.WriteValue(referenceInfo.Id);
                        writer.WriteEndObject();

                        serialize = false;
                    }
                }
            }

            if (serialize)
            {
                _jsonSerializer.Serialize((ModelBase)value, writer, _configuration);
            }
        }

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>System.Object.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var obj = _jsonSerializer.Deserialize(objectType, reader, _configuration);
            return obj;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            var canConvert = objectType.IsModelBase();
            return canConvert;
        }
    }
}