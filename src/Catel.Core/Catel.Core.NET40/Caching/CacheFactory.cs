// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Caching
{
    /// <summary>
    /// The cache factory.
    /// </summary>
    [ObsoleteEx(Replacement = "CacheStorage constructor", TreatAsErrorFromVersion = "3.6", RemoveInVersion = "4.0")]
    public static class CacheFactory
    {
        #region Methods
        /// <summary>
        /// Creates a new cache storage.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>An implementation of <see cref="ICacheStorage{TKey,TValue}"/>.</returns>
        public static ICacheStorage<TKey, TValue> Create<TKey, TValue>()
        {
            return new CacheStorage<TKey, TValue>();
        }
        #endregion
    }
}