// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextExtensions.generic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System.Data.Entity;

    public static partial class DbContextExtensions
    {
        /// <summary>
        /// Gets the name of the table as it is mapped in the database.
        /// </summary>
        /// <typeparam name="TEntity">The entity.</typeparam>
        /// <param name="context">The context.</param>
        /// <returns>The table name including the schema.</returns>
        public static string GetTableName<TEntity>(this DbContext context)
            where TEntity : class
        {
            return GetTableName(context, typeof(TEntity));
        }
    }
}