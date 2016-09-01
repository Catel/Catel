// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Json
{
    using System;
    using System.Globalization;
    using Data;
    using Newtonsoft.Json;

    /// <summary>
    /// Interface for the binary serializer.
    /// </summary>
    public interface IJsonSerializer : ISerializer
    {
        /// <summary>
        /// Gets or sets a value indicating whether references should be preserved.
        /// <para />
        /// This will add additional <c>$graphid</c> and <c>$graphrefid</c> properties to each json object.
        /// </summary>
        /// <value><c>true</c> if references should be preserved; otherwise, <c>false</c>.</value>
        bool PreserveReferences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether type information should be written to the json output.
        /// </summary>
        /// <value><c>true</c> if type info should be written; otherwise, <c>false</c>.</value>
        bool WriteTypeInfo { get; set; }

        /// <summary>
        /// Serializes the specified model to the json writer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonWriter">The json writer.</param>
        [ObsoleteEx(ReplacementTypeOrMember = "Serialize(object, JsonWriter, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.0")]
        void Serialize(object model, JsonWriter jsonWriter);

        /// <summary>
        /// Serializes the specified model to the json writer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonWriter">The json writer.</param>
        /// <param name="configuration">The configuration.</param>
        void Serialize(object model, JsonWriter jsonWriter, ISerializationConfiguration configuration);

        /// <summary>
        /// Deserializes the specified model from the json reader.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <returns>
        /// ModelBase.
        /// </returns>
        [ObsoleteEx(ReplacementTypeOrMember = "Deserialize(object, JsonWriter, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.0")]
        object Deserialize(Type modelType, JsonReader jsonReader);

        /// <summary>
        /// Deserializes the specified model from the json reader.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="jsonReader">The json reader.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// ModelBase.
        /// </returns>
        object Deserialize(Type modelType, JsonReader jsonReader, ISerializationConfiguration configuration);
    }
}