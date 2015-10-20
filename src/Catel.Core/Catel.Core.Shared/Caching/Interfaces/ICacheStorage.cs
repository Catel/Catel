// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheStorage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Policies;

    /// <summary>
    /// The cache storage interface.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface ICacheStorage<TKey, TValue>
    {
        #region Properties
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the specified key, or default value for the type of the value if the key do not exists.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Gets the keys so it is possible to enumerate the cache.
        /// </summary>
        /// <value>The keys.</value>
        IEnumerable<TKey> Keys { get; }

        /// <summary>
        /// Gets or sets the expiration timer interval.
        /// <para />
        /// The default value is <c>TimeSpan.FromSeconds(1)</c>.
        /// </summary>
        /// <value>The expiration timer interval.</value>
        TimeSpan ExpirationTimerInterval { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the value associated with the specified key
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key, or default value for the type of the value if the key do not exists.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        TValue Get(TKey key);

        /// <summary>
        /// Determines whether the cache contains a value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the cache contains an element with the specified key; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        bool Contains(TKey key);

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        TValue GetFromCacheOrFetch(TKey key, Func<TValue> code, bool @override = false, TimeSpan expiration = default(TimeSpan));

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="expirationPolicy">The expiration policy</param>
        /// <param name="override">Indicates if the key exists the value will be overridden</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        TValue GetFromCacheOrFetch(TKey key, Func<TValue> code, ExpirationPolicy expirationPolicy, bool @override = false);

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        void Add(TKey key, TValue @value, bool @override = false, TimeSpan expiration = default(TimeSpan));

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="action">The action that need to be executed in synchronization with the item cache removal.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        void Remove(TKey key, Action action = null);

        /// <summary>
        /// Adds a value to the cache associated with to a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationPolicy">The expiration policy</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key" /> is <c>null</c>.</exception>
        void Add(TKey key, TValue @value, ExpirationPolicy expirationPolicy, bool @override = false);

        /// <summary>
        /// Clears all the items currently in the cache.
        /// </summary>
        void Clear();
        #endregion

        /// <summary>
        /// Adds a value to the cache associated with to a key asynchronously.
        /// <para />
        /// Note that this is a wrapper around <see cref="CacheStorage{TKey,TValue}.GetFromCacheOrFetch(TKey,System.Func{TValue},Catel.Caching.Policies.ExpirationPolicy,bool)"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="expirationPolicy">The expiration policy.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        Task<TValue> GetFromCacheOrFetchAsync(TKey key, Func<TValue> code, ExpirationPolicy expirationPolicy, bool @override = false);

        /// <summary>
        /// Adds a value to the cache associated with to a key asynchronously.
        /// <para />
        /// Note that this is a wrapper around <see cref="CacheStorage{TKey,TValue}.GetFromCacheOrFetch(TKey,System.Func{TValue},bool,System.TimeSpan)"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        Task<TValue> GetFromCacheOrFetchAsync(TKey key, Func<TValue> code, bool @override = false, TimeSpan expiration = default(TimeSpan));

        /// <summary>
        /// Adds a value to the cache associated with to a key asynchronously.
        /// <para />
        /// Note that this is a wrapper around <see cref="CacheStorage{TKey,TValue}.GetFromCacheOrFetch(TKey,System.Func{TValue},Catel.Caching.Policies.ExpirationPolicy,bool)"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="expirationPolicy">The expiration policy.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        Task<TValue> GetFromCacheOrFetchAsync(TKey key, Func<Task<TValue>> code, ExpirationPolicy expirationPolicy, bool @override = false);

        /// <summary>
        /// Adds a value to the cache associated with to a key asynchronously.
        /// <para />
        /// Note that this is a wrapper around <see cref="CacheStorage{TKey,TValue}.GetFromCacheOrFetch(TKey,System.Func{TValue},bool,System.TimeSpan)"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The deferred initialization code of the value.</param>
        /// <param name="override">Indicates if the key exists the value will be overridden.</param>
        /// <param name="expiration">The timespan in which the cache item should expire when added.</param>
        /// <returns>The instance initialized by the <paramref name="code" />.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="code" /> is <c>null</c>.</exception>
        Task<TValue> GetFromCacheOrFetchAsync(TKey key, Func<Task<TValue>> code, bool @override = false, TimeSpan expiration = default(TimeSpan));
    }
}