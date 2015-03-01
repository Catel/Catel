// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedRepositoryBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using System.Threading;

    /// <summary>
    /// Extended base class for repositories with caching support.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class CachedRepositoryBase<TModel> : RepositoryBase<TModel>, ICachedRepository<TModel>
    {
        #region Fields
        private readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();

        private readonly Queue<Action<IEnumerable<TModel>>> _queuedCompletedHandlers = new Queue<Action<IEnumerable<TModel>>>();
        private List<TModel> _data;

        /// <summary>
        /// The timer that is being executed to invalidate the cache.
        /// </summary>
        private readonly Timer _timer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedRepositoryBase{TModel}" /> class.
        /// </summary>
        protected CachedRepositoryBase()
        {
            Expiration = new TimeSpan();
            DataLoadedTimestamp = DateTime.MinValue;

            _timer = new Timer(OnTimerElapsed, null, 1000, 1000);
        }
        #endregion

        #region ICachedRepository<TModel> Members
        /// <summary>
        /// Gets the current data. If <see cref="IsDataLoaded"/> is <c>true</c>, this property will return <c>null</c>.
        /// </summary>
        /// <value>The data.</value>
        public IEnumerable<TModel> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Gets a value indicating whether this repository is currently loading data.
        /// </summary>
        /// <value><c>true</c> if this repository is loading data; otherwise, <c>false</c>.</value>
        public bool IsLoadingData { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this repository has its data loaded.
        /// </summary>
        /// <value><c>true</c> if this repository has its data loaded; otherwise, <c>false</c>.</value>
        public bool IsDataLoaded { get; private set; }

        /// <summary>
        /// Gets the data loaded timestamp.
        /// </summary>
        /// <value>The data loaded timestamp.</value>
        public DateTime DataLoadedTimestamp { get; private set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public TimeSpan Expiration { get; set; }

        /// <summary>
        /// Gets the data. 
        /// <para />
        /// If the data is already loaded, it will immediately return.
        /// <para />
        /// If the data is currently loading, the handler will be queued and called as soon as the data is loaded.
        /// </summary>
        /// <param name="completed">The handler to call when the data is retrieved, can be <c>null</c>.</param>
        public override void GetData(Action<IEnumerable<TModel>> completed = null)
        {
            lock (_lock)
            {
                if (IsLoadingData)
                {
                    if (completed != null)
                    {
                        Log.Debug("Currently loading data, queued the handler to call it as soon as the data is loaded");

                        _queuedCompletedHandlers.Enqueue(completed);
                    }
                    return;
                }

                if (IsDataLoaded)
                {
                    if (completed != null)
                    {
                        completed(_data);
                    }
                    return;
                }

                IsLoadingData = true;

                if (completed != null)
                {
                    _queuedCompletedHandlers.Enqueue(completed);
                }

                RetrieveData(OnRetrieveDataCompleted);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retrieves the data from the actual data source.
        /// <para />
        /// This is the only method that needs to be implemented by the deriving type.
        /// </summary>
        /// <param name="completed">The completed, is never <c>null</c>.</param>
        protected abstract void RetrieveData(Action<IEnumerable<TModel>> completed);

        /// <summary>
        /// Called when the retrieval of the items has completed.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="items"/> is <c>null</c>.</exception>
        private void OnRetrieveDataCompleted(IEnumerable<TModel> items)
        {
            Argument.IsNotNull("items", items);

            lock (_lock)
            {
                IsLoadingData = false;
                IsDataLoaded = true;

                DataLoadedTimestamp = DateTime.Now;

                _data = new List<TModel>(items);

                while (_queuedCompletedHandlers.Count > 0)
                {
                    var callback = _queuedCompletedHandlers.Dequeue();
                    callback(_data);
                }
            }
        }

        /// <summary>
        /// Called when the timer to clean up the cache elapsed.
        /// </summary>
        /// <param name="state">The timer state.</param>
        private void OnTimerElapsed(object state)
        {
            CheckForCacheExpiration();
        }

        /// <summary>
        /// Checks for cache expiration. If the cache is expired, the data will be removed and reloaded.
        /// </summary>
        private void CheckForCacheExpiration()
        {
            lock (_lock)
            {
                if (IsLoadingData || !IsDataLoaded)
                {
                    return;
                }

                if (Expiration.Ticks <= 0)
                {
                    return;
                }

                var isExpired = DataLoadedTimestamp.Add(Expiration) < DateTime.Now;
                if (isExpired)
                {
                    Log.Info("Cached repository for '{0}' has expired, clearing cached data", typeof(TModel).FullName);

                    _data = null;

                    IsDataLoaded = false;
                    DataLoadedTimestamp = DateTime.MinValue;
                }
            }
        }
        #endregion
    }
}