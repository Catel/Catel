// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;
    using System.Data.Entity.Infrastructure;

#if EF5
    using System.Data.Objects;
#else
    using System.Data.Entity.Core.Objects;
#endif

    /// <summary>
    /// Class responsible for instantiating contexts.
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        /// <summary>
        /// Creates the specified context using the input parameters.
        /// </summary>
        /// <param name="contextType">Type of the context.</param>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The created context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="contextType" /> is <c>null</c>.</exception>
        public virtual object CreateContext(Type contextType, string databaseNameOrConnectionStringName, string label, DbCompiledModel model, ObjectContext context)
        {
            object createdContext = null; 

            if (model != null)
            {
                if (!string.IsNullOrEmpty(databaseNameOrConnectionStringName))
                {
                    createdContext = Activator.CreateInstance(contextType, databaseNameOrConnectionStringName, model);
                }
                else
                {
                    createdContext = Activator.CreateInstance(contextType, model);
                }
            }
            else if (context != null)
            {
                createdContext = Activator.CreateInstance(contextType, context, true);
            }
            else if (string.IsNullOrEmpty(databaseNameOrConnectionStringName))
            {
                createdContext = Activator.CreateInstance(contextType);
            }
            else
            {
                createdContext = Activator.CreateInstance(contextType, databaseNameOrConnectionStringName);
            }

            return createdContext;
        }

        /// <summary>
        /// Creates the specified context using the input parameters.
        /// </summary>
        /// <typeparam name="TContext">The type of the T context.</typeparam>
        /// <param name="databaseNameOrConnectionStringName">Name of the database name or connection string.</param>
        /// <param name="label">The label.</param>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The created context.</returns>
        public TContext CreateContext<TContext>(string databaseNameOrConnectionStringName, string label, DbCompiledModel model, ObjectContext context)
        {
            return (TContext)CreateContext(typeof(TContext), databaseNameOrConnectionStringName, label, model, context);
        }
    }
}