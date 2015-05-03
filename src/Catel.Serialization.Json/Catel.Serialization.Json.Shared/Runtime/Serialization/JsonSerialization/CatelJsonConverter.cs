// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelJsonConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.JsonSerialization
{
    using System;
    using Data;
    using IoC;
    using Json;
    using Newtonsoft.Json;
    using Reflection;
    using JsonSerializer = Newtonsoft.Json.JsonSerializer;

    /// <summary>
    /// Converts Catel models manually using the Catel serializer.
    /// </summary>
    public class CatelJsonConverter : JsonConverter
    {
        private readonly IJsonSerializer _jsonSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatelJsonConverter" /> class.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer.</param>
        public CatelJsonConverter(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            _jsonSerializer.Serialize((ModelBase)value, writer);
        }

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return _jsonSerializer.Deserialize(objectType, reader);
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof (ModelBase).IsAssignableFromEx(objectType);
        }
    }
}