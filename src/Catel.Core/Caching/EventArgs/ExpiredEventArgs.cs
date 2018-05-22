// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpiredEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Caching
{
    using System;

    /// <summary>
    /// The expired event args.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class ExpiredEventArgs<TKey, TValue> : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpiredEventArgs{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="dispose">The value indicating whether the expired value should be disposed after removal from cache.</param>
        public ExpiredEventArgs(TKey key, TValue value, bool dispose)
        {
            Dispose = dispose;
            Key = key;
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the expired value should be disposed after removal from cache.
        /// </summary>
        /// <value><c>true</c> if item should be disposed; otherwise, <c>false</c>.</value>
        /// <remarks>Default value of this property is equal to <see cref="ICacheStorage{TKey, TValue}.DisposeValuesOnRemoval"/> value.</remarks>
        public bool Dispose
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
