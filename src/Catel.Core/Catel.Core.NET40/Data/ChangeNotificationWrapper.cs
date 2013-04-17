// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationWrapper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Logging;

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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly WeakReference _weakReference;
        private readonly ConditionalWeakTable<object, IWeakEventListener> _weakPropertyChangedListenersTable = new ConditionalWeakTable<object, IWeakEventListener>();
        private readonly List<IWeakEventListener> _weakPropertyChangedListeners = new List<IWeakEventListener>();

        private readonly ConditionalWeakTable<object, IWeakEventListener> _weakCollectionChangedListenersTable = new ConditionalWeakTable<object, IWeakEventListener>();
        private readonly List<IWeakEventListener> _weakCollectionChangedListeners = new List<IWeakEventListener>();

        private readonly object _lockObject = new object();

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

            SubscribeNotifyChangedEvents(value, false);
        }

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
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    UnsubscribeNotifyChangedEvents(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    SubscribeNotifyChangedEvents(item, true);
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
                foreach (var weakListener in _weakPropertyChangedListeners)
                {
                    weakListener.Detach();
                }

                _weakPropertyChangedListeners.Clear();

                foreach (var weakListener in _weakCollectionChangedListeners)
                {
                    weakListener.Detach();
                }

                _weakCollectionChangedListeners.Clear();
            }
        }

        /// <summary>
        /// Unsubscribes from the notify changed events.
        /// </summary>
        /// <param name="value">The object to unsubscribe from.</param>
        /// <remarks>
        /// No need to check for weak events, they are unsubscribed automatically.
        /// </remarks>
        private void UnsubscribeNotifyChangedEvents(object value)
        {
            if (value != null)
            {
                var propertyChangedValue = value as INotifyPropertyChanged;
                if (propertyChangedValue != null)
                {
                    UnsubscribeNotifyChangedEvent(propertyChangedValue, EventChangeType.Property);
                }

                var collectionChangedValue = value as INotifyCollectionChanged;
                if (collectionChangedValue != null)
                {
                    UnsubscribeNotifyChangedEvent(collectionChangedValue, EventChangeType.Collection);

                    foreach (var child in (IEnumerable)value)
                    {
                        UnsubscribeNotifyChangedEvents(child);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to the notify changed events.
        /// </summary>
        /// <param name="value">The object to subscribe to.</param>
        /// <param name="isCollectionItem">If set to <c>true</c>, this is a collection item which should use <see cref="OnObjectCollectionItemPropertyChanged"/>.</param>
        private void SubscribeNotifyChangedEvents(object value, bool isCollectionItem)
        {
            if (value != null)
            {
                var collectionChangedValue = value as INotifyCollectionChanged;
                if (collectionChangedValue != null)
                {
                    SubscribeNotifyChangedEvent(collectionChangedValue, EventChangeType.Collection, isCollectionItem);

                    foreach (var child in (IEnumerable)value)
                    {
                        SubscribeNotifyChangedEvents(child, true);
                    }
                }
                else
                {
                    var propertyChangedValue = value as INotifyPropertyChanged;
                    if (propertyChangedValue != null)
                    {
                        // ObservableObject implements PropertyChanged as protected, make sure we accept that in non-.NET languages such
                        // as Silverlight, Windows Phone and WinRT
                        try
                        {
                            SubscribeNotifyChangedEvent(propertyChangedValue, EventChangeType.Property, isCollectionItem);
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, "Failed to subscribe to PropertyChanged event, the event is probably not public");
                        }
                    }
                }
            }
        }

        private void SubscribeNotifyChangedEvent(object value, EventChangeType eventChangeType, bool isCollectionItem)
        {
            if (value == null)
            {
                return;
            }

            ConditionalWeakTable<object, IWeakEventListener> eventsTable;
            List<IWeakEventListener> eventsList;

            switch (eventChangeType)
            {
                case EventChangeType.Property:
                    eventsTable = _weakPropertyChangedListenersTable;
                    eventsList = _weakPropertyChangedListeners;
                    break;

                case EventChangeType.Collection:
                    eventsTable = _weakCollectionChangedListenersTable;
                    eventsList = _weakCollectionChangedListeners;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("eventChangeType");
            }

            lock (_lockObject)
            {
                IWeakEventListener oldSubscription;
                if (eventsTable.TryGetValue(value, out oldSubscription))
                {
                    oldSubscription.Detach();

                    eventsList.Remove(oldSubscription);
                    eventsTable.Remove(value);
                }

                IWeakEventListener weakListener;
                switch (eventChangeType)
                {
                    case EventChangeType.Property:
                        if (isCollectionItem)
                        {
                            weakListener = this.SubscribeToWeakPropertyChangedEvent(value, OnObjectCollectionItemPropertyChanged);
                        }
                        else
                        {
                            weakListener = this.SubscribeToWeakPropertyChangedEvent(value, OnObjectPropertyChanged);
                        }
                        break;

                    case EventChangeType.Collection:
                        weakListener = this.SubscribeToWeakCollectionChangedEvent(value, OnObjectCollectionChanged);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("eventChangeType");
                }

                eventsTable.Add(value, weakListener);
                eventsList.Add(weakListener);
            }
        }

        private void UnsubscribeNotifyChangedEvent(object value, EventChangeType eventChangeType)
        {
            if (value == null)
            {
                return;
            }

            ConditionalWeakTable<object, IWeakEventListener> eventsTable;
            List<IWeakEventListener> eventsList;

            switch (eventChangeType)
            {
                case EventChangeType.Property:
                    eventsTable = _weakPropertyChangedListenersTable;
                    eventsList = _weakPropertyChangedListeners;
                    break;

                case EventChangeType.Collection:
                    eventsTable = _weakCollectionChangedListenersTable;
                    eventsList = _weakCollectionChangedListeners;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("eventChangeType");
            }

            lock (_lockObject)
            {
                IWeakEventListener oldSubscription;
                if (eventsTable.TryGetValue(value, out oldSubscription))
                {
                    oldSubscription.Detach();

                    eventsList.Remove(oldSubscription);
                    eventsTable.Remove(value);
                }
            }
        }
    }
}