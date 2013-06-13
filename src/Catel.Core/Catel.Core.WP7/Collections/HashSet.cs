// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashSet.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Collections.Generic
{
    /// <summary>
    /// HashSet alternative because the HashSet class is not available in WP7.
    /// </summary>
    public class HashSet<T> : IEnumerable<T>
    {
        /// <summary>
        /// Internal dictionary to keep track of all stuff.
        /// </summary>
        private readonly Dictionary<T, short> _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class.
        /// </summary>
        public HashSet()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <remarks></remarks>
        public HashSet(IEnumerable<T> values)
        {
            _dictionary = new Dictionary<T, short>();

            if (values != null)
            {
                foreach (var value in values)
                {
                    Add(value);
                }
            }
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            _dictionary[item] = 0;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="HashSet{T}"/> contains the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the item is contained; otherwise, <c>false</c>.</returns>
        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        /// <summary>
        /// Throws a <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public bool Remove(T item)
        {
            return _dictionary.Remove(item);
        }

        /// <summary>
        /// Gets the enumerator that iterations through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <remarks></remarks>
        public int Count
        {
            get { return _dictionary.Keys.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}