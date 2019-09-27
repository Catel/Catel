// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableDictionary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using IoC;
    using Logging;
    using Services;

    internal class IndexedKeyedCollection<TKey, TValue> : KeyedCollection<TKey, KeyValuePair<TKey, TValue>>
    {
        public IndexedKeyedCollection()
        {
        }

        public IndexedKeyedCollection(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        protected override TKey GetKeyForItem(KeyValuePair<TKey, TValue> item)
        {
            return item.Key;
        }
    }

    /// <summary>
    /// Represents an observable collection of keys and values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
#if NET || NETCORE
    [Serializable]
#endif
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
        IDictionary,
        IReadOnlyDictionary<TKey, TValue>,
        ISerializable,
        IDeserializationCallback,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IndexedKeyedCollection<TKey, TValue> _collection;

        private readonly Lazy<IDispatcherService> _dispatcherService = new Lazy<IDispatcherService>(() => IoCConfiguration.DefaultDependencyResolver.Resolve<IDispatcherService>());

#if NET || NETCORE
        [field: NonSerialized]
#endif
        private readonly SerializationInfo _serializationInfo;

        private Dictionary<TKey, TValue> _dictionaryCache;

        private int _dictionaryCacheVersion;

        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public ObservableDictionary()
            : this(new Dictionary<TKey, TValue>(), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that is empty, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="IEqualityComparer{T}"/> for the type of key.</param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
            : this(new Dictionary<TKey, TValue>(), comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}"/> and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> whose elements are copied to the new <see cref="ObservableDictionary{TKey,TValue}"/>.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> whose elements are copied to the new <see cref="ObservableDictionary{TKey,TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="IEqualityComparer{T}"/> for the type of key.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            Argument.IsNotNull(nameof(dictionary), dictionary);

            Comparer = comparer ?? EqualityComparer<TKey>.Default;
            _collection = new IndexedKeyedCollection<TKey, TValue>(Comparer);

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        protected ObservableDictionary(SerializationInfo info, StreamingContext context)
        {
            _serializationInfo = info;
        }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; } = true;

        /// <see cref="Dictionary{TKey,TValue}.Comparer"/>>
        public IEqualityComparer<TKey> Comparer { get; }

        /// <inheritdoc cref="Dictionary{TKey, TValue}.KeyCollection"/>
        public Dictionary<TKey, TValue>.KeyCollection Keys => Dictionary.Keys;

        /// <inheritdoc cref="Dictionary{TKey, TValue}.ValueCollection"/>
        public Dictionary<TKey, TValue>.ValueCollection Values => Dictionary.Values;

        private Dictionary<TKey, TValue> Dictionary
        {
            get
            {
                if (_dictionaryCache != null && _dictionaryCacheVersion == _version)
                {
                    return _dictionaryCache;
                }

                _dictionaryCache = _collection.ToDictionary(d => d.Key, d => d.Value);
                _dictionaryCacheVersion = _version;

                return _dictionaryCache;
            }
        }

        /// <inheritdoc cref="IDeserializationCallback.OnDeserialization"/>
        public void OnDeserialization(object sender)
        {
            if (_serializationInfo is null)
            {
                return;
            }

            Collection<KeyValuePair<TKey, TValue>> entries =
                (Collection<KeyValuePair<TKey, TValue>>)_serializationInfo.GetValue(nameof(entries), typeof(Collection<KeyValuePair<TKey, TValue>>));

            foreach (KeyValuePair<TKey, TValue> keyValuePair in entries)
            {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <inheritdoc cref="IDictionary.IsFixedSize"/>
        public bool IsFixedSize => false;

        /// <inheritdoc cref="ICollection.IsSynchronized"/>
        public bool IsSynchronized => ((ICollection)_collection).IsSynchronized;

        /// <inheritdoc cref="ICollection.SyncRoot"/>
        public object SyncRoot => ((ICollection)_collection).SyncRoot;

        /// <inheritdoc cref="IDictionary.Keys"/>
        ICollection IDictionary.Keys => Dictionary.Keys;

        /// <inheritdoc cref="IDictionary.Values"/>
        ICollection IDictionary.Values => Dictionary.Values;

        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key, or <see langword="null" /> if <paramref name="key"/> is not in the dictionary or <paramref name="key"/> is of a type that is not assignable to the key of type <typeparamref name="TKey"/> of the <see cref="ObservableDictionary{TKey,TValue}"/>.</returns>
        public object this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    return _collection[(TKey)key].Value;
                }

                return null;
            }
            set => InsertObject(key, value, false);
        }

        /// <inheritdoc cref="IDictionary.Add"/>
        public void Add(object key, object value)
        {
            InsertObject(key, value, false);
        }

        /// <inheritdoc cref="IDictionary.Contains"/>
        public bool Contains(object key)
        {
            if (IsCompatibleKey(key))
            {
                return _collection.Contains((TKey)key);
            }

            return false;
        }

        /// <inheritdoc cref="ICollection.CopyTo"/>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)_collection).CopyTo(array, index);
        }

        /// <inheritdoc cref="IDictionary.GetEnumerator"/>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <inheritdoc cref="IDictionary.Remove"/>
        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        /// <inheritdoc cref="ICollection.Count"/>
        public int Count => _collection.Count;

        /// <inheritdoc cref="IDictionary.IsReadOnly"/>
        public bool IsReadOnly => false;

        /// <inheritdoc cref="IDictionary{TKey, TValue}.Keys"/>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Dictionary.Keys;

        /// <inheritdoc cref="IDictionary{TKey, TValue}.Values"/>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Dictionary.Values;

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException" />, and a set operation creates a new element with the specified key.</returns>
        public TValue this[TKey key]
        {
            get => _collection[key].Value;
            set => Insert(key, value, false);
        }

        /// <inheritdoc cref="IDictionary{TKey, TValue}.Add(TKey, TValue)"/>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        /// <inheritdoc cref="ICollection{T}.Add"/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc cref="IDictionary.Clear"/>
        public void Clear()
        {
            if (Count <= 0)
            {
                return;
            }

            _collection.Clear();
            _version++;

            OnPropertyChanged();
            OnCollectionChanged();
        }

        /// <inheritdoc cref="ICollection{T}.Contains"/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _collection.Contains(item);
        }

        /// <inheritdoc cref="IDictionary{TKey, TValue}.ContainsKey"/>
        public bool ContainsKey(TKey key)
        {
            return _collection.Contains(key);
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo"/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Argument.IsNotNull(nameof(array), array);
            Argument.IsNotOutOfRange(nameof(arrayIndex), arrayIndex, 0, array.Length);

            if (array.Rank != 1)
            {
                throw Log.ErrorAndCreateException<ArgumentException>("Multidimensional arrays are not supported.");
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw Log.ErrorAndCreateException<ArgumentException>("Array is non-zero lower bound.");
            }

            if (array.Length - arrayIndex < _collection.Count)
            {
                throw Log.ErrorAndCreateException<ArgumentException>("Supplied array was too small.");
            }

            foreach (KeyValuePair<TKey, TValue> keyValuePair in _collection)
            {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <inheritdoc cref="IDictionary{TKey, TValue}.Remove(TKey)"/>
        public bool Remove(TKey key)
        {
            if (!TryGetValue(key, out TValue value))
            {
                return false;
            }

            KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
            int index = _collection.IndexOf(item);

            if (!_collection.Remove(key))
            {
                return false;
            }

            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);

            _version++;

            return true;
        }

        /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue"/>
        public bool TryGetValue(TKey key, out TValue value)
        {
#if NETCORE
            if (!_collection.TryGetValue(key, out KeyValuePair<TKey, TValue> item))
            {
                value = default;

                return false;
            }

            value = item.Value;

            return true;
#else
            var result = _collection.Contains(key);

            value = result ? _collection[key].Value : default;

            return result;
#endif
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="ICollection{T}.Remove"/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <inheritdoc cref="INotifyCollectionChanged.CollectionChanged"/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Keys"/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Dictionary.Keys;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Values"/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Dictionary.Values;

        /// <inheritdoc cref="ISerializable.GetObjectData"/>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Argument.IsNotNull(nameof(info), info);

            Collection<KeyValuePair<TKey, TValue>> entries = new Collection<KeyValuePair<TKey, TValue>>();
            foreach (KeyValuePair<TKey, TValue> item in _collection)
            {
                entries.Add(item);
            }

            info.AddValue(nameof(entries), entries);
        }

        private static bool IsCompatibleKey(object key)
        {
            return key is TKey;
        }

        private void Insert(TKey key, TValue value, bool append)
        {
            KeyValuePair<TKey, TValue> newItem = new KeyValuePair<TKey, TValue>(key, value);

            if (TryGetValue(key, out TValue oldValue))
            {
                if (append)
                {
                    throw Log.ErrorAndCreateException<ArgumentException>("An item with the same key has already been added.");
                }

                if (Equals(oldValue, value))
                {
                    return;
                }

                KeyValuePair<TKey, TValue> oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
                int index = _collection.IndexOf(oldItem);

                _collection.Remove(key);
                _collection.Insert(index, newItem);

                _version++;

                OnCollectionChanged(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
            }
            else
            {
                _collection.Add(new KeyValuePair<TKey, TValue>(key, value));
                _version++;

                int index = _collection.IndexOf(newItem);

                OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem, index);
            }
        }

        private void InsertObject(object key, object value, bool append)
        {
            if (value is null && default(TValue) != null)
            {
                throw Log.ErrorAndCreateException<ArgumentNullException>("Argument '{0}' cannot be null.", nameof(value));
            }

            try
            {
                TKey tempKey = (TKey)key;

                try
                {
                    Insert(tempKey, (TValue)value, append);
                }
                catch (InvalidCastException ex)
                {
                    Log.Error(ex);

                    throw;
                }
            }
            catch (InvalidCastException ex)
            {
                Log.Error(ex);

                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// </summary>
        protected virtual void OnCollectionChanged()
        {
            OnPropertyChanged();

            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
            }
            else
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="changedItem">The updated item.</param>
        /// <param name="index">The index value of the updated item.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem, int index)
        {
            OnPropertyChanged();

            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem, index)));
            }
            else
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem, index));
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="index">The index value of the old item affected.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, int index)
        {
            OnPropertyChanged();

            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index)));
            }
            else
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
            }
        }

        private void OnPropertyChanged()
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged("Item[]");
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="propertyName">The target property to invoke <see cref="INotifyPropertyChanged.PropertyChanged"/> on.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

#if NET || NETCORE
        [Serializable]
#endif
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly ObservableDictionary<TKey, TValue> _dictionary;

            private readonly int _version;

            private KeyValuePair<TKey, TValue> _current;

            private int _index;

            internal Enumerator(ObservableDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = -1;
                _current = new KeyValuePair<TKey, TValue>();
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    CheckEnumerator();

                    return _current;
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    CheckEnumerator();

                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }

            public object Key
            {
                get
                {
                    CheckEnumerator();

                    return _current.Key;
                }
            }

            public object Value
            {
                get
                {
                    CheckEnumerator();

                    return _current.Value;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                // No Managed Resources
            }

            public bool MoveNext()
            {
                CheckVersion();

                _index++;

                if (_index < _dictionary._collection.Count)
                {
                    _current = new KeyValuePair<TKey, TValue>(_dictionary._collection[_index].Key, _dictionary._collection[_index].Value);

                    return true;
                }

                _index = -2;
                _current = new KeyValuePair<TKey, TValue>();

                return false;
            }

            public void Reset()
            {
                CheckVersion();

                _index = -1;
                _current = new KeyValuePair<TKey, TValue>();
            }

            private void CheckEnumerator()
            {
                switch (_index)
                {
                    case -1:
                        throw new InvalidOperationException("The enumerator has not been started.");
                    case -2:
                        throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                }
            }

            private void CheckVersion()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
            }
        }
    }
}
