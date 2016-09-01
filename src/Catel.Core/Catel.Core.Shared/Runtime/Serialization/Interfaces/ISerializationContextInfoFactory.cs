// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationContextFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Factory responsible for the serialization context info.
    /// </summary>
    public interface ISerializationContextInfoFactory
    {
        /// <summary>
        /// Gets the serialization context based on the specific info.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="model">The model.</param>
        /// <param name="data">The data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// ISerializationContext.
        /// </returns>
        ISerializationContextInfo GetSerializationContextInfo(ISerializer serializer, object model, object data, ISerializationConfiguration configuration);
    }
}