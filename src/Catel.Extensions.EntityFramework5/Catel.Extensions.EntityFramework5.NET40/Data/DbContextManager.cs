// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using Logging;

    /// <summary>
    /// Provides an automated way to reuse Entity Framework DbContext objects within the context of a single data portal operation.
    /// </summary>
    /// <typeparam name="TDbContext">Type of the db context to use.
    /// </typeparam>
    /// <remarks>
    /// This type stores the object context object in an internal dictionary and uses reference counting through
    /// <see cref="IDisposable" /> to keep the data context object open for reuse by child objects, and to automatically
    /// dispose the object when the last consumer has called Dispose.
    /// </remarks>
    public class DbContextManager<TDbContext> : ContextManager<TDbContext> 
        where TDbContext : DbContext
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextManager{TDbContext}"/> class.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        private DbContextManager(string databaseNameOrConnectionStringName, string label, DbCompiledModel model, ObjectContext context) 
            : base(databaseNameOrConnectionStringName, label, model, context) { }

        /// <summary>
        /// Initializes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Initialize(TDbContext context)
        {
            try
            {
                context.Database.Initialize(false);
            }
            catch (Exception)
            {
                Log.Warning("Failed to initialize database context '{0}', probably the connection cannot be established", context.GetType().FullName);
            }
        }

        /// <summary>
        /// Gets the ContextManager object for the specified database.
        /// </summary>
        /// <returns>The <see cref="DbContextManager{TDbContext}" />.</returns>
        public static DbContextManager<TDbContext> GetManager()
        {
            return GetManager(string.Empty);
        }

        /// <summary>
        /// Gets the ContextManager object for the specified database.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">The database name or connection string.</param>
        /// <param name="label">Label for this context.</param>
        /// <param name="model">Database Compiled model.</param>
        /// <returns>The ContextManager.</returns>
        public static DbContextManager<TDbContext> GetManager(string databaseNameOrConnectionStringName, string label = "default", DbCompiledModel model = null)
        {
            Argument.IsNotNullOrWhitespace("label", label);

            return (DbContextManager<TDbContext>)GetManager(databaseNameOrConnectionStringName, label, () => { return new DbContextManager<TDbContext>(databaseNameOrConnectionStringName, label, model, null); });
        }
    }
}