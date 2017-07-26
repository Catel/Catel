// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializationContextFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NET

namespace Catel.Runtime.Serialization.Binary
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Binary serialization context factory.
    /// </summary>
    public class BinarySerializationContextInfoFactory : ISerializationContextInfoFactory
    {
        /// <summary>
        /// Gets the serialization context information.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="model">The model.</param>
        /// <param name="data">The data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// ISerializationContextInfo.
        /// </returns>
        public ISerializationContextInfo GetSerializationContextInfo(ISerializer serializer, object model, object data, ISerializationConfiguration configuration)
        {
            return new BinarySerializationContextInfo((SerializationInfo)data);
        }
    }
}

#endif