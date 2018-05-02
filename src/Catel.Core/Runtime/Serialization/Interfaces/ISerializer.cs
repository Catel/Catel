// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel.Reflection;
    using Data;

    /// <summary>
    /// Interface definition to serialize the <see cref="IModel"/>.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Occurs when an object is about to be serialized.
        /// </summary>
        event EventHandler<SerializationEventArgs> Serializing;

        /// <summary>
        /// Occurs when an object is about to serialize a specific member.
        /// </summary>
        event EventHandler<MemberSerializationEventArgs> SerializingMember;

        /// <summary>
        /// Occurs when an object has just serialized a specific member.
        /// </summary>
        event EventHandler<MemberSerializationEventArgs> SerializedMember;

        /// <summary>
        /// Occurs when an object has just been serialized.
        /// </summary>
        event EventHandler<SerializationEventArgs> Serialized;

        /// <summary>
        /// Occurs when an object is about to be deserialized.
        /// </summary>
        event EventHandler<SerializationEventArgs> Deserializing;

        /// <summary>
        /// Occurs when an object is about to deserialize a specific member.
        /// </summary>
        event EventHandler<MemberSerializationEventArgs> DeserializingMember;

        /// <summary>
        /// Occurs when an object has just deserialized a specific member.
        /// </summary>
        event EventHandler<MemberSerializationEventArgs> DeserializedMember;

        /// <summary>
        /// Occurs when an object has just been deserialized.
        /// </summary>
        event EventHandler<SerializationEventArgs> Deserialized;

        /// <summary>
        /// Warms up the specified types. If the <paramref name="types" /> is <c>null</c>, all types known
        /// in the <see cref="TypeCache" /> deriving from the <see cref="ModelBase"/> class will be initialized.
        /// <para />
        /// Note that it is not required to call this, but it can help to prevent an additional performance
        /// impact the first time a type is serialized.
        /// </summary>
        /// <param name="types">The types to warmp up. If <c>null</c>, all types will be initialized.</param>
        /// <param name="typesPerThread">The types per thread. If <c>-1</c>, all types will be initialized on the same thread.</param>
        void Warmup(IEnumerable<Type> types = null, int typesPerThread = 1000);

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        void Serialize(object model, Stream stream, ISerializationConfiguration configuration = null);

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        void Serialize(object model, ISerializationContextInfo serializationContext, ISerializationConfiguration configuration = null);

        /// <summary>
        /// Serializes the members.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="membersToIgnore"/> is <c>null</c>.</exception>
        void SerializeMembers(object model, Stream stream, ISerializationConfiguration configuration, params string[] membersToIgnore);

        /// <summary>
        /// Deserializes the specified model. The deserialized values will be set in the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The deserialized model.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        object Deserialize(object model, Stream stream, ISerializationConfiguration configuration = null);

        /// <summary>
        /// Deserializes the specified model. The deserialized values will be set in the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The deserialized model.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        object Deserialize(object model, ISerializationContextInfo serializationContext, ISerializationConfiguration configuration = null);

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The deserialized model.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        object Deserialize(Type modelType, Stream stream, ISerializationConfiguration configuration = null);

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The deserialized model.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        object Deserialize(Type modelType, ISerializationContextInfo serializationContext, ISerializationConfiguration configuration = null);

        /// <summary>
        /// Deserializes the members of the specified model.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The list of members that have been deserialized.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        List<MemberValue> DeserializeMembers(Type modelType, Stream stream, ISerializationConfiguration configuration = null);

        /// <summary>
        /// Deserializes the members of the specified model.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The list of members that have been deserialized.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        List<MemberValue> DeserializeMembers(Type modelType, ISerializationContextInfo serializationContext, ISerializationConfiguration configuration = null);
    }
}