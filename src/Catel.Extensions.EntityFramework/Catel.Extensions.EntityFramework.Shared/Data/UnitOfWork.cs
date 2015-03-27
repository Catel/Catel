// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.Data.Objects;
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Globalization;
    using IoC;
    using Logging;
    using Repositories;

#if EF5
    using SaveOptions = System.Data.Objects.SaveOptions;
    using System.Collections;
#else
    using SaveOptions = System.Data.Entity.Core.Objects.SaveOptions;
#endif

    /// <summary>
    /// Implementation of the unit of work pattern for entity framework.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        private bool _disposed;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tag">The tag to uniquely identify this unit of work. If <c>null</c>, a unique id will be generated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
        public UnitOfWork(DbContext context, string tag = null)
        {
            Argument.IsNotNull("context", context);

            _serviceLocator = ServiceLocator.Default;
            _typeFactory = _serviceLocator.ResolveType<ITypeFactory>();

            DbContext = context;
            Tag = tag ?? UniqueIdentifierHelper.GetUniqueIdentifier<UnitOfWork>().ToString(CultureInfo.InvariantCulture);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <value>The db context.</value>
        protected DbContext DbContext { get; private set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        protected string Tag { get; private set; }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        protected DbTransaction Transaction { get; set; }
        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a value indicating whether this instance is currently in a transaction.
        /// </summary>
        /// <value><c>true</c> if this instance is currently in a transaction; otherwise, <c>false</c>.</value>
        public bool IsInTransaction
        {
            get { return Transaction != null; }
        }

        /// <summary>
        /// Begins a new transaction on the unit of work.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <exception cref="InvalidOperationException">A transaction is already running.</exception>
        public virtual void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            Log.Debug("Beginning transaction | {0}", Tag);

            if (Transaction != null)
            {
                const string error = "Cannot begin a new transaction while an existing transaction is still running. " +
                               "Please commit or rollback the existing transaction before starting a new one.";

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            OpenConnection();

            var objectContext = DbContext.GetObjectContext();
            Transaction = objectContext.Connection.BeginTransaction(isolationLevel);

            Log.Debug("Began transaction | {0}", Tag);
        }

        /// <summary>
        /// Rolls back all the changes inside a transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">No transaction is currently running.</exception>
        public virtual void RollBackTransaction()
        {
            Log.Debug("Rolling back transaction | {0}", Tag);

            if (Transaction == null)
            {
                const string error = "Cannot roll back a transaction when there is no transaction running.";

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            Transaction.Rollback();
            ReleaseTransaction();

            Log.Debug("Rolling back transaction | {0}", Tag);
        }

        /// <summary>
        /// Commits all the changes inside a transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">No transaction is currently running.</exception>
        public virtual void CommitTransaction()
        {
            Log.Debug("Committing transaction | {0}", Tag);

            if (Transaction == null)
            {
                const string error = "Cannot commit a transaction when there is no transaction running.";

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            try
            {
                var objectContext = DbContext.GetObjectContext();
                objectContext.SaveChanges();

                Transaction.Commit();

                ReleaseTransaction();

                Log.Debug("Committed transaction | {0}", Tag);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An exception occurred while committing the transaction, automatically rolling back | {0}", Tag);

                RollBackTransaction();
                throw;
            }
        }

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
        public virtual TEntityRepository GetRepository<TEntityRepository>()
            where TEntityRepository : IEntityRepository
        {
            var registrationInfo = _serviceLocator.GetRegistrationInfo(typeof(TEntityRepository));
            if (registrationInfo == null)
            {
                string error = string.Format("The specified repository type '{0}' cannot be found. Make sure it is registered in the ServiceLocator.", typeof(TEntityRepository).FullName);
                Log.Error(error);
                throw new NotSupportedException(error);
            }

            var repository = _typeFactory.CreateInstanceWithParameters(registrationInfo.ImplementingType, DbContext);
            return (TEntityRepository)repository;
        }

        /// <summary>
        /// Refreshes the collection inside the unit of work.
        /// </summary>
        /// <param name="refreshMode">The refresh mode.</param>
        /// <param name="collection">The collection.</param>
        public virtual void Refresh(RefreshMode refreshMode, IEnumerable collection)
        {
            Log.Debug("Refreshing collection | {0}", Tag);

            var objectContext = DbContext.GetObjectContext();
            objectContext.Refresh(refreshMode, collection);

            Log.Debug("Refreshed collection | {0}", Tag);
        }

        /// <summary>
        /// Refreshes the entity inside the unit of work.
        /// </summary>
        /// <param name="refreshMode">The refresh mode.</param>
        /// <param name="entity">The entity.</param>
        public virtual void Refresh(RefreshMode refreshMode, object entity)
        {
            Log.Debug("Refreshing entity | {0}", Tag);

            var objectContext = DbContext.GetObjectContext();
            objectContext.Refresh(refreshMode, entity);

            Log.Debug("Refreshed entity | {0}", Tag);
        }

        /// <summary>
        /// Saves the changes inside the unit of work.
        /// </summary>
        /// <param name="saveOptions">The save options.</param>
        /// <exception cref="InvalidOperationException">A transaction is running. Call CommitTransaction instead.</exception>
        public virtual void SaveChanges(SaveOptions saveOptions = SaveOptions.DetectChangesBeforeSave | SaveOptions.AcceptAllChangesAfterSave)
        {
            Log.Debug("Saving changes | {0}", Tag);

            if (IsInTransaction)
            {
                const string error = "A transaction is running. Call CommitTransaction instead.";

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            var objectContext = DbContext.GetObjectContext();
            objectContext.SaveChanges(saveOptions);

            Log.Debug("Saved changes | {0}", Tag);
        }
        #endregion

        #region Implementation of IDisposable
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (_disposed)
            {
                return;
            }

            OnDisposing();

            _disposed = true;
        }

        /// <summary>
        /// Called when the object is being disposed.
        /// </summary>
        protected virtual void OnDisposing()
        {
        }

        /// <summary>
        /// Disposes the db context.
        /// </summary>
        protected void DisposeDbContext()
        {
            if (DbContext != null)
            {
                DbContext.Dispose();
            }
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        protected virtual void OpenConnection()
        {
            var objectContext = DbContext.GetObjectContext();
            if (objectContext.Connection.State != ConnectionState.Open)
            {
                Log.Debug("Opening connection | {0}", Tag);

                objectContext.Connection.Open();

                Log.Debug("Opened connection | {0}", Tag);
            }
        }

        /// <summary>
        /// Releases the transaction.
        /// </summary>
        protected virtual void ReleaseTransaction()
        {
            if (Transaction != null)
            {
                Log.Debug("Releasing transaction | {0}", Tag);

                Transaction.Dispose();
                Transaction = null;

                Log.Debug("Released transaction | {0}", Tag);
            }
        }
        #endregion
    }
}