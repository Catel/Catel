// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContractNameResolver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Catel.Caching;
    using Catel.Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IDataContractNameResolver"/> in Catel. 
    /// <para />
    /// Base on the information found at http://msdn.microsoft.com/en-us/library/ms731045.aspx.
    /// </summary>
    public class DataContractNameResolver : IDataContractNameResolver
    {
        private static readonly CacheStorage<Type, string> _cache = new CacheStorage<Type, string>();

        /// <summary>
        /// Gets the data contract name of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A string representing the data contract name.</returns>
        public string GetDataContractName(Type type)
        {
            Argument.IsNotNull("type", type);

            return _cache.GetFromCacheOrFetch(type, () => GetDataContractNameWithoutCache(type));
        }

        /// <summary>
        /// Gets the data contract name of the specified type without using the cache.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A string representing the data contract name.</returns>
        private string GetDataContractNameWithoutCache(Type type)
        {
            var dataContractName = string.Empty;
            var dataContractNamespace = GetDataContractNamespace(type);

            var dataContractAttribute = (from attribute in type.GetCustomAttributesEx(typeof(DataContractAttribute), true)
                                         where attribute is DataContractAttribute
                                         select (DataContractAttribute)attribute).FirstOrDefault();

            if (dataContractAttribute != null)
            {
                dataContractName = dataContractAttribute.Name;
            }

            if (string.IsNullOrEmpty(dataContractName))
            {
                if (!string.IsNullOrEmpty(type.Namespace))
                {
                    dataContractName = type.Namespace + ".";
                }

                dataContractName += type.Name;
            }

            var fullDataContractName = string.Format("{0}/{1}", dataContractNamespace, dataContractName);
            return fullDataContractName;
        }

        private string GetDataContractNamespace(Type type)
        {
            string dataContractNamespace = null;

            // 1) Check type (DataContractAttribute)
            // TODO

            // 2) Check assembly (ContractNamespaceAttribute)
            // TODO

            // 3) Fallback to default
            if (string.IsNullOrEmpty(dataContractNamespace))
            {
                dataContractNamespace = "http://schemas.datacontract.org/2004/07";
            }

            if (!dataContractNamespace.EndsWith("/"))
            {
                dataContractNamespace += "/";
            }

            return dataContractNamespace;
        }
    }
}