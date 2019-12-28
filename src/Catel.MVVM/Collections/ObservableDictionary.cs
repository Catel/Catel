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



    /// <summary>
    /// Represents an observable collection of keys and values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
#if NET || NETCORE
    [Serializable]
#endif
    public class ObservableDictionary<TKey, TValue> : FastObservableDictionary<TKey, TValue>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public ObservableDictionary()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that is empty, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="IEqualityComparer{T}"/> for the type of key.</param>
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}"/> and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> whose elements are copied to the new <see cref="ObservableDictionary{TKey,TValue}"/>.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableDictionary{TKey,TValue}"/> class that contains elements copied from the specified <see cref="IDictionary{TKey,TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey,TValue}"/> whose elements are copied to the new <see cref="ObservableDictionary{TKey,TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="IEqualityComparer{T}"/> for the type of key.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {
        }

        protected ObservableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// <remarks>Set <see cref="FastObservableDictionary{TKey,TValue}.AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// </summary>
        protected virtual void OnCollectionChanged()
        {
            base.OnCollectionChanged(_cachedResetArgs);
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="FastObservableDictionary{TKey,TValue}.AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="changedItem">The updated item.</param>
        /// <param name="index">The index value of the updated item.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem, int index)
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="FastObservableDictionary{TKey,TValue}.AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="action">The <see cref="NotifyCollectionChangedAction"/> operation that was executed.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="index">The index value of the old item affected.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, int index)
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event while also ensuring that it is dispatched to the UI thread.
        /// </summary>
        /// <remarks>Set <see cref="FastObservableDictionary{TKey,TValue}.AutomaticallyDispatchChangeNotifications"/> to <c>false</c> if you do not wish to dispatch to the UI.</remarks>
        /// <param name="propertyName">The target property to invoke <see cref="INotifyPropertyChanged.PropertyChanged"/> on.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

#if NET || NETCORE
        [Serializable]
        [ObsoleteEx(RemoveInVersion = "6.0", TreatAsErrorFromVersion = "5.12.1", Message = "Use the normal enumerator")]
#endif
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    return new KeyValuePair<TKey, TValue>();
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry();
                }
            }

            public object Key
            {
                get
                {
                    return null;
                }
            }

            public object Value
            {
                get
                {
                    return null;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }
        }
    }
}
