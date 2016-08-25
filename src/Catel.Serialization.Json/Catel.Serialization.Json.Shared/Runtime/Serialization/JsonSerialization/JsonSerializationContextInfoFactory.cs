// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializationContextFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NET

namespace Catel.Runtime.Serialization.Binary
{
    using Json;
    using JsonSerialization;

    /// <summary>
    /// Json serialization context factory.
    /// </summary>
    public class JsonSerializationContextInfoFactory : ISerializationContextInfoFactory
    {
        /// <summary>
        /// Gets the serialization context information.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="model">The model.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        /// ISerializationContextInfo.
        /// </returns>
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
        /// ISerializationContextInfo.
        /// </returns>
        [ObsoleteEx(ReplacementTypeOrMember = "GetSerializationContextInfo(ISerializer, object, object, ISerializationConfiguration)",
                    TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public ISerializationContextInfo GetSerializationContextInfo(ISerializer serializer, object model, object data, ISerializationConfiguration configuration)
        {
            var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            jsonSerializer.ContractResolver = new CatelJsonContractResolver();
            jsonSerializer.Converters.Add(new CatelJsonConverter((IJsonSerializer)serializer, configuration));

            return new JsonSerializationContextInfo(jsonSerializer, null, null);
        }
    }
}

#endif