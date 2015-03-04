// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EfConnectionStringHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;

#if EF5
    using System.Data.EntityClient;
#else
    using System.Data.Entity.Core.EntityClient;
#endif

    /// <summary>
    /// The entity framework connection string helper.
    /// </summary>
    public static class EfConnectionStringHelper
    {
        /// <summary>
        /// Gets the entity framework connection string.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The entity framework connection string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="contextType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="connectionString" /> is <c>null</c> or whitespace.</exception>
        public static string GetEntityFrameworkConnectionString(Type contextType, string connectionString)
        {
            Argument.IsNotNull("contextType", contextType);
            Argument.IsNotNullOrWhitespace("connectionString", connectionString);

            if (connectionString.Contains("res://"))
            {
                // already EF connection string
                return connectionString;
            }

            var fullName = contextType.FullName;
            if (fullName.EndsWith("Container"))
            {
                fullName = fullName.Substring(0, fullName.Length - "Container".Length);
            }

            var assemblyNameParts = contextType.Assembly.GetName().Name.Split(new[] { '.' });
            foreach (var assemblyNamePart in assemblyNameParts)
            {
                string itemToReplace = string.Format("{0}.", assemblyNamePart);
                if (fullName.StartsWith(itemToReplace))
                {
                    fullName = fullName.Remove(0, itemToReplace.Length);
                }
            }

            var connectionBuilder = new EntityConnectionStringBuilder();
            connectionBuilder.Provider = "System.Data.SqlClient";
            connectionBuilder.Metadata = string.Format("res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", fullName);

            connectionBuilder.ProviderConnectionString = connectionString;

            return connectionBuilder.ToString();
        }
    }
}