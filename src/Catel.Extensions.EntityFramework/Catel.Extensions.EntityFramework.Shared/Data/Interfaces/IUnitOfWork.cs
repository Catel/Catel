// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Entity;
    using IoC;
    using Repositories;

#if EF_ASYNC
    using System.Threading.Tasks;
#endif

#if EF5
    using SaveOptions = System.Data.Objects.SaveOptions;
    using System.Data.Objects;
#else
    using SaveOptions = System.Data.Entity.Core.Objects.SaveOptions;
    using System.Data.Entity.Core.Objects;
#endif

    /// <summary>
    /// Interface defining a unit of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is currently in a transaction.
        /// </summary>
        /// <value><c>true</c> if this instance is currently in a transaction; otherwise, <c>false</c>.</value>
        bool IsInTransaction { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the repository that is created specificially for this unit of work.
        /// <para />
        /// Note that the following conditions must be met: <br />
        /// <list type="number">
        /// <item>
        /// <description>
        /// The container must be registered in the <see cref="ServiceLocator" /> as <see cref="RegistrationType.Transient" /> type. If the
        /// repository is declared as non-transient, it will be instantiated as new instance anyway.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// The repository must have a constructor accepting a <see cref="DbContext" /> instance.
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="TEntityRepository">The type of the entity repository.</typeparam>
        /// <returns>The entity repository.</returns>
        /// <exception cref="NotSupportedException">The specified repository type cannot be found.</exception>
        TEntityRepository GetRepository<TEntityRepository>()
            where TEntityRepository : IEntityRepository;

        /// <summary>
        /// Refreshes the collection inside the unit of work.
        /// </summary>
        /// <param name="refreshMode">The refresh mode.</param>
        /// <param name="collection">The collection.</param>
        void Refresh(RefreshMode refreshMode, IEnumerable collection);

#if EF_ASYNC
        /// <summary>
        /// Refreshes the collection inside the unit of work.
        /// </summary>
        /// <param name="refreshMode">The refresh mode.</param>
        /// <param name="collection">The collection.</param>
        Task RefreshAsync(RefreshMode refreshMode, IEnumerable collection);
#endif

        /// <summary>
        /// Refreshes the entity inside the unit of work.
        /// </summary>
        /// <param name="refreshMode">The refresh mode.</param>
        /// <param name="entity">The entity.</param>
        void Refresh(RefreshMode refreshMode, object entity);

#if EF_ASYNC
        /// <summary>
        /// Refreshes the entity inside the unit of work.
        /// </summary>
        /// <param name="refreshMode">The refresh mode.</param>
        /// <param name="entity">The entity.</param>
        Task RefreshAsync(RefreshMode refreshMode, object entity);
#endif

        /// <summary>
        /// Saves the changes inside the unit of work.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction is running. Call CommitTransaction instead.</exception>
        void SaveChanges();

#if EF_ASYNC
        /// <summary>
        /// Saves the changes inside the unit of work.
        /// </summary>
        /// <exception cref="InvalidOperationException">A transaction is running. Call CommitTransaction instead.</exception>
        Task SaveChangesAsync();
#endif

        /// <summary>
        /// Begins a new transaction on the unit of work.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Rolls back all the changes inside a transaction.
        /// </summary>
        void RollBackTransaction();

        /// <summary>
        /// Commits all the changes inside a transaction.
        /// </summary>
        void CommitTransaction();

#if EF_ASYNC
        /// <summary>
        /// Commits all the changes inside a transaction.
        /// </summary>
        Task CommitTransactionAsync();
#endif
        #endregion
    }
}