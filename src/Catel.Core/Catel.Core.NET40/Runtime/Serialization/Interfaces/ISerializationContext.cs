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
    /// Interface for the serialization context used to serialize and deserialize models.
    /// </summary>
    public interface ISerializationContext
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