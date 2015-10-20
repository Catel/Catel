// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Additional features for serializable objects.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Occurs when the object has been serialized.
        /// </summary>
        event EventHandler<EventArgs> Serialized;

        /// <summary>
        /// Occurs when the object has been deserialized.
        /// </summary>
        event EventHandler<EventArgs> Deserialized;

        /// <summary>
        /// Starts the serialization.
        /// </summary>
        void StartSerialization();

        /// <summary>
        /// Finishes the serialization.
        /// </summary>
        void FinishSerialization();

        /// <summary>
        /// Starts the deserialization.
        /// </summary>
        void StartDeserialization();

        /// <summary>
        /// Finishes the deserialization.
        /// </summary>
        void FinishDeserialization();
    }
}