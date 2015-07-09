// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityRepositoryBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

#if EF5
    using EntityKey = System.Data.EntityKey;
#else
    using EntityKey = System.Data.Entity.Core.EntityKey;
#endif

    /// <summary>
    /// Base class for entity repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    public class EntityRepositoryBase<TEntity, TPrimaryKey> : IEntityRepository<TEntity, TPrimaryKey>
        where TEntity : class
    {
        #region Fields
        private readonly DbContext _dbContext;

        private readonly string _entitySetName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRepositoryBase{TEntity, TPrimaryKey}" /> class.
        /// </summary>
        /// <param name="dbContext">The db context. The caller is responsible for correctly disposing the <see cref="DbContext"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        public EntityRepositoryBase(DbContext dbContext)
        {
            Argument.IsNotNull("dbContext", dbContext);

            _dbContext = dbContext;

            _entitySetName = dbContext.GetEntitySetName<TEntity>();
        }
        #endregion

        #region IEntityRepository<TEntity,TPrimaryKey> Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets a specific entity by it's primary key value.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <returns>The entity or <c>null</c> if the entity could not be found.</returns>
        public virtual TEntity GetByKey(TPrimaryKey keyValue)
        {
            var key = _dbContext.GetEntityKey<TEntity>(keyValue);
            var objectContext = _dbContext.GetObjectContext();

            object originalItem;
            if (objectContext.TryGetObjectByKey(key, out originalItem))
            {
                return (TEntity)originalItem;
            }

            return null;
        }

        /// <summary>
        /// Gets the default query for this repository.
        /// </summary>
        /// <returns>The default queryable for this repository.</returns>
        public virtual IQueryable<TEntity> GetQuery()
        {
            var objectContext = _dbContext.GetObjectContext();
            return objectContext.CreateQuery<TEntity>(_entitySetName);
        }

        /// <summary>
        /// Gets a customized query for this repository.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The customized queryable for this repository.</returns>
        public virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> predicate)
        {
            var query = GetQuery();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }

        /// <summary>
        /// Gets an new entity instance, which may be a proxy if the entity meets the proxy requirements and the underlying context is configured to create proxies.
        /// <para />
        /// Note that the returned proxy entity is NOT added or attached to the set.
        /// </summary>
        /// <returns>The proxy entity</returns>
        public virtual TEntity Create()
        {
            return _dbContext.Set<TEntity>().Create();
        }

        /// <summary>
        /// Adds the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity"/> is <c>null</c>.</exception>
        public virtual void Add(TEntity entity)
        {
            Argument.IsNotNull("entity", entity);

            _dbContext.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Attaches the specified entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Attach(TEntity entity)
        {
            Argument.IsNotNull("entity", entity);

            _dbContext.Set<TEntity>().Attach(entity);
        }

        /// <summary>
        /// Deletes the specified entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Delete(TEntity entity)
        {
            Argument.IsNotNull("entity", entity);

            _dbContext.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Updates changes of the existing entity.
        /// <para />
        /// Note that this method does not actually call <c>SaveChanges</c>, but only updates the entity in the repository.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entity" /> is <c>null</c>.</exception>
        public virtual void Update(TEntity entity)
        {
            Argument.IsNotNull("entity", entity);

            var objectContext = _dbContext.GetObjectContext();

            object originalItem;
            var key = objectContext.CreateEntityKey(_entitySetName, entity);
            if (objectContext.TryGetObjectByKey(key, out originalItem))
            {
                objectContext.ApplyCurrentValues(key.EntitySetName, entity);
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
