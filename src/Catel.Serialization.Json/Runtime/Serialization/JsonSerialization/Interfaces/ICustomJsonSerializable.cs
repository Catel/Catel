// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomJsonSerializable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Json
{
    using Newtonsoft.Json;

    /// <summary>
    /// Allows a type to implement their own (de)serialization mechanism.
    /// </summary>
    public interface ICustomJsonSerializable
    {
        /// <summary>
        /// Serializes the object to the specified json writer.
        /// </summary>
        /// <param name="jsonWriter">The json writer.</param>
        void Serialize(JsonWriter jsonWriter);

        /// <summary>
        /// Serializes the object from the specified json reader.
        /// <para />
        /// Note that the object is always constructed by the serialization engine, the
        /// object itself must read the values from the <paramref name="jsonReader"/>.
        /// </summary>
        /// <param name="jsonReader">The json reader.</param>
        void Deserialize(JsonReader jsonReader);
    }
}