// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationManagerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Extension methods for the <see cref="ISerializationManager"/>.
    /// </summary>
    public static class ISerializationManagerExtensions
    {
        /// <summary>
        /// Gets the serializer modifier for a specific type.
        /// </summary>
        /// <typeparam name="TType">The type of the to be (de)serialized type.</typeparam>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager"/> is <c>null</c>.</exception>
        public static ISerializerModifier[] GetSerializerModifiers<TType>(this ISerializationManager serializationManager)
        {
            Argument.IsNotNull("serializationManager", serializationManager);

            return serializationManager.GetSerializerModifiers(typeof(TType));
        }

        /// <summary>
        /// Adds the serializer modifier for a specific type.
        /// </summary>
        /// <typeparam name="TType">The type of the to be (de)serialized type.</typeparam>
        /// <typeparam name="TSerializerModifier">The type of the serializer modifier.</typeparam>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager"/> is <c>null</c>.</exception>
        public static void AddSerializerModifier<TType, TSerializerModifier>(this ISerializationManager serializationManager)
            where TSerializerModifier : ISerializerModifier
        {
            Argument.IsNotNull("serializationManager", serializationManager);

            serializationManager.AddSerializerModifier(typeof(TType), typeof(TSerializerModifier));
        }

        /// <summary>
        /// Removes the serializer modifier for a specific type.
        /// </summary>
        /// <typeparam name="TType">The type of the to be (de)serialized type.</typeparam>
        /// <typeparam name="TSerializerModifier">The type of the serializer modifier.</typeparam>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager"/> is <c>null</c>.</exception>
        public static void RemoveSerializerModifier<TType, TSerializerModifier>(this ISerializationManager serializationManager)
            where TSerializerModifier : ISerializerModifier
        {
            Argument.IsNotNull("serializationManager", serializationManager);

            serializationManager.RemoveSerializerModifier(typeof(TType), typeof(TSerializerModifier));
        }
    }
}