// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializationContextFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NET || NETCORE

namespace Catel.Runtime.Serialization.Json
{
    using Json;

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
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// ISerializationContextInfo.
        /// </returns>
        public ISerializationContextInfo GetSerializationContextInfo(ISerializer serializer, object model, object data, ISerializationConfiguration configuration = null)
        {
            var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            jsonSerializer.ContractResolver = new CatelJsonContractResolver();
            jsonSerializer.Converters.Add(new CatelJsonConverter((IJsonSerializer)serializer, configuration));

            return new JsonSerializationContextInfo(jsonSerializer, null, null);
        }
    }
}

#endif
