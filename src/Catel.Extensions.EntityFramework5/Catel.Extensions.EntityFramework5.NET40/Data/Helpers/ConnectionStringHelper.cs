// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Objects;

    /// <summary>
    /// The connection string helper.
    /// </summary>
    public static class ConnectionStringHelper
    {
        #region Methods
        /// <summary>
        /// Sets the connection string of the specified <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="connectionString"/> is <c>null</c> or whitespace.</exception>
        public static void SetConnectionString(this DbContext dbContext, string connectionString)
        {
            Argument.IsNotNull("dbContext", dbContext);
            Argument.IsNotNullOrWhitespace("connectionString", connectionString);

            dbContext.Database.Connection.ConnectionString = connectionString;
        }

        /// <summary>
        /// Sets the connection string of the specified <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="objectContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="connectionString"/> is <c>null</c> or whitespace.</exception>
        public static void SetConnectionString(this ObjectContext objectContext, string connectionString)
        {
            Argument.IsNotNull("objectContext", objectContext);
            Argument.IsNotNullOrWhitespace("connectionString", connectionString);

            var efConnectionString = EfConnectionStringHelper.GetEntityFrameworkConnectionString(objectContext.GetType(), connectionString);

            objectContext.Connection.ConnectionString = efConnectionString;
        }

        /// <summary>
        /// Gets the connection string currently used by the <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <returns>The connection string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        public static string GetConnectionString(this DbContext dbContext)
        {
            Argument.IsNotNull("dbContext", dbContext);

            var connectionString = dbContext.Database.Connection.ConnectionString;

            return GetConnectionStringFromConfigIfRequired(connectionString);
        }

        /// <summary>
        /// Gets the connection string currently used by the <see cref="ObjectContext"/>.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <returns>The connection string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="objectContext" /> is <c>null</c>.</exception>
        public static string GetConnectionString(this ObjectContext objectContext)
        {
            Argument.IsNotNull("objectContext", objectContext);

            var connectionString = objectContext.Connection.ConnectionString;

            return GetConnectionStringFromConfigIfRequired(connectionString);
        }

        /// <summary>
        /// Gets the connection string from the configuration if required.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The connection string.</returns>
        /// <exception cref="ArgumentException">The <paramref name="connectionString" /> is <c>null</c> or whitespace.</exception>
        private static string GetConnectionStringFromConfigIfRequired(string connectionString)
        {
            Argument.IsNotNullOrWhitespace("connectionString", connectionString);

            if (connectionString.StartsWith("name="))
            {
                string configurationName = connectionString.Replace("name=", string.Empty);
                connectionString = ConfigurationManager.ConnectionStrings[configurationName].ConnectionString;
            }

            return connectionString;
        }
        #endregion
    }
}