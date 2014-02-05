// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRepository.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Entity repository.
    /// </summary>
    public interface IEntityRepository : IDisposable
    {
    }

    /// <summary>
    /// Entity repository with a specific key type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IEntityRepository<TEntity> : IEntityRepository
        where TEntity : class
    {
        #region Methods
        /// <summary>
        /// Gets the default query for this repository.
        /// </summary>
        /// <returns>The default queryable for this repository.</returns>
        IQueryable<TEntity> GetQuery();

        /// <summary>
        /// Gets a customized query for this repository.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The customized queryable for this repository.</returns>
        IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        TEntity First(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        void Add(TEntity entity);

        /// <summary>
        /// Attaches the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        void Attach(TEntity entity);

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all entities that match the predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Updates changes of the existing entity.
        /// <para />
        /// Note that this method does not actually call <c>SaveChanges</c>, but only updates the entity in the repository.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        void Update(TEntity entity);

        /// <summary>
        /// Finds entities based on provided criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Enumerable of all matching entities.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets all entities available in the repository.
        /// <para />
        /// Not that this method executes the default query returned by <see cref="GetQuery()" />/.
        /// </summary>
        /// <returns>Enumerable of all entities.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Counts entities with the specified criteria.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of entities that match the criteria.</returns>
        int Count(Expression<Func<TEntity, bool>> predicate = null);
        #endregion
    }

    /// <summary>
    /// Entity repository with a specific key type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    public interface IEntityRepository<TEntity, TPrimaryKey> : IEntityRepository<TEntity>
        where TEntity : class
    {
        #region Methods
        /// <summary>
        /// Gets a specific entity by it's primary key value.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <returns>The entity or <c>null</c> if the entity could not be found.</returns>
        TEntity GetByKey(TPrimaryKey keyValue);
        #endregion
    }
}