// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Specification.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Specifications
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Expressions;

    /// <summary>
    /// Generic specification class that serves as base for customized specifications.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class Specification<TEntity> : ISpecification<TEntity>
    {
        #region Fields
        private readonly Lazy<Func<TEntity, bool>> _compiledPredicate;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Specification{TEntity}" /> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public Specification(Expression<Func<TEntity, bool>> predicate)
        {
            Argument.IsNotNull("predicate", predicate);

            Predicate = predicate;
            _compiledPredicate = new Lazy<Func<TEntity, bool>>(() => Predicate.Compile());
        }
        #endregion

        #region Operators
        /// <summary>
        /// Performs an implicit conversion from <see cref="Specification{TEntity}" /> to <see cref="Func{TEntity, Boolean}" />.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Func<TEntity, bool>(Specification<TEntity> specification)
        {
            return specification._compiledPredicate.Value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the predicate used for this specification.
        /// </summary>
        /// <value>The predicate.</value>
        public Expression<Func<TEntity, bool>> Predicate { get; private set; }
        #endregion

        #region ISpecification<TEntity> Members
        /// <summary>
        /// Returns a single entity that match the current specification.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Entity that matches the specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        public TEntity SatisfyingEntityFrom(IQueryable<TEntity> query)
        {
            Argument.IsNotNull("query", query);

            return query.Where(Predicate).SingleOrDefault();
        }

        /// <summary>
        /// Returns a queryable of entities that match the current specification.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable of entities that match the specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        public IQueryable<TEntity> SatisfyingEntitiesFrom(IQueryable<TEntity> query)
        {
            Argument.IsNotNull("query", query);

            return query.Where(Predicate);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new specification based on the current one combined with the specified specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>New specification with the current specification combined with the specified specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="specification"/> is <c>null</c>.</exception>
        public Specification<TEntity> And(Specification<TEntity> specification)
        {
            Argument.IsNotNull("specification", specification);

            return new Specification<TEntity>(Predicate.And(specification.Predicate));
        }

        /// <summary>
        /// Creates a new specification based on the current one combined with the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>New specification with the current specification combined with the specified predicate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public Specification<TEntity> And(Expression<Func<TEntity, bool>> predicate)
        {
            Argument.IsNotNull("predicate", predicate);

            return new Specification<TEntity>(Predicate.And(predicate));
        }

        /// <summary>
        /// Creates a new specification based on the current one combined with the specified specification.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns>New specification with the current specification combined with the specified specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="specification"/> is <c>null</c>.</exception>
        public Specification<TEntity> Or(Specification<TEntity> specification)
        {
            Argument.IsNotNull("specification", specification);

            return new Specification<TEntity>(Predicate.Or(specification.Predicate));
        }

        /// <summary>
        /// Creates a new specification based on the current one combined with the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>New specification with the current specification combined with the specified predicate.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public Specification<TEntity> Or(Expression<Func<TEntity, bool>> predicate)
        {
            Argument.IsNotNull("predicate", predicate);

            return new Specification<TEntity>(Predicate.Or(predicate));
        }
        #endregion
    }
}