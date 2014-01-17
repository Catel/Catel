// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterRebinder.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Expression visitor implementation to rebind parameters in linq queries.
    /// </summary>
    public class ParameterRebinder : ExpressionVisitor
    {
        #region Fields
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterRebinder" /> class.
        /// </summary>
        /// <param name="map">The map. If <c>null</c>, an empty map will be created.</param>
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Replaces the parameters.
        /// </summary>
        /// <param name="map">The expression mappings dictionary.</param>
        /// <param name="exp">The expression.</param>
        /// <returns>The expression with the replaced parameters.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="map"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exp"/> is <c>null</c>.</exception>
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            Argument.IsNotNull("map", map);
            Argument.IsNotNull("exp", exp);

            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="parameterExpression">The parameter expression.</param>
        /// <returns>The visited expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterExpression"/> is <c>null</c>.</exception>
        protected override Expression VisitParameter(ParameterExpression parameterExpression)
        {
            ParameterExpression replacement;
            if (_map.TryGetValue(parameterExpression, out replacement))
            {
                parameterExpression = replacement;
            }

            return base.VisitParameter(parameterExpression);
        }
        #endregion
    }
}