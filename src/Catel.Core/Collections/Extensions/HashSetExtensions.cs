namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Hash set extensions.
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Adds the range of items to the hash set.
        /// </summary>
        /// <typeparam name="T">Type of the hash set.</typeparam>
        /// <param name="hashSet">The hash set.</param>
        /// <param name="items">The items.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="hashSet"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="items"/> is <c>null</c>.</exception>
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }
    }
}
