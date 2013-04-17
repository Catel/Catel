// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression extensions class.
    /// </summary>
    public static class ExpressionExtensions
    {
        #region Methods
        /// <summary>
        /// Composes the two specified expressions with the merge action.
        /// </summary>
        /// <typeparam name="T">The object type inside the expression.</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <param name="merge">The merge function.</param>
        /// <returns>The composed expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="first"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="second"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="merge"/> is <c>null</c>.</exception>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            Argument.IsNotNull("first", first);
            Argument.IsNotNull("second", second);
            Argument.IsNotNull("merge", merge);

            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new {f, s = second.Parameters[i]}).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Combines the expressions using the <see cref="System.Linq.Expressions.Expression.And(Expression, Expression)"/> method.
        /// </summary>
        /// <typeparam name="T">The object type inside the expression.</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>The composed expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="first"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="second"/> is <c>null</c>.</exception>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            Argument.IsNotNull("first", first);
            Argument.IsNotNull("second", second);

            return first.Compose(second, Expression.And);
        }

        /// <summary>
        /// Combines the expressions using the <see cref="System.Linq.Expressions.Expression.Or(Expression, Expression)"/> method.
        /// </summary>
        /// <typeparam name="T">The object type inside the expression.</typeparam>
        /// <param name="first">The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>The composed expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="first"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="second"/> is <c>null</c>.</exception>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            Argument.IsNotNull("first", first);
            Argument.IsNotNull("second", second);

            return first.Compose(second, Expression.Or);
        }
        #endregion
    }
}