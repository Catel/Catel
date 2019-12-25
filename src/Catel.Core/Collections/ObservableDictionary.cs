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
        INotifyPropertyChanged,
        ISuspendChangeNotificationsCollection
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IndexedKeyedCollection<TKey, TValue> _collection;

        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs(nameof(Count));
        internal static readonly PropertyChangedEventArgs KeysPropertyChanged = new PropertyChangedEventArgs(nameof(Keys));
        internal static readonly PropertyChangedEventArgs ValuesPropertyChanged = new PropertyChangedEventArgs(nameof(Values));

#if NET || NETCORE
        [field: NonSerialized]
#endif
        private readonly SerializationInfo _serializationInfo;

        #endregion

        #region Fields
#if NET || NETCORE
        [field: NonSerialized]
#endif
        private SuspensionContext<KeyValuePair<TKey, TValue>> _suspensionContext;
        private Dictionary<TKey, TValue> _dictionaryCache;
        private int _dictionaryCacheVersion;
        private int _version;
        #endregion

        #region Constructors
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

            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        protected ObservableDictionary(SerializationInfo info, StreamingContext context)
        {
            _serializationInfo = info;
        }

        #endregion


        #region Methods


        /// <inheritdoc cref="ISerializable.GetObjectData"/>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Argument.IsNotNull(nameof(info), info);

            var entries = new Collection<KeyValuePair<TKey, TValue>>();
            foreach (var item in _collection)
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
            // Check
            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Removing)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
            }

            var newItem = new KeyValuePair<TKey, TValue>(key, value);

            int index;
            if (TryGetValue(key, out var oldValue))
            {
                if (append)
                {
                    throw Log.ErrorAndCreateException<ArgumentException>("An item with the same key has already been added.");
                }

                if (Equals(oldValue, value))
                {
                    return;
                }

                // Check
                if (_suspensionContext != null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Silent && !_suspensionContext.IsMixedMode()))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Replacing items is only allowed in SuspensionMode.None, SuspensionMode.Silent or a mixed mode, current mode is '{_suspensionContext.Mode}'");
                }

                var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
                index = _collection.IndexOf(oldItem);

                if (_suspensionContext != null && _suspensionContext.IsMixedMode())
                {
                    // Split up (call 2 events)
                    Remove(key);
                    Insert(key, value, true);
                }
                else
                {
                    _collection.Remove(key);
                    _collection.Insert(index, newItem);

                    _version++;

                    RaiseInternalPropertyChanged();
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, newItem, oldItem, index);
                }
            }
            else
            {
                _collection.Add(newItem);
                _version++;

                index = _collection.IndexOf(newItem);

                RaiseInternalPropertyChanged();
                OnCollectionChanged(NotifyCollectionChangedAction.Add, newItem, index);

                if (_suspensionContext != null)
                {
                    if (_suspensionContext.Mode == SuspensionMode.Adding || _suspensionContext.IsMixedMode())
                    {
                        _suspensionContext.ChangedItems.Add(newItem);
                        _suspensionContext.ChangedItemIndices.Add(index);
                    }

                    if (_suspensionContext.IsMixedMode())
                    {
                        _suspensionContext.MixedActions.Add(NotifyCollectionChangedAction.Add);
                    }
                }
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
                var tempKey = (TKey)key;

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
        /// Raises <see cref="INotifyCollectionChanged.CollectionChanged"/> with
        /// <see cref="NotifyCollectionChangedAction.Reset"/> changed action.
        /// </summary>
        public void Reset()
        {
            if (NotificationsSuspended)
            {
                Log.Error("Cannot reset while notifications are suspended");
                return;
            }

            NotifyChanges();
        }

        /// <summary>
        /// Suspends the change notifications until the returned <see cref="IDisposable"/> is disposed.
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var fastCollection = new FastObservableCollection<int>();
        /// using (fastCollection.SuspendChangeNotificaftions())
        /// {
        ///     // Adding or removing events inside here will not raise events
        ///     fastCollection.Add(1);
        ///     fastCollection.Add(2);
        ///     fastCollection.Add(3);
        /// 
        ///     fastCollection.Remove(3);
        ///     fastCollection.Remove(2);
        ///     fastCollection.Remove(1);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>IDisposable.</returns>
        public IDisposable SuspendChangeNotifications()
        {
            return SuspendChangeNotifications(SuspensionMode.None);
        }

        /// <summary>
        /// Suspends the change notifications until the returned <see cref="IDisposable"/> is disposed.
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var fastCollection = new FastObservableCollection<int>();
        /// using (fastCollection.SuspendChangeNotificaftions())
        /// {
        ///     // Adding or removing events inside here will not raise events
        ///     fastCollection.Add(1);
        ///     fastCollection.Add(2);
        ///     fastCollection.Add(3);
        /// 
        ///     fastCollection.Remove(3);
        ///     fastCollection.Remove(2);
        ///     fastCollection.Remove(1);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="mode">The suspension Mode.</param>
        /// <returns>IDisposable.</returns>
        public IDisposable SuspendChangeNotifications(SuspensionMode mode)
        {
            if (_suspensionContext is null)
            {
                // Create new context
                _suspensionContext = new SuspensionContext<KeyValuePair<TKey, TValue>>(mode);
            }
            else if (_suspensionContext != null && _suspensionContext.Mode != mode)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot change mode during another active suspension.");
            }

            return new DisposableToken<ObservableDictionary<TKey, TValue>>(
                this,
                x =>
                {
                    x.Instance._suspensionContext.Count++;
                },
                x =>
                {
                    x.Instance._suspensionContext.Count--;
                    if (x.Instance._suspensionContext.Count == 0)
                    {
                        if (x.Instance.IsDirty)
                        {
                            x.Instance.IsDirty = false;
                            x.Instance.NotifyChanges();
                        }

                        x.Instance._suspensionContext = null;
                    }
                }, _suspensionContext);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether change to the collection is made when
        /// its notifications are suspended.
        /// </summary>
        /// <value><c>true</c> if this instance is has been changed while notifications are
        /// suspended; otherwise, <c>false</c>.</value>
        public bool IsDirty { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether change notifications are suspended.
        /// </summary>
        /// <value><c>True</c> if notifications are suspended, otherwise, <c>false</c>.</value>
        public bool NotificationsSuspended => _suspensionContext != null;


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

            foreach (var keyValuePair in entries)
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

        #endregion

        #region Events
        /// <inheritdoc cref="INotifyCollectionChanged.CollectionChanged"/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void UnsafeRaiseCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged;
        private void UnsafeRaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        protected void OnCollectionChanged()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="changedItem">The updated item.</param>
        /// <param name="index">The index value of the updated item.</param>
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="index">The index value of the old item affected.</param>
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }



        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The target property to invoke <see cref="INotifyPropertyChanged.PropertyChanged"/> on.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseInternalPropertyChanged()
        {
            OnPropertyChanged(IndexerPropertyChanged);
            OnPropertyChanged(CountPropertyChanged);
            OnPropertyChanged(KeysPropertyChanged);
            OnPropertyChanged(ValuesPropertyChanged);
        }

        /// <summary>
        /// Notifies external classes of property changes.
        /// </summary>
        protected virtual void NotifyChanges()
        {
            // Create event args list
            var eventArgsList = _suspensionContext.CreateEvents();

            // Fire events
            if (eventArgsList.Count != 0)
            {
                RaiseInternalPropertyChanged();
                foreach (var eventArgs in eventArgsList)
                {
                    OnCollectionChanged(eventArgs);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_suspensionContext is null || _suspensionContext.Count == 0)
            {
                UnsafeRaisePropertyChanged(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_suspensionContext is null || _suspensionContext.Count == 0)
            {                
                UnsafeRaiseCollectionChanged(e);
                return;
            }

            if (_suspensionContext.Count != 0)
            {
                IsDirty = true;
            }
        }


        #endregion

        #region IDictionary Members
        /// <inheritdoc cref="IDictionary.Keys"/>
        ICollection IDictionary.Keys => Dictionary.Keys;

        /// <inheritdoc cref="IDictionary.Values"/>
        ICollection IDictionary.Values => Dictionary.Values;

        /// <inheritdoc cref="IDictionary.IsReadOnly"/>
        public bool IsReadOnly => false;
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

        /// <inheritdoc cref="IDictionary.Clear"/>
        public virtual void Clear()
        {
            if (_suspensionContext != null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Silent && !_suspensionContext.IsMixedMode()))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Clearing items is only allowed in SuspensionMode.None, SuspensionMode.Silent or mixed modes, current mode is '{_suspensionContext.Mode}'");
            }

            if (_suspensionContext != null && _suspensionContext.IsMixedMode())
            {
                while (Count > 0)
                {   
                    Remove(_collection[0].Key);
                }
            }
            else
            {
                // Call base
                if (Count <= 0)
                {
                    return;
                }

                _collection.Clear();
                _version++;

                RaiseInternalPropertyChanged();
                OnCollectionChanged();
            }


            
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

        #endregion
        #region Generic IDictionary Members


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

        /// <inheritdoc cref="IDictionary{TKey, TValue}.ContainsKey"/>
        public bool ContainsKey(TKey key)
        {
            return _collection.Contains(key);
        }


        /// <inheritdoc cref="IDictionary{TKey, TValue}.Add(TKey, TValue)"/>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        /// <inheritdoc cref="IDictionary{TKey, TValue}.Remove(TKey)"/>
        public bool Remove(TKey key)
        {
            // Check
            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Adding)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Removing items is not allowed in mode SuspensionMode.Adding.");
            }

            if (!TryGetValue(key, out var value))
            {
                return false;
            }
            
            var item = new KeyValuePair<TKey, TValue>(key, value);
            var index = _collection.IndexOf(item);

            if (!_collection.Remove(key))
            {
                return false;
            }
            _version++;
            RaiseInternalPropertyChanged();
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);


            if (_suspensionContext != null)
            {
                if (_suspensionContext.Mode == SuspensionMode.Removing || _suspensionContext.IsMixedMode())
                {
                    _suspensionContext.ChangedItems.Add(item);
                    _suspensionContext.ChangedItemIndices.Add(index);
                }

                if (_suspensionContext.IsMixedMode())
                {
                    _suspensionContext.MixedActions.Add(NotifyCollectionChangedAction.Remove);
                }
            }

            return true;
        }

        /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue"/>
        public bool TryGetValue(TKey key, out TValue value)
        {
#if NETCORE
            if (!_collection.TryGetValue(key, out var item))
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

        /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Keys"/>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Dictionary.Keys;

        /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Values"/>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Dictionary.Values;
        #endregion

        #region ICollection Members
        /// <inheritdoc cref="ICollection.Count"/>
        public int Count => _collection.Count;

        /// <inheritdoc cref="ICollection.CopyTo"/>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)_collection).CopyTo(array, index);
        }
        #endregion
        #region Generic ICollection Members

        /// <inheritdoc cref="ICollection{T}.Add"/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc cref="ICollection{T}.Contains"/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _collection.Contains(item);
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

            foreach (var keyValuePair in _collection)
            {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
            }
        }


        /// <inheritdoc cref="ICollection{T}.Remove"/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        #region Generic IEnumerable Members
        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion






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
