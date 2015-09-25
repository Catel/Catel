// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbEntityEntryExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Management.Instrumentation;
    using System.Text.RegularExpressions;

    using Caching;
    using Reflection;

#if EF5
    using ObjectContext = System.Data.Objects.ObjectContext;
    using DataSpace = System.Data.Metadata.Edm.DataSpace;
    using EntityKey = System.Data.EntityKey;
    using EntityKeyMember = System.Data.EntityKeyMember;
    using EntitySet = System.Data.Metadata.Edm.EntitySet;
    using EntityState = System.Data.EntityState;
#else
    using ObjectContext = System.Data.Entity.Core.Objects.ObjectContext;
    using DataSpace = System.Data.Entity.Core.Metadata.Edm.DataSpace;
    using EntityKey = System.Data.Entity.Core.EntityKey;
    using EntityKeyMember = System.Data.Entity.Core.EntityKeyMember;
    using EntitySet = System.Data.Entity.Core.Metadata.Edm.EntitySet;
    using EntityState = System.Data.Entity.EntityState;
#endif


    /// <summary>
    /// Extension methods for the DbEntityEntry class.
    /// </summary>
    public static class DbEntityEntryExtensions
    {
        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="dbEntityEntry">The database entity entry.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        public static Type GetEntityType(this DbEntityEntry dbEntityEntry)
        {
            Argument.IsNotNull("dbEntityEntry", dbEntityEntry);

            var entity = dbEntityEntry.Entity;
            return entity.GetEntityType();
        }

        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        public static Type GetEntityType(this object entity)
        {
            Argument.IsNotNull("entity", entity);

            var entityType = entity.GetType();
            return entityType.GetEntityType();
        }

        /// <summary>
        /// Gets the type of the entity. Even when proxies are enabled, this will return the 
        /// actual entity type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type or <c>null</c> if the type could not be determined.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static Type GetEntityType(this Type type)
        {
            Argument.IsNotNull("type", type);

            while (type != null && type.FullName.Contains(".DynamicProxies."))
            {
                type = type.BaseType;
            }

            return type;
        }

        /// <summary>
        /// Gets the current database value. If a value is <c>null</c>, it will be returned as <c>DBNull.Value</c>.
        /// </summary>
        /// <param name="dbEntityEntry">The database entity entry.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static object GetCurrentDbValue(this DbEntityEntry dbEntityEntry, string propertyName)
        {
            Argument.IsNotNull("dbEntityEntry", dbEntityEntry);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return GetDbValue(dbEntityEntry.CurrentValues, propertyName);
        }

        /// <summary>
        /// Gets the original database value. If a value is <c>null</c>, it will be returned as <c>DBNull.Value</c>.
        /// </summary>
        /// <param name="dbEntityEntry">The database entity entry.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbEntityEntry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static object GetOriginalDbValue(this DbEntityEntry dbEntityEntry, string propertyName)
        {
            Argument.IsNotNull("dbEntityEntry", dbEntityEntry);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return GetDbValue(dbEntityEntry.OriginalValues, propertyName);
        }

        /// <summary>
        /// Gets the database value. If a value is <c>null</c>, it will be returned as <c>DBNull.Value</c>.
        /// </summary>
        /// <param name="propertyValues">The property values.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyValues" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public static object GetDbValue(this DbPropertyValues propertyValues, string propertyName)
        {
            Argument.IsNotNull("propertyValues", propertyValues);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var value = propertyValues[propertyName];
            var dbValue = value ?? DBNull.Value;

            return dbValue;
        }
    }
}