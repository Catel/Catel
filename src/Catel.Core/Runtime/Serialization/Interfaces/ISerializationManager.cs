// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manager which is responsible for discovering what fields and properties of an object should be serialized.
    /// </summary>
    public interface ISerializationManager
    {
        /// <summary>
        /// Occurs when the cache for a specific type has been invalidated.
        /// </summary>
        event EventHandler<CacheInvalidatedEventArgs> CacheInvalidated;

        /// <summary>
        /// Warmups the specified type by calling all the methods for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        void Warmup(Type type);

        /// <summary>
        /// Clears the specified type from cache so it will be evaluated.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        void Clear(Type type);

        /// <summary>
        /// Gets the fields to serialize for the specified object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of fields to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        Dictionary<string, MemberMetadata> GetFieldsToSerialize(Type type);

        /// <summary>
        /// Gets the Catel properties to serialize for the specified object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of properties to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        Dictionary<string, MemberMetadata> GetCatelPropertiesToSerialize(Type type);

        /// <summary>
        /// Gets the regular properties to serialize for the specified object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of properties to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        Dictionary<string, MemberMetadata> GetRegularPropertiesToSerialize(Type type);

        /// <summary>
        /// Gets the catel property names.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <param name="includeModelBaseProperties">if set to <c>true</c>, also include model base properties.</param>
        /// <returns>A hash set containing the Catel property names.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        HashSet<string> GetCatelPropertyNames(Type type, bool includeModelBaseProperties = false);

        /// <summary>
        /// Gets the catel properties.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <param name="includeModelBaseProperties">if set to <c>true</c>, also include model base properties.</param>
        /// <returns>A hash set containing the Catel properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        Dictionary<string, MemberMetadata> GetCatelProperties(Type type, bool includeModelBaseProperties = false);

        /// <summary>
        /// Gets the regular property names.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the regular property names.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        HashSet<string> GetRegularPropertyNames(Type type);

        /// <summary>
        /// Gets the regular properties.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the regular properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        Dictionary<string, MemberMetadata> GetRegularProperties(Type type);

        /// <summary>
        /// Gets the field names.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the field names.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        HashSet<string> GetFieldNames(Type type);

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the fields.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        Dictionary<string, MemberMetadata> GetFields(Type type);

        /// <summary>
        /// Gets the serializer modifiers for the specified type.
        /// <para />
        /// Note that the order is important because the modifiers will be called in the returned order during serialization
        /// and in reversed order during deserialization.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An array containing the modifiers. Never <c>null</c>, but can be an empty array.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        ISerializerModifier[] GetSerializerModifiers(Type type);

        /// <summary>
        /// Adds the serializer modifier for a specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializerModifierType">Type of the serializer modifier.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializerModifierType"/> is <c>null</c>.</exception>
        void AddSerializerModifier(Type type, Type serializerModifierType);

        /// <summary>
        /// Removes the serializer modifier for a specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializerModifierType">Type of the serializer modifier.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializerModifierType"/> is <c>null</c>.</exception>
        void RemoveSerializerModifier(Type type, Type serializerModifierType);
    }
}