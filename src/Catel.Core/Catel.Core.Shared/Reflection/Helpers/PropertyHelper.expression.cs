﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelper.expression.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Caching;
    using Logging;

    /// <summary>
    /// Property helper class.
    /// </summary>
    public static partial class PropertyHelper
    {
        private static readonly ICacheStorage<string, string> _expressionNameCache = new CacheStorage<string, string>(); 

        /// <summary>
        /// Gets the name of the property based on the expression.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="allowNested">If set to <c>true</c>, nested properties are allowed.</param>
        /// <returns>The string representing the property name.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The specified expression is not a member access expression.</exception>
        public static string GetPropertyName(Expression propertyExpression, bool allowNested = false)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            return GetPropertyName(propertyExpression, allowNested, false);
        }

        /// <summary>
        /// Gets the name of the property based on the expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="allowNested">If set to <c>true</c>, nested properties are allowed.</param>
        /// <returns>The string representing the property name.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The specified expression is not a member access expression.</exception>
        public static string GetPropertyName<TValue>(Expression<Func<TValue>> propertyExpression, bool allowNested = false)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            var body = propertyExpression.Body;
            return GetPropertyName(body, allowNested);
        }

        /// <summary>
        /// Gets the name of the property based on the expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="allowNested">If set to <c>true</c>, nested properties are allowed.</param>
        /// <returns>The string representing the property name.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The specified expression is not a member access expression.</exception>
        public static string GetPropertyName<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression, bool allowNested = false)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            var body = propertyExpression.Body;
            return GetPropertyName(body, allowNested);
        }

        /// <summary>
        /// Gets the name of the property based on the expression.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="allowNested">If set to <c>true</c>, nested properties are allowed.</param>
        /// <param name="nested">If set to <c>true</c>, this is a nested call.</param>
        /// <returns>The string representing the property name or <see cref="string.Empty"/> if no property can be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The specified expression is not a member access expression.</exception>
        private static string GetPropertyName(Expression propertyExpression, bool allowNested = false, bool nested = false)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);

            const string NoMemberExpression = "The expression is not a member access expression";

            string cacheKey = string.Format("{0}_{1}_{2}", propertyExpression, allowNested, nested);

            return _expressionNameCache.GetFromCacheOrFetch(cacheKey, () =>
            {
                MemberExpression memberExpression;

                var unaryExpression = propertyExpression as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                else
                {
                    memberExpression = propertyExpression as MemberExpression;
                }

                if (memberExpression == null)
                {
                    if (nested)
                    {
                        return string.Empty;
                    }

                    Log.Error(NoMemberExpression);
                    throw new NotSupportedException(NoMemberExpression);
                }

                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo == null)
                {
                    if (nested)
                    {
                        return string.Empty;
                    }

                    Log.Error(NoMemberExpression);
                    throw new NotSupportedException(NoMemberExpression);
                }

                if (allowNested && (memberExpression.Expression != null) && (memberExpression.Expression.NodeType == ExpressionType.MemberAccess))
                {
                    var propertyName = GetPropertyName(memberExpression.Expression, true, true);

                    return propertyName + (!string.IsNullOrEmpty(propertyName) ? "." : string.Empty) + propertyInfo.Name;
                }

                return propertyInfo.Name;
            });
        }
    }
}