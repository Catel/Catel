// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbContextManagerHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using Catel;
    using Reflection;

    /// <summary>
    /// Helper class for the <see cref="DbContextManager{TDbContext}"/> class.
    /// </summary>
    public static class DbContextManagerHelper
    {
        private static readonly MethodInfo _genericCreateDbContextForHttpContextMethod;
        private static readonly MethodInfo _genericDisposeDbContextForHttpContextMethod;

        /// <summary>
        /// Initializes static members of the <see cref="DbContextManagerHelper" /> class.
        /// </summary>
        static DbContextManagerHelper()
        {
            _genericCreateDbContextForHttpContextMethod = (from method in typeof(DbContextManagerHelper).GetMethodsEx(false, true)
                                                       where method.Name == "CreateDbContextForHttpContext" && method.IsGenericMethod
                                                       select method).First();

            _genericDisposeDbContextForHttpContextMethod = (from method in typeof(DbContextManagerHelper).GetMethodsEx(false, true)
                                                        where method.Name == "DisposeDbContextForHttpContext" && method.IsGenericMethod
                                                        select method).First();
        }

        #region Methods
        /// <summary>
        /// Creates the db context for the specified http context.
        /// </summary>
        /// <param name="dbContextType">Type of the db context.</param>
        /// <returns>The created <see cref="DbContextManager{TDbContext}"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContextType" /> is <c>null</c>.</exception>
        public static object CreateDbContextForHttpContext(Type dbContextType)
        {
            Argument.IsNotNull("dbContextType", dbContextType);

            var genericMethod = _genericCreateDbContextForHttpContextMethod.MakeGenericMethod(dbContextType);
            return genericMethod.Invoke(null, null);
        }

        /// <summary>
        /// Creates the db context for the specified http context.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the db context.</typeparam>
        /// <returns>The created <see cref="DbContextManager{TDbContext}"/>.</returns>
        public static DbContextManager<TDbContext> CreateDbContextForHttpContext<TDbContext>()
            where TDbContext : DbContext
        {
            var key = GetDbContextKey<TDbContext>();
            var httpContext = HttpContext.Current;

            var manager = (DbContextManager<TDbContext>)DbContextManager<TDbContext>.GetManager();
            httpContext.Items[key] = manager;

            return manager;
        }

        /// <summary>
        /// Gets the db context for the specified http context.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the db context.</typeparam>
        /// <returns>The <see cref="DbContextManager{TDbContext}"/> or <c>null</c> if no context was available.</returns>
        public static DbContextManager<TDbContext> GetDbContextForHttpContext<TDbContext>()
            where TDbContext : DbContext
        {
            var key = GetDbContextKey<TDbContext>();
            var httpContext = HttpContext.Current;

            return httpContext.Items[key] as DbContextManager<TDbContext>;
        }

        /// <summary>
        /// Disposes the db context for the specified http context.
        /// </summary>
        /// <param name="dbContextType">Type of the db context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContextType"/> is <c>null</c>.</exception>
        public static void DisposeDbContextForHttpContext(Type dbContextType)
        {
            Argument.IsNotNull("dbContextType", dbContextType);

            var genericMethod = _genericDisposeDbContextForHttpContextMethod.MakeGenericMethod(dbContextType);
            genericMethod.Invoke(null, null);
        }

        /// <summary>
        /// Disposes the db context for the specified http context.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the T db context.</typeparam>
        public static void DisposeDbContextForHttpContext<TDbContext>()
            where TDbContext : DbContext
        {
            var key = GetDbContextKey<TDbContext>();
            var httpContext = HttpContext.Current;

            var dbContextManager = httpContext.Items[key] as DbContextManager<TDbContext>;
            if (dbContextManager != null)
            {
                dbContextManager.Dispose();
            }
        }

        /// <summary>
        /// Gets the db context key to be used in an http context.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the db context.</typeparam>
        /// <returns>The context key.</returns>
        private static string GetDbContextKey<TDbContext>()
        {
            return string.Format("{0}Key", typeof(TDbContext).Name);
        }
        #endregion
    }
}