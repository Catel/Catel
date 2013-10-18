// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalWeakTable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace System.Runtime.CompilerServices
{
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of the ConditionalWeakTable because it is not available on Windows Phone 7.x.
    /// <para />
    /// Note that this implementation leaks memory because internally it uses a dictionary. If you want this memory
    /// leak to be solved, we would love to see a pull request ;-)
    /// </summary>
    public class ConditionalWeakTable<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        /// <summary>
        /// Tries to get the value from the weak table.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is successfully retrieved, <c>false</c> otherwise.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
    }
}