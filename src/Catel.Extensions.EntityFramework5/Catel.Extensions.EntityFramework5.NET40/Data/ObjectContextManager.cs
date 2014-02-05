// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectContextManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Data.Objects;

    /// <summary>
    /// Provides an automated way to reuse Entity Framework ObjectContext objects within the context of a single data portal operation.
    /// </summary>
    /// <typeparam name="TObjectContext">Type of the object context to use.
    /// </typeparam>
    /// <remarks>
    /// This type stores the object context object in an internal dictionary and uses reference counting through
    /// <see cref="IDisposable" /> to keep the data context object open for reuse by child objects, and to automatically
    /// dispose the object when the last consumer has called Dispose.
    /// </remarks>
    public class ObjectContextManager<TObjectContext> : ContextManager<TObjectContext>
        where TObjectContext : ObjectContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContextManager{TObjectContext}"/> class.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        private ObjectContextManager(string databaseNameOrConnectionStringName, string label) 
            : base(databaseNameOrConnectionStringName, label, null, null) { }

        /// <summary>
        /// Initializes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Initialize(TObjectContext context)
        {
            // No initialization required
        }

        /// <summary>
        /// Gets the ContextManager object for the specified database.
        /// </summary>
        /// <returns>The <see cref="ObjectContextManager{TObjectContext}" />.</returns>
        public static ObjectContextManager<TObjectContext> GetManager()
        {
            return GetManager(string.Empty);
        }

        /// <summary>
        /// Gets the ContextManager object for the specified database.
        /// </summary>
        /// <param name="databaseNameOrConnectionStringName">The database name or connection string.</param>
        /// <param name="label">Label for this context.</param>
        /// <returns>The ContextManager.</returns>
        public static ObjectContextManager<TObjectContext> GetManager(string databaseNameOrConnectionStringName, string label = "default")
        {
            Argument.IsNotNullOrWhitespace("label", label);

            return (ObjectContextManager<TObjectContext>)GetManager(databaseNameOrConnectionStringName, label, () => { return new ObjectContextManager<TObjectContext>(databaseNameOrConnectionStringName, label); });
        }
    }
}