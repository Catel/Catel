// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListDictionary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implements <see cref="System.Collections.Generic.IDictionary{TKey, TValue}"/> using a singly linked list. Recommended for collections that typically include fewer than 10 items.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public class ListDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Fields
        private readonly IList<KeyValuePair<TKey, TValue>> _list;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ListDictionary{TKey, TValue}"/> class.
        /// </summary>
        public ListDictionary()
        {
            _list = new List<KeyValuePair<TKey, TValue>>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The property is retrieved and key is not found.</exception>
        public TValue this[TKey key]
        {
            get
            {
                var keyValuePair = _list.FirstOrDefault(item => Equals(item.Key, key));
                if (Equals(keyValuePair, default(KeyValuePair<TKey, TValue>)))
                {
                    throw new KeyNotFoundException();
                }

                return keyValuePair.Value;
            }
            set
            {
                var index = RemoveAndGetIndex(key);
                var keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
                if (index >= 0)
                {
                    _list.Insert(index, keyValuePair);
                }
                else
                {
                    _list.Add(keyValuePair);
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="System.Collections.Generic.ICollection{T}"/> containing the keys of the <see cref="ListDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// An <see cref="System.Collections.Generic.ICollection{T}"/> containing the keys of the <see cref="ListDictionary{TKey, TValue}"/>.
        /// </value>
        public ICollection<TKey> Keys
        {
            get { return _list.Select(item => item.Key).ToList(); }
        }

        /// <summary>
        /// Gets an <see cref="System.Collections.Generic.ICollection{T}"/> containing the values in the <see cref="ListDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <value>
        /// An <see cref="System.Collections.Generic.ICollection{T}"/> containing the values in the <see cref="ListDictionary{TKey, TValue}"/>.
        /// </value>
       public ICollection<TValue> Values
        {
            get { return _list.Select(item => item.Value).ToList(); }
        }
        #endregion

        #region Methods
        /// <summary>
        ///  Returns an enumerator that iterates through the collection.
        /// </summary>
        ///  <returns>A <see cref="System.Collections.Generic.IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The object to add.</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// Removes all items.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ListDictionary{TKey, TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate.</param>
        /// <returns>true if item is found; otherwise, false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// Copies the elements to an <see cref="System.Array"/>, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ListDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// Adds the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            _list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Determines whether the dictionary contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _list.Any(item => Equals(item.Key, key));
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="ListDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            var index = RemoveAndGetIndex(key);
            return index >= 0;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the object contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var index = GetIndex(key);
            if (index < 0)
            {
                value = default(TValue);
                return false;
            }

            value = _list[index].Value;
            return true;
        }

        private int RemoveAndGetIndex(TKey key)
        {
            var index = GetIndex(key);

            if (index >= 0)
            {
                _list.RemoveAt(index);
            }
            return index;
        }

        private int GetIndex(TKey key)
        {
            var index = 0;
            while (index >= 0 && index < _list.Count)
            {
                var keyValuePair = _list[index];
                if (Equals(keyValuePair.Key, key))
                {
                    return index;
                }

                index++;
            }
            return -1;
        }
        #endregion
    }
}