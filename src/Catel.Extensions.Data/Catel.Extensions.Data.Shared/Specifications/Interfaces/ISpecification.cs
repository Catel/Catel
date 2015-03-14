// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpecification.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Specifications
{
    using System;
    using System.Linq;

    /// <summary>
    /// Definition of the default specification pattern logic.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ISpecification<TEntity>
    {
        #region Methods
        /// <summary>
        /// Returns a single entity that match the current specification.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Entity that matches the specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        TEntity SatisfyingEntityFrom(IQueryable<TEntity> query);

        /// <summary>
        /// Returns a queryable of entities that match the current specification.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable of entities that match the specification.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="query"/> is <c>null</c>.</exception>
        IQueryable<TEntity> SatisfyingEntitiesFrom(IQueryable<TEntity> query);
        #endregion
    }
}