// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListDictionary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Caching
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ListDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Fields
        private readonly IList<KeyValuePair<TKey, TValue>> _list;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public ListDictionary()
        {
            _list = new List<KeyValuePair<TKey, TValue>>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return _list.Select(item => item.Key).ToList(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return _list.Select(item => item.Value).ToList(); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _list.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _list.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _list.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            _list.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _list.Any(item => Equals(item.Key, key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            var index = RemoveAndGetIndex(key);
            return index > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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

            if (index > 0)
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