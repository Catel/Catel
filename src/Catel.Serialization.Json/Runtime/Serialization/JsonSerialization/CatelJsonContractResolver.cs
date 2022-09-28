namespace Catel.Runtime.Serialization.Json
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Reflection;

    /// <summary>
    /// Contract resolver to ensure that Catel models will be serialized by the Catel serializers.
    /// </summary>
    public class CatelJsonContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Resolves the contract converter.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>JsonConverter.</returns>
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType.IsModelBase())
            {
                return null; // pretend converter is not specified
            }

            return base.ResolveContractConverter(objectType);
        }
    }
}
