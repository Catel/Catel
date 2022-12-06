namespace Catel.Reflection
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Caching;
    using Catel.Data;
    using Logging;

    /// <summary>
    /// Property helper class.
    /// </summary>
    public static partial class PropertyHelper
    {
        private static readonly ICacheStorage<string, string> ExpressionNameCache = new CacheStorage<string, string>(); 

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
            ArgumentNullException.ThrowIfNull(propertyExpression);

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
            ArgumentNullException.ThrowIfNull(propertyExpression);

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
            ArgumentNullException.ThrowIfNull(propertyExpression);

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
            ArgumentNullException.ThrowIfNull(propertyExpression);

            const string NoMemberExpression = "The expression is not a member access expression";

            var cacheKey = string.Format("{0}_{1}_{2}", propertyExpression, BoxingCache.GetBoxedValue(allowNested), BoxingCache.GetBoxedValue(nested));

            return ExpressionNameCache.GetFromCacheOrFetch(cacheKey, () =>
            {
                MemberExpression? memberExpression;

                var unaryExpression = propertyExpression as UnaryExpression;
                if (unaryExpression is not null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                else
                {
                    memberExpression = propertyExpression as MemberExpression;
                }

                if (memberExpression is null)
                {
                    if (nested)
                    {
                        return string.Empty;
                    }

                    throw Log.ErrorAndCreateException<NotSupportedException>(NoMemberExpression);
                }

                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo is null)
                {
                    if (nested)
                    {
                        return string.Empty;
                    }

                    throw Log.ErrorAndCreateException<NotSupportedException>(NoMemberExpression);
                }

                if (allowNested && (memberExpression.Expression is not null) && (memberExpression.Expression.NodeType == ExpressionType.MemberAccess))
                {
                    var propertyName = GetPropertyName(memberExpression.Expression, true, true);

                    return propertyName + (!string.IsNullOrEmpty(propertyName) ? "." : string.Empty) + propertyInfo.Name;
                }

                return propertyInfo.Name;
            });
        }
    }
}
