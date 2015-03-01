// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AndSpecification.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Specifications
{
    using System;
    using System.Linq;
    using Expressions;

    /// <summary>
    /// A specification where the entity must either match the left and right side.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class AndSpecification<TEntity> : CompositeSpecification<TEntity>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AndSpecification{TEntity}" /> class.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <param name="rightSide">The right side.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="leftSide"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rightSide"/> is <c>null</c>.</exception>
        public AndSpecification(Specification<TEntity> leftSide, Specification<TEntity> rightSide)
            : base(leftSide, rightSide, () => leftSide.Predicate.And(rightSide.Predicate).Compile())
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
            return query.Where(LeftSide.Predicate.And(RightSide.Predicate));
        }
        #endregion
    }
}