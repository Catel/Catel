// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Json
{
    using System;
    using Data;
    using Newtonsoft.Json;

    /// <summary>
    /// Interface for the binary serializer.
    /// </summary>
    public interface IJsonSerializer : IModelBaseSerializer<JsonSerializationContextInfo>
    {
        /// <summary>
        /// Serializes the specified model to the json writer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonWriter">The json writer.</param>
        void Serialize(ModelBase model, JsonWriter jsonWriter);

        /// <summary>
        /// Deserializes the specified model from the json reader.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <returns>ModelBase.</returns>
        ModelBase Deserialize(Type modelType, JsonReader jsonReader);
    }
}