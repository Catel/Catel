// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityRepositoryExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data.Repositories
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

#if EF_ASYNC
    using System.Data.Entity;
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Extensions methods for entity repositories.
    /// </summary>
    public static class IEntityRepositoryExtensions
    {
        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static TEntity Single<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.Single();
        }

#if EF_ASYNC
        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static Task<TEntity> SingleAsync<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.SingleAsync();
        }
#endif

        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static TEntity SingleOrDefault<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.SingleOrDefault();
        }

#if EF_ASYNC
        /// <summary>
        /// Gets a single entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static Task<TEntity> SingleOrDefaultAsync<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.SingleOrDefaultAsync();
        }
#endif

        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static TEntity First<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.First();
        }

#if EF_ASYNC
        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static Task<TEntity> FirstAsync<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.FirstAsync();
        }
#endif

        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static TEntity FirstOrDefault<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.FirstOrDefault();
        }

#if EF_ASYNC
        /// <summary>
        /// Gets the first entity based on the matching criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The entity or <c>null</c> if no entity matches the criteria.</returns>
        public static Task<TEntity> FirstOrDefaultAsync<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.FirstOrDefaultAsync();
        }
#endif

        /// <summary>
        /// Counts entities with the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of entities that match the criteria.</returns>
        public static int Count<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.Count();
        }

#if EF_ASYNC
        /// <summary>
        /// Counts entities with the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The number of entities that match the criteria.</returns>
        public static Task<int> CountAsync<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            var query = repository.GetQuery(predicate);
            return query.CountAsync();
        }
#endif

        /// <summary>
        /// Deletes all entities that match the predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        public static void Delete<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);
            Argument.IsNotNull("predicate", predicate);

            var entities = repository.Find(predicate);

            foreach (var entity in entities)
            {
                repository.Delete(entity);
            }
        }

        /// <summary>
        /// Finds entities based on provided criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Enumerable of all matching entities.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> is <c>null</c>.</exception>
        public static IQueryable<TEntity> Find<TEntity>(this IEntityRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            return repository.GetQuery(predicate);
        }

        /// <summary>
        /// Gets all entities available in the repository.
        /// <para />
        /// Not that this method executes the default query returned by <see cref="IEntityRepository{TEntity}.GetQuery()" />/.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <returns>Enumerable of all entities.</returns>
        public static IQueryable<TEntity> GetAll<TEntity>(this IEntityRepository<TEntity> repository)
            where TEntity : class
        {
            Argument.IsNotNull("repository", repository);

            return repository.GetQuery();
        }

        /// <summary>
        /// Ensures a validate predicate.
        /// <para />
        /// If the <paramref name="predicate" /> is <c>null</c>, this method will create a default predicate which
        /// is always true.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The ensured valid predicate.</returns>
        public static Expression<Func<TEntity, bool>> GetValidPredicate<TEntity>(this Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                predicate = x => true;
            }

            return predicate;
        }
    }
}