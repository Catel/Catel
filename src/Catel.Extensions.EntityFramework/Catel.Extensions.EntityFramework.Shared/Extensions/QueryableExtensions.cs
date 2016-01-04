// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryableExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

#if EF5
    using System.Data.Objects;
#else
    using System.Data.Entity.Core.Objects;
#endif

    /// <summary>
    /// Extensions for the <see cref="IQueryable"/> interface.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Includes the specified expression in the query. This method allows the additional retrieval of elements.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>The queryable with the include path.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is <c>null</c>.</exception>
#if !EF5
        // TODO: Put #if EF5 on top of this file
        [ObsoleteEx(ReplacementTypeOrMember = "System.Data.Entity.QueryableExtensions.Include(Expression<Func<T, object>>)", TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
#endif
        public static IQueryable<T> Include<T>(this IQueryable<T> query, Expression<Func<T, object>> expression)
        {
            Argument.IsNotNull(() => query);
            Argument.IsNotNull(() => expression);

            var objectQuery = query as ObjectQuery<T>;
            if (objectQuery != null)
            {
                var memberExpression = (MemberExpression)expression.Body;
                var path = GetIncludePath(memberExpression);

                return objectQuery.Include(path);
            }

            return query;
        }

        /// <summary>
        /// Gets the include path.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns>The include path.</returns>
        private static string GetIncludePath(MemberExpression memberExpression)
        {
            var path = string.Empty;

            var includePathExpression = memberExpression.Expression as MemberExpression;
            if (includePathExpression != null)
            {
                path = GetIncludePath(includePathExpression) + ".";
            }

            var propertyInfo = (PropertyInfo)memberExpression.Member;
            return path + propertyInfo.Name;
        }
    }
}