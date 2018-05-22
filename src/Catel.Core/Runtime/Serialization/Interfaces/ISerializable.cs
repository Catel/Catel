// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Additional features for serializable objects.
    /// </summary>
    public interface ISerializable
    {
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