// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelBaseSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Data;

    /// <summary>
    /// Interface definition to serialize the <see cref="IModel"/>.
    /// </summary>
    public interface IModelBaseSerializer
    {
        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        void Serialize(ModelBase model, Stream stream);

        /// <summary>
        /// Serializes the members.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="membersToIgnore"/> is <c>null</c>.</exception>
        void SerializeMembers(ModelBase model, Stream stream, params string[] membersToIgnore);

        /// <summary>
        /// Gets the serializable members for the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        /// <returns>The list of members to serialize.</returns>
        List<MemberValue> GetSerializableMembers(ModelBase model, params string[] membersToIgnore);

        /// <summary>
        /// Deserializes the specified model. The deserialized values will be set in the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        void Deserialize(ModelBase model, Stream stream);

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized model.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        ModelBase Deserialize(Type modelType, Stream stream);

        /// <summary>
        /// Deserializes the members of the specified model.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The list of members that have been deserialized.</returns>
        List<MemberValue> DeserializeMembers(Type modelType, Stream stream);
    }

    /// <summary>
    /// Interface definition to serialize the <see cref="IModel"/>.
    /// </summary>
    public interface IModelBaseSerializer<TSerializationContext> : IModelBaseSerializer
        where TSerializationContext : class
    {
        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        void Serialize(ModelBase model, TSerializationContext serializationContext);

        /// <summary>
        /// Deserializes the specified model. The deserialized values will be set in the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        void Deserialize(ModelBase model, TSerializationContext serializationContext);

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <returns>The deserialized model.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        ModelBase Deserialize(Type modelType, TSerializationContext serializationContext);

        /// <summary>
        /// Deserializes the members of the specified model.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <returns>The list of members that have been deserialized.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        List<MemberValue> DeserializeMembers(Type modelType, TSerializationContext serializationContext);
    }
}