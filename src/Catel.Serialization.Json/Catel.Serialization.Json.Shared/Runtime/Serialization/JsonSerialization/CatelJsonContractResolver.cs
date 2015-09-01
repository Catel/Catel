// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelJsonContractResolver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Json
{
    using System;
    using Data;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Reflection;

    /// <summary>
    /// Contract resolver to ensure that Catel models will be serialized by the Catel serializers.
    /// </summary>
    public class CatelJsonContractResolver : DefaultContractResolver
    {
        #region Methods
        /// <summary>
        /// Resolves the contract converter.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>JsonConverter.</returns>
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof (ModelBase).IsAssignableFromEx(objectType))
            {
                return null; // pretend converter is not specified
            }

            return base.ResolveContractConverter(objectType);
        }
        #endregion
    }
}