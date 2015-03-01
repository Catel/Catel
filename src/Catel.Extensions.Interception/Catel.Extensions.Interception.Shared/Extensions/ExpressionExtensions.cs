// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Linq.Expressions;
    using Interception.Utilities;

    /// <summary>
    /// The <see cref="Expression{TDelegate}"/> extensions.
    /// </summary>
    public static class ExpressionExtensions
    {
        #region Methods
        /// <summary>
        /// Gets the method expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Method call expected</exception>
        public static MethodCallExpression GetMethodExpression<T>(this Expression<Action<T>> method)
        {
            Argument.IsNotNull(() => method);

            if (method.Body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException(@"Method call expected", method.Body.ToString());
            }

            return (MethodCallExpression) method.Body;
        }

        /// <summary>
        /// Gets the method expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="System.InvalidOperationException">Method expected:  + expression.Body</exception>
        public static MethodCallExpression GetMethodExpression<T>(this Expression<Func<T, object>> expression)
        {
            Argument.IsNotNull(() => expression);

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Call:
                    return (MethodCallExpression) expression.Body;

                case ExpressionType.Convert:
                    var unaryExpression = expression.Body as UnaryExpression;
                    Confirm.Assertion(unaryExpression != null && unaryExpression.Operand is MethodCallExpression, string.Format("Method expected: {0}", unaryExpression.Operand));
                    return unaryExpression.Operand as MethodCallExpression;

                default:
                    throw new InvalidOperationException(string.Format("Method expected: {0}", expression.Body));
            }
        }

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">Property expected</exception>
        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T, object>> expression)
        {
            Argument.IsNotNull(() => expression);

            switch (expression.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return expression.Body as MemberExpression;

                case ExpressionType.Convert:
                    var unaryExpression = expression.Body as UnaryExpression;
                    if (unaryExpression != null)
                    {
                        Confirm.Assertion(unaryExpression.Operand is MemberExpression, string.Format("Property expected: {0}", unaryExpression.Operand));
                    }
                    return ((UnaryExpression) expression.Body).Operand as MemberExpression;

                default:
                    throw new ArgumentException(@"Property expected", expression.Body.ToString());
            }
        }
        #endregion
    }
}