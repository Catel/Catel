// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpiringEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Caching
{
    using Policies;
    using System;

    /// <summary>
    /// The expiring event args.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class ExpiringEventArgs<TKey, TValue> : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiringEventArgs{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationPolicy">The expiration policy.</param>
        public ExpiringEventArgs(TKey key, TValue value, ExpirationPolicy expirationPolicy)
        {
            Cancel = false;
            ExpirationPolicy = expirationPolicy;
            Key = key;
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the expiration of value should be canceled and the value should stay in cache.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the expiration policy.
        /// </summary>
        public ExpirationPolicy ExpirationPolicy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public TKey Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValue Value
        {
            get;
            private set;
        }

        #endregion
    }
}
