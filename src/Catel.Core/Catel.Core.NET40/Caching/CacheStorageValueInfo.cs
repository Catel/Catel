// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheStorageValueInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Caching
{
    using System;

    using Policies;

    /// <summary>
    /// Value info for the cache storage.
    /// </summary>
    /// <typeparam name="TValue">
    /// The value type.
    /// </typeparam>
    internal class CacheStorageValueInfo<TValue>
    {
        #region Fields

        /// <summary>
        /// The expiration policy.
        /// </summary>
        private readonly ExpirationPolicy _expirationPolicy;

        /// <summary>
        /// The value.
        /// </summary>
        private TValue _value;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheStorageValueInfo{TValue}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="expiration">
        /// The expiration.
        /// </param>
        public CacheStorageValueInfo(TValue value, TimeSpan expiration)
            : this(value, ExpirationPolicy.Duration(expiration))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheStorageValueInfo{TValue}"/> class.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="expirationPolicy">
        /// The expiration policy.
        /// </param>
        public CacheStorageValueInfo(TValue value, ExpirationPolicy expirationPolicy = null)
        {
            _value = value;
            _expirationPolicy = expirationPolicy;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value
        {
            get
            {
                if (CanExpire && _expirationPolicy.CanReset)
                {
                    _expirationPolicy.Reset();
                }

                return _value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this value can expire.
        /// </summary>
        /// <value><c>true</c> if this value can expire; otherwise, <c>false</c>.</value>
        public bool CanExpire
        {
            get
            {
                return _expirationPolicy != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this value is expired.
        /// </summary>
        /// <value><c>true</c> if this value is expired; otherwise, <c>false</c>.</value>
        public bool IsExpired
        {
            get
            {
                return CanExpire && _expirationPolicy.IsExpired;
            }
        }
        #endregion
    }
}