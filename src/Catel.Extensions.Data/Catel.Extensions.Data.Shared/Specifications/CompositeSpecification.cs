// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeSpecification.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Specifications
{
    using System;
    using System.Linq;

    /// <summary>
    /// The composite specification which combines two specifications together.
    /// <para />
    /// Deriving from this class allows customization of how the two specifications must be met.
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public abstract class CompositeSpecification<TEntity> : ISpecification<TEntity>
    {
        #region Fields
        private readonly Lazy<Func<TEntity, bool>> _compiledPredicate;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeSpecification{TEntity}" /> class.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <param name="rightSide">The right side.</param>
        /// <param name="compilePredicateFunction">The compile predicate function.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="leftSide" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rightSide" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="compilePredicateFunction" /> is <c>null</c>.</exception>
        protected CompositeSpecification(Specification<TEntity> leftSide, Specification<TEntity> rightSide,
            Func<Func<TEntity, bool>> compilePredicateFunction)
        {
            Argument.IsNotNull("leftSide", leftSide);
            Argument.IsNotNull("rightSide", rightSide);
            Argument.IsNotNull("compilePredicateFunction", compilePredicateFunction);

            LeftSide = leftSide;
            RightSide = rightSide;

            _compiledPredicate = new Lazy<Func<TEntity, bool>>(compilePredicateFunction);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Performs an implicit conversion from <see cref="Specification{TEntity}" /> to <see cref="Func{TEntity, Boolean}" />.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Func<TEntity, bool>(CompositeSpecification<TEntity> specification)
        {
            return specification._compiledPredicate.Value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the left side.
        /// </summary>
        /// <value>The left side.</value>
        protected Specification<TEntity> LeftSide { get; private set; }

        /// <summary>
        /// Gets the right side.
        /// </summary>
        /// <value>The right side.</value>
        protected Specification<TEntity> RightSide { get; private set; }
        #endregion

        #region ISpecification<TEntity> Members
        /// <summary>
        /// Returns a single entity that match the current specification.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Entity that matches the specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        public virtual TEntity SatisfyingEntityFrom(IQueryable<TEntity> query)
        {
            Argument.IsNotNull("query", query);

            return SatisfyingEntitiesFrom(query).FirstOrDefault();
        }

        /// <summary>
        /// Returns a queryable of entities that match the current specification.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable of entities that match the specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        public abstract IQueryable<TEntity> SatisfyingEntitiesFrom(IQueryable<TEntity> query);
        #endregion
    }
}