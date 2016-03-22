// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
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
    using System.Diagnostics;

    using IoC;
    using Services;

    /// <summary>
    /// Fast implementation of <see cref="ObservableCollection{T}"/> where the change notifications
    /// can be suspended.
    /// </summary>
    /// <typeparam name="T">Type of the elements contained by this collection.</typeparam>
#if NET
    [Serializable]
#endif
    public class FastObservableCollection<T> : ObservableCollection<T>, ISuspendChangeNotificationsCollection
    {
        #region Constants
        private static readonly IDispatcherService _dispatcherService;
        #endregion

        #region Fields
        private bool _suspendChangeNotifications;

        /// <summary>
        /// Added items while suspending notifications.
        /// </summary>
        private readonly LinkedList<Tuple<int, T>> _newItems = new LinkedList<Tuple<int, T>>();

        /// <summary>
        /// Removed items while suspending notifications.
        /// </summary>
        private readonly LinkedList<Tuple<int, T>> _oldItems = new LinkedList<Tuple<int, T>>();

        /// <summary>
        /// The current suspension mode.
        /// </summary>
        private SuspensionMode _suspensionMode;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="FastObservableCollection{T}"/> class.
        /// </summary>
        static FastObservableCollection()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            _dispatcherService = dependencyResolver.Resolve<IDispatcherService>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastObservableCollection{T}" /> class.
        /// </summary>
        public FastObservableCollection()
        {
            AutomaticallyDispatchChangeNotifications = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public FastObservableCollection(IEnumerable<T> collection)
            : this()
        {
            AddItems(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public FastObservableCollection(IEnumerable collection)
            : this()
        {
            AddItems(collection);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether change to the collection is made when
        /// its notifications are suspended.
        /// </summary>
        /// <value><c>true</c> if this instance is has been changed while notifications are
        /// suspended; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get; protected set;
        }

        /// <summary>
        /// Gets a value indicating whether change notifications are suspended.
        /// </summary>
        /// <value>
        /// <c>True</c> if notifications are suspended, otherwise, <c>false</c>.
        /// </value>
        public bool NotificationsSuspended
        {
            get
            {
                return _suspendChangeNotifications;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Inserts the elements of the specified collection at the specified index.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The start index.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public virtual void InsertItems(IEnumerable<T> collection, int index)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(SuspensionMode.Adding))
            {
                foreach (var item in collection)
                {
                    Insert(index++, item);
                }
            }
        }

        /// <summary>
        /// Inserts the elements of the specified collection at the specified index.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The start index.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public virtual void InsertItems(IEnumerable collection, int index)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(SuspensionMode.Adding))
            {
                foreach (var item in collection)
                {
                    ((IList)this).Insert(index++, item);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="E:System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged" /> with 
        /// <see cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" /> changed action.
        /// </summary>
        public void Reset()
        {
            var oldSuspensionMode = _suspensionMode;
            _suspensionMode = SuspensionMode.Mixed;

            NotifyChanges();

            _suspensionMode = oldSuspensionMode;
        }

        /// <summary>
        /// Adds the specified items to the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void AddItems(IEnumerable<T> collection)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(SuspensionMode.Adding))
            {
                foreach (var item in collection)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Adds the specified items to the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void AddItems(IEnumerable collection)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(SuspensionMode.Adding))
            {
                foreach (var item in collection)
                {
                    ((IList)this).Add(item);
                }
            }
        }

        /// <summary>
        /// Removes the specified items from the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void RemoveItems(IEnumerable<T> collection)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(SuspensionMode.Removing))
            {
                foreach (var item in collection)
                {
                    Remove(item);
                }
            }
        }

        /// <summary>
        /// Removes the specified items from the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void RemoveItems(IEnumerable collection)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(SuspensionMode.Removing))
            {
                foreach (var item in collection)
                {
                    ((IList)this).Remove(item);
                }
            }
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
            return SuspendChangeNotifications(SuspensionMode.Mixed);
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
            // Set suspension mode
            _suspensionMode = mode;

            return new DisposableToken<FastObservableCollection<T>>(
                this,
                x =>
                {
                    x.Instance._suspendChangeNotifications = true;
                },
                x =>
                {
                    x.Instance._suspendChangeNotifications = (bool)x.Tag;
                    if (x.Instance.IsDirty && !x.Instance._suspendChangeNotifications)
                    {
                        x.Instance.IsDirty = false;
                        x.Instance.NotifyChanges();
                    }
                }, _suspendChangeNotifications);
        }

        /// <summary>
        /// Notifies external classes of property changes.
        /// </summary>
        protected void NotifyChanges()
        {
            Action action = () =>
            {
                // Create event args
                NotifyCollectionChangedEventArgs eventArgs;
                if (_suspensionMode == SuspensionMode.Adding)
                {
                    if (_newItems.Count == 0)
                    {
                        return;
                    }

                    var items = new List<T>(_newItems.Count);
                    var indices = new List<int>(_newItems.Count);
                    foreach (var newItemTuple in _newItems)
                    {
                        items.Add(newItemTuple.Item2);
                        indices.Add(newItemTuple.Item1);
                    }

                    eventArgs = new NotifyRangedCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, indices);
                }
                else if (_suspensionMode == SuspensionMode.Removing)
                {
                    if (_oldItems.Count == 0)
                    {
                        return;
                    }

                    var items = new List<T>(_oldItems.Count);
                    var indices = new List<int>(_oldItems.Count);
                    foreach (var oldItemTuple in _oldItems)
                    {
                        items.Add(oldItemTuple.Item2);
                        indices.Add(oldItemTuple.Item1);
                    }

                    eventArgs = new NotifyRangedCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, indices);
                }
                else
                {
                    Debug.Assert(_suspensionMode == SuspensionMode.Mixed, "Wrong/unknown suspension mode!");
                    eventArgs = new NotifyRangedCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                }

                // Fire events
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(eventArgs);

                // Reset cached items
                _newItems.Clear();
                _oldItems.Clear();
            };

            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.BeginInvokeIfRequired(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Raises the <see cref="ObservableCollection{T}.CollectionChanged" /> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suspendChangeNotifications)
            {
                if (AutomaticallyDispatchChangeNotifications)
                {
                    _dispatcherService.BeginInvokeIfRequired(() => base.OnCollectionChanged(e));
                }
                else
                {
                    base.OnCollectionChanged(e);
                }

                return;
            }

            IsDirty = true;
        }

        /// <summary>
        /// Raises the <c>ObservableCollection{T}.PropertyChanged</c> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!_suspendChangeNotifications)
            {
                if (AutomaticallyDispatchChangeNotifications)
                {
                    _dispatcherService.BeginInvokeIfRequired(() => base.OnPropertyChanged(e));
                }
                else
                {
                    base.OnPropertyChanged(e);
                }
            }
        }

        #region Overrides of ObservableCollection
        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            // Check
            if (_suspendChangeNotifications && _suspensionMode != SuspensionMode.Mixed)
            {
                throw new InvalidOperationException("Clearing items is not allowed in modes other than mixed.");
            }

            // Call base
            base.ClearItems();
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            // Check
            if (_suspendChangeNotifications && _suspensionMode == SuspensionMode.Removing)
            {
                throw new InvalidOperationException("Adding items is not allowed in removing mode.");
            }

            // Call base
            base.InsertItem(index, item);

            if (_suspendChangeNotifications && _suspensionMode == SuspensionMode.Adding)
            {
                // Remember
                _newItems.AddLast(Tuple.Create(index, item));
            }
        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param><param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            // Check
            if (_suspendChangeNotifications && _suspensionMode != SuspensionMode.Mixed)
            {
                throw new InvalidOperationException("Moving items is not allowed in modes other than mixed.");
            }

            // Call base
            base.MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            // Check
            if (_suspendChangeNotifications && _suspensionMode == SuspensionMode.Adding)
            {
                throw new InvalidOperationException("Removing items is not allowed in adding mode.");
            }

            // Get item
            T item;
            if (_suspendChangeNotifications && _suspensionMode == SuspensionMode.Removing)
            {
                item = this[index];
            }
            else
            {
                item = default(T);
            }

            // Call base
            base.RemoveItem(index);

            if (_suspendChangeNotifications && _suspensionMode == SuspensionMode.Removing)
            {
                // Remember
                _oldItems.AddLast(Tuple.Create(index, item));
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param><param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            // Check
            if (_suspendChangeNotifications && _suspensionMode != SuspensionMode.Mixed)
            {
                throw new InvalidOperationException("Replacing items is not allowed in modes other than mixed.");
            }

            // Call base
            base.SetItem(index, item);
        }
        #endregion Overrides of ObservableCollection
        #endregion
    }
}