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
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Services;

    public class FastObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
        IReadOnlyDictionary<TKey, TValue>,
        IDictionary,
        IList<KeyValuePair<TKey, TValue>>,
        ISerializable,
        IDeserializationCallback,
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        ISuspendChangeNotificationsCollection
        where TKey : notnull
    {
        #region Fields & Properties
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<IDispatcherService> _dispatcherService = new Lazy<IDispatcherService>(() =>
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var dispatcherService = dependencyResolver.ResolveRequiredType<IDispatcherService>();
            return dispatcherService;
        });

        /// <summary>
        /// The current suspension context.
        /// </summary>
        [field: NonSerialized]
        private SuspensionContext<KeyValuePair<TKey, TValue>>? _suspensionContext;

        /// <summary>
        /// maps from TKey to TValue
        /// </summary>
        private readonly Dictionary<TKey, TValue> _dict;

        /// <summary>
        /// maps from TKey to index
        /// </summary>
        private readonly Dictionary<TKey, int> _dictIndexMapping;

        /// <summary>
        /// maps from index to TKey (don't map to TValue here to avoid duplication of value was a ValueType)
        /// 
        /// this is only used to store the order in which keys were entered
        /// </summary>
        private readonly List<TKey> _list;

        [field: NonSerialized]
        private readonly SerializationInfo? _serializationInfo;

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; } = true;

        /// <see cref="Dictionary{TKey,TValue}.Comparer"/>>
        public IEqualityComparer<TKey> Comparer => _dict.Comparer;
        #endregion

        #region Constructors
        public FastObservableDictionary()
        {
            _dict = new Dictionary<TKey, TValue>();
            _dictIndexMapping = new Dictionary<TKey, int>();
            _list = new List<TKey>();
        }

        public FastObservableDictionary(int capacity)
        {
            _dict = new Dictionary<TKey, TValue>(capacity);
            _dictIndexMapping = new Dictionary<TKey, int>(capacity);
            _list = new List<TKey>(capacity);
        }

        public FastObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> originalDict)
        {
            if (originalDict is ICollection<KeyValuePair<TKey, TValue>> collection)
            {
                _dict = new Dictionary<TKey, TValue>(collection.Count);
                _dictIndexMapping = new Dictionary<TKey, int>(collection.Count);
                _list = new List<TKey>(collection.Count);
            }
            else
            {
                _dict = new Dictionary<TKey, TValue>();
                _dictIndexMapping = new Dictionary<TKey, int>();
                _list = new List<TKey>();
            }

            InsertMultipleValues(0, originalDict, false);
        }

        public FastObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _list = new List<TKey>();
            _dictIndexMapping = new Dictionary<TKey, int>(comparer);
            _dict = new Dictionary<TKey, TValue>(comparer);
        }

        public FastObservableDictionary(IDictionary<TKey, TValue> dictionary)
            : this((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary)
        {
        }

        public FastObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            _list = new List<TKey>(capacity);
            _dictIndexMapping = new Dictionary<TKey, int>(capacity, comparer);
            _dict = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public FastObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _dict = new Dictionary<TKey, TValue>(dictionary.Count, comparer);
            _dictIndexMapping = new Dictionary<TKey, int>(dictionary.Count, comparer);
            _list = new List<TKey>(dictionary.Count);
            InsertMultipleValues(dictionary, false);
        }


        private FastObservableDictionary(Dictionary<TKey, TValue> dict, Dictionary<TKey, int> dictIndexMapping, List<TKey> list)
        {
            _dict = dict;
            _list = list;
            _dictIndexMapping = dictIndexMapping;
        }
        #endregion

        #region Methods
        public FastObservableDictionary<TKey, TValue> AsReadOnly()
        {
            return new FastObservableDictionary<TKey, TValue>(_dict, _dictIndexMapping, _list) { IsReadOnly = true };
        }

        /// <summary>
        /// Loops through the dictionary with the same order as the list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> AsEnumerable()
        {
            return _list.Select(x => new KeyValuePair<TKey, TValue>(x, _dict[x]));
        }

        private void WriteReplaceSuspension(KeyValuePair<TKey, TValue> changedItem, int changedIndex)
        {
            if (_suspensionContext is not null)
            {
                if (_suspensionContext.Mode == SuspensionMode.None || _suspensionContext.IsMixedMode() || _suspensionContext.Mode == SuspensionMode.Silent)
                {
                    _suspensionContext.ChangedItems.Add(changedItem);
                    _suspensionContext.ChangedItemIndices.Add(changedIndex);
                }

                if (_suspensionContext.IsMixedMode())
                {
                    _suspensionContext.MixedActions.Add(NotifyCollectionChangedAction.Replace);
                }
            }
        }
        private void WriteAddSuspension(KeyValuePair<TKey, TValue> changedItem, int changedIndex)
        {
            if (_suspensionContext is not null)
            {
                if (_suspensionContext.Mode == SuspensionMode.Adding || _suspensionContext.IsMixedMode())
                {
                    _suspensionContext.ChangedItems.Add(changedItem);
                    _suspensionContext.ChangedItemIndices.Add(changedIndex);
                }

                if (_suspensionContext.IsMixedMode())
                {
                    _suspensionContext.MixedActions.Add(NotifyCollectionChangedAction.Add);
                }
            }
        }
        private void WriteRemoveSuspension(KeyValuePair<TKey, TValue> changedItem, int changedIndex)
        {
            if (_suspensionContext is not null)
            {
                if (_suspensionContext.Mode == SuspensionMode.Removing || _suspensionContext.IsMixedMode())
                {
                    _suspensionContext.ChangedItems.Add(changedItem);
                    _suspensionContext.ChangedItemIndices.Add(changedIndex);
                }

                if (_suspensionContext.IsMixedMode())
                {
                    _suspensionContext.MixedActions.Add(NotifyCollectionChangedAction.Remove);
                }
            }
        }

        /// <summary>
        /// Inserts a single item into the ObservableDictionary using its key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="checkKeyDuplication"></param>
        public virtual void InsertSingleValue(TKey key, TValue newValue, bool checkKeyDuplication)
        {

            var changedItem = new KeyValuePair<TKey, TValue>(key, newValue);
            int changedIndex;
            if (checkKeyDuplication && _dictIndexMapping.TryGetValue(key, out var oldIndex) && _dict.TryGetValue(key, out var oldValue))
            {
                // Check
                if (_suspensionContext is not null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Silent && !_suspensionContext.IsMixedMode()))
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>($"Replacing items is only allowed in SuspensionMode.None, SuspensionMode.Silent or a mixed mode, current mode is '{Enum<SuspensionMode>.ToString(_suspensionContext.Mode)}'");
                }

                //simply change the value
                //raise replace event
                //leave the indexes as is

                //DON'T raise count change
                _dict[key] = newValue;

                OnPropertyChanged(_cachedIndexerArgs);
                OnPropertyChanged(_cachedValuesArgs);
                changedIndex = oldIndex;

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    changedItem,
                    new KeyValuePair<TKey, TValue>(key, oldValue),
                    changedIndex));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                WriteReplaceSuspension(changedItem, changedIndex);
            }
            else
            {
                // Check
                if (_suspensionContext is not null && _suspensionContext.Mode == SuspensionMode.Removing)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
                }

                //append to the end of the list
                var newIndex = Values.Count;
                _dict[key] = newValue;
                _dictIndexMapping[key] = newIndex;
                _list.Add(key);

                OnPropertyChanged(_cachedIndexerArgs);
                OnPropertyChanged(_cachedCountArgs);
                OnPropertyChanged(_cachedKeysArgs);
                OnPropertyChanged(_cachedValuesArgs);
                changedIndex = newIndex;

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    changedItem,
                    newIndex));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                WriteAddSuspension(changedItem, changedIndex);
            }
        }

        /// <summary>
        /// Inserts a single item into the ObservableDictionary using its index and key
        /// if a key exists but with a different index, a 'move' operation will occur and the key will be moved to the new location
        /// if a key doesn't exist, an 'add' operation will occur
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="checkKeyDuplication"></param>
        public virtual void InsertSingleValue(int index, TKey key, TValue newValue, bool checkKeyDuplication)
        {
            // Check
            if (_suspensionContext is not null && _suspensionContext.Mode == SuspensionMode.Removing)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
            }

            var changedItem = new KeyValuePair<TKey, TValue>(key, newValue);
            int changedIndex = index;
            if (checkKeyDuplication && _dict.TryGetValue(key, out var oldValue) && _dictIndexMapping.TryGetValue(key, out var oldIndex))
            {
                //DON'T raise count change
                if (oldIndex == index)
                {
                    // Check
                    if (_suspensionContext is not null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Silent && !_suspensionContext.IsMixedMode()))
                    {
                        throw Log.ErrorAndCreateException<InvalidOperationException>($"Replacing items is only allowed in SuspensionMode.None, SuspensionMode.Silent or a mixed mode, current mode is '{Enum<SuspensionMode>.ToString(_suspensionContext.Mode)}'");
                    }

                    //raise replace event
                    //leave the indexes as is
                    _dict[key] = newValue;
                    OnPropertyChanged(_cachedIndexerArgs);
                    OnPropertyChanged(_cachedValuesArgs);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        changedItem,
                        new KeyValuePair<TKey, TValue>(key, oldValue),
                        index));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                    WriteReplaceSuspension(changedItem, changedIndex);
                }
                else
                {
                    InternalMoveItem(oldIndex, index, key, newValue);
                }
            }
            else
            {
                // Check
                if (_suspensionContext is not null && _suspensionContext.Mode == SuspensionMode.Removing)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
                }

                for (var i = index; i < Count; i++)
                {
                    var keyAtCurrentIndex = _list[i];
                    _dictIndexMapping[keyAtCurrentIndex] = i + 1;
                }
                _list.Insert(index, key);
                _dictIndexMapping[key] = index;

                OnPropertyChanged(_cachedIndexerArgs);
                OnPropertyChanged(_cachedCountArgs);
                OnPropertyChanged(_cachedKeysArgs);
                OnPropertyChanged(_cachedValuesArgs);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new KeyValuePair<TKey, TValue>(key, newValue),
                    index));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                WriteAddSuspension(changedItem, changedIndex);
            }
        }

        public virtual void MoveItem(int oldIndex, int newIndex)
        {
            var key = _list[oldIndex];
            if (_dict.TryGetValue(key, out var oldValue))
            {
                InternalMoveItem(oldIndex, newIndex, key, oldValue);
            }
        }
        protected virtual void InternalMoveItem(int oldIndex, int newIndex, TKey key, TValue element)
        {
            // Check
            if (_suspensionContext is not null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Silent && !_suspensionContext.IsMixedMode()))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Moving items is only allowed in SuspensionMode.None, SuspensionMode.Silent or mixed modes, current mode is '{Enum<SuspensionMode>.ToString(_suspensionContext.Mode)}'");
            }

            var temp = _list[oldIndex];
            var sign = Math.Sign(newIndex - oldIndex);
            var checkCondition = sign > 0 ? (Func<int, bool>)((int i) => i < newIndex) : ((int i) => i > newIndex);
            for (var i = oldIndex; checkCondition(i); i += sign) //negative sign
            {
                _dictIndexMapping[_list[i] = _list[i + sign]] = i;
            }

            _list[newIndex] = temp;
            _dictIndexMapping[temp] = newIndex;

            var changedItem = new KeyValuePair<TKey, TValue>(key, element);
            OnPropertyChanged(_cachedIndexerArgs);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,
                changedItem,
                oldIndex,
                newIndex));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

            if (_suspensionContext is not null && _suspensionContext.IsMixedMode())
            {
                WriteRemoveSuspension(changedItem, oldIndex);
                WriteAddSuspension(changedItem, newIndex);
            }
        }

        /// <summary>
        /// Removes a single item using its key
        /// </summary>
        /// <param name="keyToRemove"></param>
        /// <param name="value">the removed value</param>
        /// <returns></returns>
        public virtual bool TryRemoveSingleValue(TKey keyToRemove, out TValue? value)
        {
            if (_dictIndexMapping.TryGetValue(keyToRemove, out var removedKeyIndex) && _dict.TryGetValue(keyToRemove, out var removedKeyValue))
            {
                // Check
                if (_suspensionContext is not null && _suspensionContext.Mode == SuspensionMode.Adding)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Removing items is not allowed in mode SuspensionMode.Adding.");
                }

                _list.RemoveAt(removedKeyIndex);
                _dict.Remove(keyToRemove);

                for (var i = removedKeyIndex; i < _list.Count; i++)
                {
                    var curKey = _list[i];
                    _dictIndexMapping[curKey] = i;
                }

                OnPropertyChanged(_cachedIndexerArgs);
                OnPropertyChanged(_cachedCountArgs);
                OnPropertyChanged(_cachedKeysArgs);
                OnPropertyChanged(_cachedValuesArgs);

                var changedItem = new KeyValuePair<TKey, TValue>(keyToRemove, removedKeyValue);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    changedItem,
                    removedKeyIndex));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                WriteRemoveSuspension(changedItem, removedKeyIndex);

                //raise remove event 
                value = removedKeyValue;
                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        /// removes a single item using its index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfRangeException">if the index is outside boundaries</exception>
        public virtual void RemoveSingleValue(int index, out TValue value)
        {
            // Check      
            if (_suspensionContext is not null && _suspensionContext.Mode == SuspensionMode.Adding)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Removing items is not allowed in mode SuspensionMode.Adding.");
            }
            var keyToRemove = _list[index];
            value = _dict[keyToRemove];
            _dict.Remove(keyToRemove);
            _list.RemoveAt(index);
            for (var i = index; i < Count; i++)
            {
                var key = _list[i];
                _dictIndexMapping[key] = i;
            }
            OnPropertyChanged(_cachedIndexerArgs);
            OnPropertyChanged(_cachedCountArgs);
            OnPropertyChanged(_cachedKeysArgs);
            OnPropertyChanged(_cachedValuesArgs);
            var changedItem = new KeyValuePair<TKey, TValue>(keyToRemove, value);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    changedItem,
                    index));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

            WriteRemoveSuspension(changedItem, index);
        }

        /// <summary>
        /// Append or replace multiple values
        /// </summary>
        /// <param name="newValues"></param>
        /// <param name="checkKeyDuplication"></param>
        public virtual void InsertMultipleValues(IEnumerable<KeyValuePair<TKey, TValue>> newValues, bool checkKeyDuplication)
        {
            if (checkKeyDuplication)
            {
                newValues = newValues.Where(x => !_dict.ContainsKey(x.Key));
            }

            var startIndex = _list.Count;
            if (newValues is ICollection<KeyValuePair<TKey, TValue>> collection)
            {
                var counterIndex = startIndex;
                _list.AddRange(newValues.Select(x => x.Key));
                foreach (var item in collection)
                {
                    _dict[item.Key] = item.Value;
                    _dictIndexMapping[item.Key] = counterIndex++;
                }

                OnPropertyChanged(_cachedIndexerArgs);
                OnPropertyChanged(_cachedCountArgs);
                OnPropertyChanged(_cachedKeysArgs);
                OnPropertyChanged(_cachedValuesArgs);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection, startIndex));

                WriteMultipleAddSuspension(collection, startIndex, collection.Count);
            }
            else
            {
                using (SuspendChangeNotifications())
                {
                    foreach (var item in newValues)
                    {
                        InsertSingleValue(item.Key, item.Value, checkKeyDuplication);
                    }
                }
            }
        }

        private void WriteMultipleAddSuspension(IEnumerable<KeyValuePair<TKey, TValue>> collection, int startIndex, int count)
        {
            if (_suspensionContext is not null)
            {
                if (_suspensionContext.Mode == SuspensionMode.Adding || _suspensionContext.IsMixedMode())
                {
                    _suspensionContext.ChangedItems.AddRange(collection);
                    _suspensionContext.ChangedItemIndices.AddRange(Enumerable.Range(startIndex, count));
                }

                if (_suspensionContext.IsMixedMode())
                {
                    _suspensionContext.MixedActions.AddRange(Enumerable.Repeat(NotifyCollectionChangedAction.Add, count));
                }
            }
        }
        private void WriteMultipleRemoveSuspension(IEnumerable<KeyValuePair<TKey, TValue>> collection, int startIndex, int count)
        {
            if (_suspensionContext is not null)
            {
                if (_suspensionContext.Mode == SuspensionMode.Adding || _suspensionContext.IsMixedMode())
                {
                    _suspensionContext.ChangedItems.AddRange(collection);
                    _suspensionContext.ChangedItemIndices.AddRange(Enumerable.Range(startIndex, count));
                }

                if (_suspensionContext.IsMixedMode())
                {
                    _suspensionContext.MixedActions.AddRange(Enumerable.Repeat(NotifyCollectionChangedAction.Remove, count));
                }
            }
        }

        /// <summary>
        /// Inserts multiple values with a start index
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="newValues"></param>
        /// <param name="checkKeyDuplication">only set to false if you are absolutely sure there is never going to be key duplication (e.g. during construction)</param>
        public virtual void InsertMultipleValues(int startIndex, IEnumerable<KeyValuePair<TKey, TValue>> newValues, bool checkKeyDuplication)
        {
            if (checkKeyDuplication)
            {
                newValues = newValues.Where(x => !_dict.ContainsKey(x.Key));
            }

            var counterIndex = startIndex;

            if (newValues is ICollection<KeyValuePair<TKey, TValue>> collection)
            {
                var count = collection.Count;
                _list.InsertRange(startIndex, newValues.Select(x => x.Key));
                foreach (var item in collection)
                {
                    _dict[item.Key] = item.Value;
                    _dictIndexMapping[item.Key] = counterIndex++;
                }

                OnPropertyChanged(_cachedIndexerArgs);
                OnPropertyChanged(_cachedCountArgs);
                OnPropertyChanged(_cachedKeysArgs);
                OnPropertyChanged(_cachedValuesArgs);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection, startIndex));

                WriteMultipleAddSuspension(collection, startIndex, collection.Count);
            }
            else
            {
                using (SuspendChangeNotifications())
                {
                    foreach (var item in newValues)
                    {
                        InsertSingleValue(counterIndex++, item.Key, item.Value, checkKeyDuplication);
                    }
                }
            }
        }


        /// <summary>
        /// removes a range of values
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public virtual void RemoveMultipleValues(int startIndex, int count)
        {
            var lastIndex = startIndex + count - 1;
            var removed = new List<KeyValuePair<TKey, TValue>>();
            for (var i = startIndex; i <= lastIndex; i++)
            {
                var key = _list[i];
                if (_dict.TryGetValue(key, out var val))
                {
                    removed.Add(new KeyValuePair<TKey, TValue>(key, val));
                    _dict.Remove(key);
                    _dictIndexMapping.Remove(key);
                }
            }
            _list.RemoveRange(startIndex, count);

            OnPropertyChanged(_cachedIndexerArgs);
            OnPropertyChanged(_cachedCountArgs);
            OnPropertyChanged(_cachedKeysArgs);
            OnPropertyChanged(_cachedValuesArgs);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, startIndex));

            WriteMultipleRemoveSuspension(removed, startIndex, removed.Count);
        }

        /// <summary>
        /// Removes multiple keys from the dictionary
        /// </summary>
        /// <param name="keysToRemove">The keys to remove</param>
        public virtual void RemoveMultipleValues(IEnumerable<TKey> keysToRemove)
        {
            Dictionary<int, List<KeyValuePair<TKey, TValue>>> removedList;

            if (keysToRemove is ICollection<TKey> collectionOfKeysToRemove)
            {
                removedList = new Dictionary<int, List<KeyValuePair<TKey, TValue>>>(collectionOfKeysToRemove.Count);
            }
            else
            {
                removedList = new Dictionary<int, List<KeyValuePair<TKey, TValue>>>();
            }
            foreach (var keyToRemove in keysToRemove)
            {
                if (_dictIndexMapping.TryGetValue(keyToRemove, out var removedKeyIndex) && _dict.TryGetValue(keyToRemove, out var valueToRemove))
                {
                    if (removedList.TryGetValue(removedKeyIndex - 1, out var toRemoveList))
                    {
                        toRemoveList.Add(new KeyValuePair<TKey, TValue>(keyToRemove, valueToRemove));
                    }
                    else
                    {
                        removedList[removedKeyIndex] = new List<KeyValuePair<TKey, TValue>>() { new KeyValuePair<TKey, TValue>(keyToRemove, valueToRemove) };
                    }

                    _dict.Remove(keyToRemove);
                    _dictIndexMapping.Remove(keyToRemove);
                }
            }

            OnPropertyChanged(_cachedIndexerArgs);
            OnPropertyChanged(_cachedCountArgs);
            OnPropertyChanged(_cachedKeysArgs);
            OnPropertyChanged(_cachedValuesArgs);

            foreach (var range in removedList)
            {
                _list.RemoveRange(range.Key, range.Value.Count);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, range.Value, range.Key));
                WriteMultipleRemoveSuspension(range.Value, range.Key, range.Value.Count);
            }
        }


        public virtual void RemoveAllItems()
        {
            // Check
            if (_suspensionContext is not null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Silent && !_suspensionContext.IsMixedMode()))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Clearing items is only allowed in SuspensionMode.None, SuspensionMode.Silent or mixed modes, current mode is '{Enum<SuspensionMode>.ToString(_suspensionContext.Mode)}'");
            }

            List<KeyValuePair<TKey, TValue>>? copyList = null;

            if (_suspensionContext is not null && _suspensionContext.IsMixedMode())
            {
                copyList = AsEnumerable().ToList();
            }

            _list.Clear();
            _dict.Clear();

            OnPropertyChanged(_cachedIndexerArgs);
            OnPropertyChanged(_cachedCountArgs);
            OnPropertyChanged(_cachedKeysArgs);
            OnPropertyChanged(_cachedValuesArgs);

            OnCollectionChanged(_cachedResetArgs);

            if (copyList is not null)
            {
                WriteMultipleRemoveSuspension(copyList, 0, copyList.Count);
            }
        }
        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>>
        public int Count => _list.Count;
        public bool IsReadOnly { get; private set; }

        public void Add(KeyValuePair<TKey, TValue> item)
        {

            InsertSingleValue(item.Key, item.Value, true);
        }

        public void Clear()
        {
            RemoveAllItems();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count)
            {
                throw new IndexOutOfRangeException("Array doesn't have enough space to copy all the elements");
            }

            for (var i = 0; i < Count; i++)
            {
                var key = _list[i];
                var value = _dict[key];
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return TryRemoveSingleValue(item.Key, out _);
        }
        #endregion

        #region IDictionary<TKey,TValue>
        public TValue this[TKey key]
        {
            get => _dict[key];
            set => InsertSingleValue(key, value, true);
        }

        public ICollection<TKey> Keys => _dict.Keys;
        public ICollection<TValue> Values => _dict.Values;

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            InsertSingleValue(key, value, true);
        }

        public bool Remove(TKey keyToRemove)
        {
            return TryRemoveSingleValue(keyToRemove, out _);
        }

        /// <summary>
        /// Attempts to get the value from a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            value = default;
#pragma warning restore CS8601 // Possible null reference assignment.

            if (_dict.TryGetValue(key, out var typedValue))
            {
                value = typedValue;
                return true;
            }

            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return AsEnumerable().GetEnumerator();
        }
        #endregion

        #region IReadOnlyDictionary<TKey,TValue>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dict.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dict.Values;
        #endregion

        #region IDictionary
        ICollection IDictionary.Keys => _dict.Keys;
        ICollection IDictionary.Values => _dict.Values;

        public bool IsFixedSize => ((IDictionary)_dict).IsFixedSize;
        public object SyncRoot => ((IDictionary)_dict).SyncRoot;
        public bool IsSynchronized => ((IDictionary)_dict).IsSynchronized;


        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>
        /// The value associated with the specified key, or <see langword="null" /> if 
        /// <paramref name="key"/> is not in the dictionary or <paramref name="key"/> is of a type that 
        /// is not assignable to the key of type <typeparamref name="TKey"/> of the 
        /// <see cref="FastObservableDictionary{TKey,TValue}"/>.
        /// </returns>
        public object? this[object key]
        {
            get
            {
                if (key is TKey castedKey)
                {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                    return this[castedKey];
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                }
                else
                {
                    return null;
                }
            }
            set
            {
                TValue? typedValue = default;

                if (value is TValue correctTypedValue)
                {
                    typedValue = correctTypedValue;
                }

#pragma warning disable CS8601 // Possible null reference assignment.
                this[(TKey)key] = typedValue;
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }

        /// <summary>
        /// checks if the 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(object key)
        {
            if (key is TKey castedKey)
            {
                return ContainsKey(castedKey);
            }

            return false;
        }

        public void Add(object key, object? value)
        {
            if (key is TKey castedKey)
            {
                if (value is TValue castedValue)
                {
                    InsertSingleValue(castedKey, castedValue, true);
                }
                else
                {
                    throw Log.ErrorAndCreateException<InvalidCastException>($"Value must be of type {typeof(TValue)}");
                }
            }
            else
            {
                throw Log.ErrorAndCreateException<InvalidCastException>($"Key must be of type {typeof(TKey)}");
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
#pragma warning disable IDISP005 // Return type should indicate that the value should be disposed.
            return _dict.GetEnumerator();
#pragma warning restore IDISP005 // Return type should indicate that the value should be disposed.
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }

        void IDictionary.Remove(object key)
        {
            if (key is TKey castedKey)
            {
                TryRemoveSingleValue(castedKey, out _);
            }
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count) throw new IndexOutOfRangeException("Array doesn't have enough space to copy all the elements");
            for (var i = 0; i < Count; i++)
            {
                var key = _list[i];
                var value = _dict[key];
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                array.SetValue(new KeyValuePair<TKey, TValue>(key, value), arrayIndex + i);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            }
        }
        #endregion

        #region IList<KeyValuePair<TKey,TValue>>
        KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
        {
            get
            {
                var key = _list[index];
                return new KeyValuePair<TKey, TValue>(key, _dict[key]);
            }
            set => InsertSingleValue(index, value.Key, value.Value, true);
        }

        public int IndexOf(KeyValuePair<TKey, TValue> item)
        {
            return _dictIndexMapping[item.Key];
        }

        public void Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            InsertSingleValue(index, item.Key, item.Value, true);
        }

        public void RemoveAt(int index)
        {
            RemoveSingleValue(index, out _);
        }
        #endregion

        #region ISerializable and IDeserializationCallback
        private const string EntriesName = "entries";
        protected FastObservableDictionary(SerializationInfo info, StreamingContext context)
            : this()
        {
            _serializationInfo = info;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var entries = new Collection<KeyValuePair<TKey, TValue>>();
            foreach (var item in AsEnumerable())
            {
                entries.Add(item);
            }

            info.AddValue(EntriesName, entries);
        }

        public void OnDeserialization(object? sender)
        {
            if (_serializationInfo is null)
            {
                return;
            }

            var entries = (Collection<KeyValuePair<TKey, TValue>>?)_serializationInfo.GetValue(EntriesName, typeof(Collection<KeyValuePair<TKey, TValue>>));
            if (entries is not null)
            {
                foreach (var keyValuePair in entries)
                {
                    Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return AsEnumerable().GetEnumerator();
        }
        #endregion

        #region INotifyPropertyChanged and INotifyCollectionChanged
        protected readonly PropertyChangedEventArgs _cachedIndexerArgs = new PropertyChangedEventArgs("Item[]");
        protected readonly PropertyChangedEventArgs _cachedCountArgs = new PropertyChangedEventArgs(nameof(Count));
        protected readonly PropertyChangedEventArgs _cachedKeysArgs = new PropertyChangedEventArgs(nameof(Keys));
        protected readonly PropertyChangedEventArgs _cachedValuesArgs = new PropertyChangedEventArgs(nameof(Values));

        protected readonly NotifyCollectionChangedEventArgs _cachedResetArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);


        protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            if (_suspensionContext is null || _suspensionContext.Count == 0)
            {
                if (AutomaticallyDispatchChangeNotifications)
                {
                    _dispatcherService.Value.BeginInvokeIfRequired(() => PropertyChanged?.Invoke(this, eventArgs));
                }
                else
                {
                    PropertyChanged?.Invoke(this, eventArgs);
                }
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {

            if (_suspensionContext is null || _suspensionContext.Count == 0)
            {
                if (AutomaticallyDispatchChangeNotifications)
                {
                    _dispatcherService.Value.BeginInvokeIfRequired(() => CollectionChanged?.Invoke(this, eventArgs));
                }
                else
                {
                    CollectionChanged?.Invoke(this, eventArgs);
                }

                return;
            }

            if (_suspensionContext.Count != 0)
            {
                IsDirty = true;
            }
        }

        /// <inheritdoc cref="INotifyCollectionChanged.CollectionChanged"/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region ISuspendChangeNotificationsCollection
        public void Reset()
        {
            if (NotificationsSuspended)
            {
                Log.Error("Cannot reset while notifications are suspended");
                return;
            }

            NotifyChanges();
        }

        public IDisposable SuspendChangeNotifications()
        {
            return SuspendChangeNotifications(SuspensionMode.None);
        }

        public IDisposable SuspendChangeNotifications(SuspensionMode mode)
        {
            if (_suspensionContext is null)
            {
                // Create new context
                _suspensionContext = new SuspensionContext<KeyValuePair<TKey, TValue>>(mode);
            }
            else if (_suspensionContext is not null && _suspensionContext.Mode != mode)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot change mode during another active suspension.");
            }

            return new DisposableToken<FastObservableDictionary<TKey, TValue>>(
                this,
                x =>
                {
                    x.Instance._suspensionContext!.Count++;
                },
                x =>
                {
                    x.Instance._suspensionContext!.Count--;
                    if (x.Instance._suspensionContext.Count == 0)
                    {
                        if (x.Instance.IsDirty)
                        {
                            x.Instance.IsDirty = false;
                            x.Instance.NotifyChanges();
                        }

                        x.Instance._suspensionContext = null;
                    }
                }, _suspensionContext!);
        }

        /// <summary>
        /// Notifies external classes of property changes.
        /// </summary>
        protected void NotifyChanges()
        {
            var action = () =>
            {
                // Create event args list
                var eventArgsList = _suspensionContext!.CreateEvents();

                // Fire events
                if (eventArgsList.Count != 0)
                {
                    OnPropertyChanged(_cachedCountArgs);
                    OnPropertyChanged(_cachedIndexerArgs);
                    OnPropertyChanged(_cachedKeysArgs);
                    OnPropertyChanged(_cachedValuesArgs);
                    foreach (var eventArgs in eventArgsList)
                    {
                        OnCollectionChanged(eventArgs);
                    }
                }
            };

            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.Value.BeginInvokeIfRequired(action);
            }
            else
            {
                action();
            }
        }
        public bool IsDirty { get; private set; }

        public bool NotificationsSuspended { get; private set; }
        #endregion
    }
}
