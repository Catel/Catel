// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheStorage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Policies;

    /// <summary>
    /// The cache storage.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class CacheStorage<TKey, TValue> : ICacheStorage<TKey, TValue>
    {
        #region Fields
        private readonly Func<ExpirationPolicy> _defaultExpirationPolicyInitCode;

        /// <summary>
        /// Determines whether the cache storage can store null values.
        /// </summary>
        private readonly bool _storeNullValues;

        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly Dictionary<TKey, CacheStorageValueInfo<TValue>> _dictionary;

        /// <summary>
        /// The synchronization object.
        /// </summary>
        private readonly object _syncObj = new object();

        /// <summary>
        /// The synchronization objects.
        /// </summary>
        private readonly Dictionary<TKey, object> _syncObjs = new Dictionary<TKey, object>();

        /// <summary>
        /// The timer that is being executed to invalidate the cache.
        /// </summary>
        private Timer _expirationTimer;

        /// <summary>
        /// The expiration timer interval.
        /// </summary>
        private TimeSpan _expirationTimerInterval;

        /// <summary>
        /// Determines whether the cache storage can check for expired items.
        /// </summary>
        private bool _checkForExpiredItems;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheStorage{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="defaultExpirationPolicyInitCode">The default expiration policy initialization code.</param>
        /// <param name="storeNullValues">Allow store null values on the cache.</param>
        public CacheStorage(Func<ExpirationPolicy> defaultExpirationPolicyInitCode = null, bool storeNullValues = false)
        {
            _dictionary = new Dictionary<TKey, CacheStorageValueInfo<TValue>>();
            _storeNullValues = storeNullValues;
            _defaultExpirationPolicyInitCode = defaultExpirationPolicyInitCode;

            _expirationTimerInterval = TimeSpan.FromSeconds(1);
        }
        #endregion

        #region ICacheStorage<TKey,TValue> Members
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the specified key, or default value for the type of the value if the key do not exists.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        public TValue this[TKey key]
        {
            get { return Get(key); }
        }

        /// <summary>
        /// Gets the keys so it is possible to enumerate the cache.
        /// </summary>
        /// <value>The keys.</value>
        public IEnumerable<TKey> Keys
        {
            get
            {
                lock (_syncObj)
                {
                    return _dictionary.Keys;
                }
            }
        }

        /// <summary>
        /// Gets or sets the expiration timer interval.
        /// <para />
        /// The default value is <c>TimeSpan.FromSeconds(1)</c>.
        /// </summary>
        /// <value>The expiration timer interval.</value>
        public TimeSpan ExpirationTimerInterval
        {
            get { return _expirationTimerInterval; }
            set
            {
                _expirationTimerInterval = value;
                UpdateTimer();
            }
        }

        private void UpdateTimer()
        {
            lock (_syncObj)
            {
                if (!_checkForExpiredItems)
                {
                    if (_expirationTimer != null)
                    {
                        _expirationTimer.Dispose();
                        _expirationTimer = null;
                    }
                }
                else
                {
                    var timeSpan = _expirationTimerInterval;

                    if (_expirationTimer == null)
                    {
                        _expirationTimer = new Timer(OnTimerElapsed, null, timeSpan, timeSpan);
                    }
                    else
                    {
                        _expirationTimer.Change(timeSpan, timeSpan);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key, or default value for the type of the value if the key do not exists.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        public TValue Get(TKey key)
        {
            Argument.IsNotNull("key", key);

            CacheStorageValueInfo<TValue> valueInfo;
            lock (GetLockByKey(key))
            {
                _dictionary.TryGetValue(key, out valueInfo);
            }

            return (valueInfo != null) ? valueInfo.Value : default(TValue);
        }

        /// <summary>
        /// Determines whether the cache contains a value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the cache contains an element with the specified key; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        public bool Contains(TKey key)
        {
            Argument.IsNotNull("key", key);

            lock (GetLockByKey(key))
            {
                return _dictionary.ContainsKey(key);
            }
        }

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="expirationPolicy">The expiration policy.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1027:TabsMustNotBeUsed", Justification = "Reviewed. Suppression is OK here.")]
        public TValue GetFromCacheOrFetch(TKey key, Func<TValue> code, ExpirationPolicy expirationPolicy, bool @override = false)
        {
            Argument.IsNotNull("key", key);
            Argument.IsNotNull("code", code);

            TValue value;
            lock (GetLockByKey(key))
            {
                bool containsKey = _dictionary.ContainsKey(key);
                if (!containsKey || @override)
                {
                    value = code.Invoke();
                    if (!ReferenceEquals(value, null) || _storeNullValues)
                    {
                        if (expirationPolicy == null && _defaultExpirationPolicyInitCode != null)
                        {
                            expirationPolicy = _defaultExpirationPolicyInitCode.Invoke();
                        }

                        var valueInfo = new CacheStorageValueInfo<TValue>(value, expirationPolicy);
                        lock (_syncObj)
                        {
                            _dictionary[key] = valueInfo;
                        }

                        if (valueInfo.CanExpire)
                        {
                            _checkForExpiredItems = true;
                        }

                        if (expirationPolicy != null)
                        {
                            if (_expirationTimer == null)
                            {
                                UpdateTimer();
                            }
                        }
                    }
                }
                else
                {
                    value = _dictionary[key].Value;
                }
            }

            return value;
        }

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        public TValue GetFromCacheOrFetch(TKey key, Func<TValue> code, bool @override = false, TimeSpan expiration = default(TimeSpan))
        {
            return GetFromCacheOrFetch(key, code, ExpirationPolicy.Duration(expiration), @override);
        }

        /// <summary>
        /// Adds a value to the cache associated with to a key asynchronously.
        /// <para />
        /// Note that this is a wrapper around <see cref="GetFromCacheOrFetch(TKey,System.Func{TValue},ExpirationPolicy,bool)"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="expirationPolicy">The expiration policy.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        public async Task<TValue> GetFromCacheOrFetchAsync(TKey key, Func<TValue> code, ExpirationPolicy expirationPolicy, bool @override = false)
        {
            return await Task.Factory.StartNew(() => GetFromCacheOrFetch(key, code, expirationPolicy, @override));
        }

        /// <summary>
        /// Adds a value to the cache associated with to a key asynchronously.
        /// <para />
        /// Note that this is a wrapper around <see cref="GetFromCacheOrFetch(TKey,System.Func{TValue},bool,TimeSpan)"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        public async Task<TValue> GetFromCacheOrFetchAsync(TKey key, Func<TValue> code, bool @override = false, TimeSpan expiration = default(TimeSpan))
        {
            return await Task.Factory.StartNew(() => GetFromCacheOrFetch(key, code, @override, expiration));
        }

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        public void Add(TKey key, TValue @value, bool @override = false, TimeSpan expiration = default(TimeSpan))
        {
            Add(key, value, ExpirationPolicy.Duration(expiration), @override);
        }

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationPolicy">The expiration policy.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        public void Add(TKey key, TValue @value, ExpirationPolicy expirationPolicy, bool @override = false)
        {
            Argument.IsNotNull("key", key);

            if (!_storeNullValues)
            {
                Argument.IsNotNull("value", value);
            }

            GetFromCacheOrFetch(key, () => @value, expirationPolicy, @override);
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The action that need to be executed in synchronization with the item cache removal.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        public void Remove(TKey key, Action action = null)
        {
            Argument.IsNotNull("key", key);

            lock (GetLockByKey(key))
            {
                if (_dictionary.ContainsKey(key))
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }

                    _dictionary.Remove(key);
                }
            }
        }

        /// <summary>
        /// Clears all the items currently in the cache.
        /// </summary>
        public void Clear()
        {
            var keysToRemove = new List<TKey>();
            lock (_syncObj)
            {
                keysToRemove.AddRange(_dictionary.Keys);

                foreach (var keyToRemove in keysToRemove)
                {
                    lock (GetLockByKey(keyToRemove))
                    {
                        _dictionary.Remove(keyToRemove);
                    }
                }

                _checkForExpiredItems = false;

                UpdateTimer();
            }
        }

        /// <summary>
        /// Removes the expired items from the cache.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1409:RemoveUnnecessaryCode", Justification = "Reviewed. Suppression is OK here.")]
        private void RemoveExpiredItems()
        {
            bool containsItemsThatCanExpire = false;

            var keysToRemove = new List<TKey>();

            lock (_syncObj)
            {
                foreach (var cacheItem in _dictionary)
                {
                    var valueInfo = cacheItem.Value;
                    if (valueInfo.IsExpired)
                    {
                        keysToRemove.Add(cacheItem.Key);
                    }
                    else
                    {
                        if (!containsItemsThatCanExpire && valueInfo.CanExpire)
                        {
                            containsItemsThatCanExpire = true;
                        }
                    }
                }
            }

            foreach (var keyToRemove in keysToRemove)
            {
                lock (GetLockByKey(keyToRemove))
                {
                    _dictionary.Remove(keyToRemove);
                }
            }

            lock (_syncObj)
            {
                if (_checkForExpiredItems != containsItemsThatCanExpire)
                {
                    _checkForExpiredItems = containsItemsThatCanExpire;
                    UpdateTimer();
                }
            }
        }

        /// <summary>
        /// Gets the lock by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The lock object.</returns>
        private object GetLockByKey(TKey key)
        {
            lock (_syncObj)
            {
                var containsKey = _syncObjs.ContainsKey(key);
                if (!containsKey)
                {
                    _syncObjs[key] = new object();
                }
            }

            return _syncObjs[key];
        }

        /// <summary>
        /// Called when the timer to clean up the cache elapsed.
        /// </summary>
        /// <param name="state">The timer state.</param>
        private void OnTimerElapsed(object state)
        {
            if (!_checkForExpiredItems)
            {
                return;
            }

            RemoveExpiredItems();
        }
        #endregion
    }
}