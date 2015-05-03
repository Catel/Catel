// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializationContextInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Json
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Class containing all information about the Json serialization context.
    /// </summary>
    public class JsonSerializationContextInfo
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <param name="jsonWriter">The json writer.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="jsonSerializer" /> is <c>null</c>.</exception>
        public JsonSerializationContextInfo(Newtonsoft.Json.JsonSerializer jsonSerializer, JsonReader jsonReader, JsonWriter jsonWriter)
        {
            Argument.IsNotNull("jsonSerializer", jsonSerializer);

            JsonSerializer = jsonSerializer;
            JsonReader = jsonReader;
            JsonWriter = jsonWriter;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the serialization info.
        /// </summary>
        /// <value>The serialization info.</value>
        public Newtonsoft.Json.JsonSerializer JsonSerializer { get; private set; }

        /// <summary>
        /// Gets the json reader.
        /// </summary>
        /// <value>The json reader.</value>
        public JsonReader JsonReader { get; private set; }

        /// <summary>
        /// Gets the json writer.
        /// </summary>
        /// <value>The json writer.</value>
        public JsonWriter JsonWriter { get; private set; }
        #endregion
    }
}