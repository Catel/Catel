// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectAdapter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    using System.Xml.Linq;

    /// <summary>
    /// Xml serialization context factory.
    /// </summary>
    public class XmlSerializationContextInfoFactory : ISerializationContextInfoFactory
    {
        /// <summary>
        /// Gets the serialization context information.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="model">The model.</param>
        /// <param name="data">The data.</param>
        /// <returns>ISerializationContext.</returns>
        [ObsoleteEx(ReplacementTypeOrMember = "GetSerializationContextInfo(ISerializer, object, object, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public ISerializationContextInfo GetSerializationContextInfo(ISerializer serializer, object model, object data)
        {
            return GetSerializationContextInfo(serializer, model, data, null);
        }

        /// <summary>
        /// Gets the serialization context information.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="model">The model.</param>
        /// <param name="data">The data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// ISerializationContext.
        /// </returns>
        public ISerializationContextInfo GetSerializationContextInfo(ISerializer serializer, object model, object data, ISerializationConfiguration configuration)
        {
            return new XmlSerializationContextInfo((XElement)data, model);
        }
    }
}
