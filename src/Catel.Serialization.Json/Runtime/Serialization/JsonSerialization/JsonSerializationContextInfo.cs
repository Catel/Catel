namespace Catel.Runtime.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Class containing all information about the Json serialization context.
    /// </summary>
    public class JsonSerializationContextInfo : SerializationContextInfoBase<JsonSerializationContextInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <param name="jsonWriter">The json writer.</param>
        public JsonSerializationContextInfo(Newtonsoft.Json.JsonSerializer jsonSerializer, JsonReader? jsonReader, JsonWriter? jsonWriter)
        {
            JsonSerializer = jsonSerializer;
            JsonReader = jsonReader;
            JsonWriter = jsonWriter;
        }

        /// <summary>
        /// Gets the serialization info.
        /// </summary>
        /// <value>The serialization info.</value>
        public Newtonsoft.Json.JsonSerializer JsonSerializer { get; private set; }

        /// <summary>
        /// Gets the json reader.
        /// </summary>
        /// <value>The json reader.</value>
        public JsonReader? JsonReader { get; private set; }

        /// <summary>
        /// Gets the json writer.
        /// </summary>
        /// <value>The json writer.</value>
        public JsonWriter? JsonWriter { get; private set; }

        /// <summary>
        /// Gets or sets the json array.
        /// </summary>
        /// <value>The json array.</value>
        public JArray? JsonArray { get; set; }

        /// <summary>
        /// Gets or sets the json properties used during deserialization.
        /// </summary>
        /// <value>The json object.</value>
        public Dictionary<string, JProperty>? JsonProperties { get; set; }
    }
}
