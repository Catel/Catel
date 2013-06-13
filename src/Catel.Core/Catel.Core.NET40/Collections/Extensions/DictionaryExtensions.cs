// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions for the <see cref="Dictionary{TKey,TValue}"/> class.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds all items from the source into the target dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        /// <param name="overwriteExisting">if set to <c>true</c>, existing items in the target dictionary will be overwritten.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValue> source, bool overwriteExisting = true)
        {
            Argument.IsNotNull("target", target);
            Argument.IsNotNull("source", source);

            foreach (var keyValuePair in source)
            {
                if (!overwriteExisting)
                {
                    if (target.ContainsKey(keyValuePair.Key))
                    {
                        continue;
                    }
                }

                target[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        /// <summary>
        /// Adds the specified value using the key if the value is not <c>null</c> or whitespace.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to check and to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is <c>null</c>.</exception>
        public static void AddItemIfNotEmpty<TKey>(this Dictionary<TKey, string> dictionary, TKey key, string value)
        {
            Argument.IsNotNull("dictionary", dictionary);
            Argument.IsNotNull("key", key);

            if (!string.IsNullOrEmpty(value))
            {
                dictionary[key] = value;
            }
        }
    }
}