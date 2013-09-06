// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using Data;

    /// <summary>
    /// The mode in which a context is being used.
    /// </summary>
    public enum SerializationContextMode 
    {
        /// <summary>
        /// The context is being used for serialization.
        /// </summary>
        Serialization,

        /// <summary>
        /// The context is being used for deserialization.
        /// </summary>
        Deserialization
    }

    /// <summary>
    /// Interface for the serialization context used to serialize and deserialize models.
    /// </summary>
    public interface ISerializationContext : IDisposable
    {
        /// <summary>
        /// Gets the model that needs serialization or deserialization.
        /// </summary>
        /// <value>The model.</value>
        ModelBase Model { get; }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        Type ModelType { get; }

        /// <summary>
        /// Gets the context mode.
        /// </summary>
        /// <value>The context mode.</value>
        SerializationContextMode ContextMode { get; }

        /// <summary>
        /// Gets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        ReferenceManager ReferenceManager { get; }
    }

    /// <summary>
    /// Interface for the serialization context used to serialize and deserialize models.
    /// </summary>
    /// <typeparam name="TSerializationContext">The type of the serialization context.</typeparam>
    public interface ISerializationContext<TSerializationContext> : ISerializationContext
        where TSerializationContext : class
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        TSerializationContext Context { get; }
    }
}