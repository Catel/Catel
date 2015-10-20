// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationWrapper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Catel.Logging;
    using Reflection;
    using IWeakEventListener = Catel.IWeakEventListener;

    /// <summary>
    /// Available event change types.
    /// </summary>
    public enum EventChangeType
    {
        /// <summary>
        /// Property change.
        /// </summary>
        Property,

        /// <summary>
        /// Collection change.
        /// </summary>
        Collection
    }

    /// <summary>
    /// Wrapper for an object that implements the <see cref="INotifyPropertyChanged"/> and <see cref="INotifyCollectionChanged"/>.
    /// <para />
    /// This class is thread-safe and uses weak events to prevent memory leaks.
    /// </summary>
    public class ChangeNotificationWrapper
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly object _lockObject = new object();

        private readonly WeakReference _weakReference;
        private List<IWeakEventListener> _weakCollectionChangedListeners;
        private ConditionalWeakTable<object, IWeakEventListener> _weakCollectionChangedListenersTable;
        private List<IWeakEventListener> _weakPropertyChangedListeners;
        private ConditionalWeakTable<object, IWeakEventListener> _weakPropertyChangedListenersTable;

        private ConditionalWeakTable<object, List<WeakReference>> _collectionItems;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeNotificationWrapper"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c> or whitespace.</exception>
        public ChangeNotificationWrapper(object value)
        {
            Argument.IsNotNull("value", value);

            _weakReference = new WeakReference(value);

            // Note that we either support collections OR property changed, not both because ObservableCollection implements
            // the PropertyChanged as protected.
            var valueAsNotifyCollectionChanged = value as INotifyCollectionChanged;
            if (valueAsNotifyCollectionChanged != null)
            {
                SupportsNotifyCollectionChanged = true;
            }

            var valueAsNotifyPropertyChanged = value as INotifyPropertyChanged;
            if (valueAsNotifyPropertyChanged != null)
            {
                SupportsNotifyPropertyChanged = true;
            }

            SubscribeNotifyChangedEvents(value, null);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether <see cref="INotifyPropertyChanged"/> is supported by the target object.
        /// </summary>
        /// <value><c>true</c> if <see cref="INotifyPropertyChanged"/> is supported by the target object; otherwise, <c>false</c>.</value>
        public bool SupportsNotifyPropertyChanged { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="INotifyCollectionChanged"/> is supported by the target object.
        /// </summary>
        /// <value><c>true</c> if <see cref="INotifyCollectionChanged"/> is supported by the target object; otherwise, <c>false</c>.</value>
        public bool SupportsNotifyCollectionChanged { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the object is alive.
        /// </summary>
        /// <value><c>true</c> if the object is alive; otherwise, <c>false</c>.</value>
        public bool IsObjectAlive
        {
            get { return _weakReference.IsAlive; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether creating a <see cref="ChangeNotificationWrapper"/> is useful for the specified object.
        /// <para />
        /// An object is considered usable when it implements either <see cref="INotifyPropertyChanged"/> or <see cref="INotifyCollectionChanged"/>.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns><c>true</c> if it is useful to create a <see cref="ChangeNotificationWrapper"/>; otherwise, <c>false</c>.</returns>
        public static bool IsUsefulForObject(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is INotifyPropertyChanged)
            {
                return true;
            }

            if (obj is INotifyCollectionChanged)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when the target object raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public to allow the usage of the <see cref="WeakEventListener"/>, do not call this method yourself.
        /// </remarks>
        public void OnObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged.SafeInvoke(sender, e);
        }

        /// <summary>
        /// Called when the target object raises the <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public to allow the usage of the <see cref="WeakEventListener"/>, do not call this method yourself.
        /// </remarks>
        public void OnObjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as ICollection;

            lock (_lockObject)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        UnsubscribeNotifyChangedEvents(item, collection);
                    }
                }

                // Reset requires our own logic
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (collection != null)
                    {
                        UpdateCollectionSubscriptions(collection);
                    }
                    else
                    {
                        Log.Warning("Received NotifyCollectionChangedAction.Reset for '{0}', but the type does implement ICollection", sender.GetType().GetSafeFullName());
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        SubscribeNotifyChangedEvents(item, collection);
                    }
                }
            }

            CollectionChanged.SafeInvoke(sender, e);
        }

        /// <summary>
        /// Called when the target object raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event of an object
        /// that is located inside the collection being monitored.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public to allow the usage of the <see cref="WeakEventListener"/>, do not call this method yourself.
        /// </remarks>
        public void OnObjectCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CollectionItemPropertyChanged.SafeInvoke(sender, e);
        }

        /// <summary>
        /// Unsubscribes from all events.
        /// </summary>
        public void UnsubscribeFromAllEvents()
        {
            lock (_lockObject)
            {
                if (_weakPropertyChangedListeners != null)
                {
                    foreach (var weakListener in _weakPropertyChangedListeners)
                    {
                        weakListener.Detach();
                    }

                    _weakPropertyChangedListeners.Clear();
                }

                if (_weakCollectionChangedListeners != null)
                {
                    foreach (var weakListener in _weakCollectionChangedListeners)
                    {
                        weakListener.Detach();
                    }

                    _weakCollectionChangedListeners.Clear();
                }
            }
        }

        /// <summary>
        /// Updates all the collection subscriptions.
        /// <para />
        /// This method is internally used when a notifiable collection raises the <see cref="NotifyCollectionChangedAction.Reset"/> event.
        /// </summary>
        public void UpdateCollectionSubscriptions(ICollection collection)
        {
            if (collection == null)
            {
                return;
            }

            lock (_lockObject)
            {
                List<WeakReference> collectionItems;
                if (_collectionItems.TryGetValue(collection, out collectionItems))
                {
                    var oldItems = collectionItems.ToArray();
                    foreach (var item in oldItems)
                    {
                        if (item.IsAlive)
                        {
                            var actualItem = item.Target;
                            UnsubscribeNotifyChangedEvents(actualItem, collection);
                        }
                    }

                    collectionItems.Clear();

                    var newItems = collection.Cast<object>().ToArray();
                    foreach (var item in newItems)
                    {
                        SubscribeNotifyChangedEvents(item, collection);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes from the notify changed events.
        /// </summary>
        /// <param name="value">The object to unsubscribe from.</param>
        /// <param name="parentCollection">The parent collection.</param>
        /// <remarks>No need to check for weak events, they are unsubscribed automatically.</remarks>
        public void UnsubscribeNotifyChangedEvents(object value, ICollection parentCollection)
        {
            if (value == null)
            {
                return;
            }

            lock (_lockObject)
            {
                var propertyChangedValue = value as INotifyPropertyChanged;
                if (propertyChangedValue != null)
                {
                    UnsubscribeNotifyChangedEvent(propertyChangedValue, EventChangeType.Property, parentCollection);
                }

                var collectionChangedValue = value as INotifyCollectionChanged;
                if (collectionChangedValue != null)
                {
                    UnsubscribeNotifyChangedEvent(collectionChangedValue, EventChangeType.Collection, parentCollection);

                    foreach (var child in (IEnumerable)value)
                    {
                        UnsubscribeNotifyChangedEvents(child, parentCollection);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to the notify changed events.
        /// </summary>
        /// <param name="value">The object to subscribe to.</param>
        /// <param name="parentCollection">If not <c>null</c>, this is a collection item which should use <see cref="OnObjectCollectionItemPropertyChanged"/>.</param>
        public void SubscribeNotifyChangedEvents(object value, ICollection parentCollection)
        {
            if (value == null)
            {
                return;
            }

            lock (_lockObject)
            {
                var collectionChangedValue = value as INotifyCollectionChanged;
                if (collectionChangedValue != null)
                {
                    SubscribeNotifyChangedEvent(collectionChangedValue, EventChangeType.Collection, parentCollection);

                    var collection = value as ICollection;
                    if (collection != null)
                    {
                        foreach (var child in collection)
                        {
                            SubscribeNotifyChangedEvents(child, collection);
                        }
                    }
                }

                var propertyChangedValue = value as INotifyPropertyChanged;
                if (propertyChangedValue != null)
                {
                    // ObservableObject implements PropertyChanged as protected, make sure we accept that in non-.NET languages such
                    // as Silverlight, Windows Phone and WinRT
                    try
                    {
                        SubscribeNotifyChangedEvent(propertyChangedValue, EventChangeType.Property, parentCollection);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to subscribe to PropertyChanged event, the event is probably not public");
                    }
                }
            }
        }

        private void SubscribeNotifyChangedEvent(object value, EventChangeType eventChangeType, ICollection parentCollection)
        {
            if (value == null)
            {
                return;
            }

            lock (_lockObject)
            {
                ConditionalWeakTable<object, IWeakEventListener> eventsTable;
                List<IWeakEventListener> eventsList;

                switch (eventChangeType)
                {
                    case EventChangeType.Property:
                        if (_weakPropertyChangedListenersTable == null)
                        {
                            _weakPropertyChangedListenersTable = new ConditionalWeakTable<object, IWeakEventListener>();
                        }

                        if (_weakPropertyChangedListeners == null)
                        {
                            _weakPropertyChangedListeners = new List<IWeakEventListener>();
                        }

                        eventsTable = _weakPropertyChangedListenersTable;
                        eventsList = _weakPropertyChangedListeners;
                        break;

                    case EventChangeType.Collection:
                        if (_weakCollectionChangedListenersTable == null)
                        {
                            _weakCollectionChangedListenersTable = new ConditionalWeakTable<object, IWeakEventListener>();
                        }

                        if (_weakCollectionChangedListeners == null)
                        {
                            _weakCollectionChangedListeners = new List<IWeakEventListener>();
                        }

                        if (_collectionItems == null)
                        {
                            _collectionItems = new ConditionalWeakTable<object, List<WeakReference>>();
                        }

                        eventsTable = _weakCollectionChangedListenersTable;
                        eventsList = _weakCollectionChangedListeners;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("eventChangeType");
                }

                IWeakEventListener oldSubscription;
                if (eventsTable != null && eventsTable.TryGetValue(value, out oldSubscription))
                {
                    oldSubscription.Detach();

                    eventsList.Remove(oldSubscription);
                    eventsTable.Remove(value);
                }

                IWeakEventListener weakListener;
                switch (eventChangeType)
                {
                    case EventChangeType.Property:
                        if (parentCollection != null)
                        {
                            weakListener = this.SubscribeToWeakPropertyChangedEvent(value, OnObjectCollectionItemPropertyChanged, false);
                            if (weakListener == null)
                            {
                                Log.Debug("Failed to use weak events to subscribe to 'value.PropertyChanged', going to subscribe without weak events");

                                ((INotifyPropertyChanged) value).PropertyChanged += OnObjectCollectionItemPropertyChanged;
                            }

                            var collectionItems = _collectionItems.GetOrCreateValue(parentCollection);
                            collectionItems.Add(weakListener != null ? weakListener.SourceWeakReference : new WeakReference(value));
                        }
                        else
                        {
                            weakListener = this.SubscribeToWeakPropertyChangedEvent(value, OnObjectPropertyChanged, false);
                            if (weakListener == null)
                            {
                                Log.Debug("Failed to use weak events to subscribe to 'value.PropertyChanged', going to subscribe without weak events");

                                ((INotifyPropertyChanged)value).PropertyChanged += OnObjectPropertyChanged;
                            }
                        }
                        break;

                    case EventChangeType.Collection:
                        weakListener = this.SubscribeToWeakCollectionChangedEvent(value, OnObjectCollectionChanged, false);
                        if (weakListener == null)
                        {
                            Log.Debug("Failed to use weak events to subscribe to 'value.CollectionChanged', going to subscribe without weak events");

                            ((INotifyCollectionChanged)value).CollectionChanged += OnObjectCollectionChanged;
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("eventChangeType");
                }

                if (eventsTable != null)
                {
                    eventsTable.Add(value, weakListener);
                }

                if (eventsList != null)
                {
                    eventsList.Add(weakListener);
                }
            }
        }

        private void UnsubscribeNotifyChangedEvent(object value, EventChangeType eventChangeType, ICollection parentCollection)
        {
            if (value == null)
            {
                return;
            }

            lock (_lockObject)
            {
                ConditionalWeakTable<object, IWeakEventListener> eventsTable;
                List<IWeakEventListener> eventsList;

                switch (eventChangeType)
                {
                    case EventChangeType.Property:
                        eventsTable = _weakPropertyChangedListenersTable;
                        eventsList = _weakPropertyChangedListeners;

                        if (parentCollection != null)
                        {
                            List<WeakReference> collectionItems;
                            if (_collectionItems.TryGetValue(parentCollection, out collectionItems))
                            {
                                // TODO: Consider less costly way to determine this
                                for (var i = 0; i < collectionItems.Count; i++)
                                {
                                    if (ReferenceEquals(collectionItems[i].Target, value))
                                    {
                                        collectionItems.RemoveAt(i--);
                                        break;
                                    }
                                }
                            }
                        }
                        break;

                    case EventChangeType.Collection:
                        eventsTable = _weakCollectionChangedListenersTable;
                        eventsList = _weakCollectionChangedListeners;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("eventChangeType");
                }

                IWeakEventListener oldSubscription;
                if (eventsTable != null && eventsTable.TryGetValue(value, out oldSubscription))
                {
                    if (oldSubscription != null)
                    {
                        oldSubscription.Detach();

                        eventsList.Remove(oldSubscription);
                    }

                    eventsTable.Remove(value);
                }

                if (value is ICollection)
                {
                    List<WeakReference> collectionItems;
                    if (_collectionItems.TryGetValue(value, out collectionItems))
                    {
                        foreach (var item in collectionItems)
                        {
                            if (item.IsAlive)
                            {
                                var actualItem = item.Target;
                                UnsubscribeNotifyChangedEvents(actualItem, parentCollection);
                            }
                        }

                        collectionItems.Clear();
                        _collectionItems.Remove(value);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Occurs when the <see cref="PropertyChanged"/> event occurs on the target object.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="PropertyChanged"/> event occurs in the collection when the target object is a collection.
        /// </summary>
        public event PropertyChangedEventHandler CollectionItemPropertyChanged;

        /// <summary>
        /// Occurs when the <see cref="CollectionChanged"/> event occurs on the target object.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}