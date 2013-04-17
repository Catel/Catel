// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrSpecification.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Specifications
{
    using System;
    using System.Linq;
    using Expressions;

    /// <summary>
    /// A specification where the entity must either match the left or right side.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class OrSpecification<TEntity> : CompositeSpecification<TEntity>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OrSpecification{TEntity}" /> class.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <param name="rightSide">The right side.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="leftSide"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rightSide"/> is <c>null</c>.</exception>
        public OrSpecification(Specification<TEntity> leftSide, Specification<TEntity> rightSide)
            : base(leftSide, rightSide, () => leftSide.Predicate.Or(rightSide.Predicate).Compile())
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a queryable of entities that match the current specification.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable of entities that match the specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        public override IQueryable<TEntity> SatisfyingEntitiesFrom(IQueryable<TEntity> query)
        {
            Argument.IsNotNull("query", query);

            return query.Where(LeftSide.Predicate.Or(RightSide.Predicate));
        }
        #endregion
    }
}