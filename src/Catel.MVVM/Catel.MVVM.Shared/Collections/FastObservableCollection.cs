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
    using Catel.Logging;

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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<IDispatcherService> _dispatcherService = new Lazy<IDispatcherService>(() =>
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            return dependencyResolver.Resolve<IDispatcherService>();
        });
        #endregion

        #region Fields
        /// <summary>
        /// The current suspension context.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private SuspensionContext<T> _suspensionContext;
        #endregion

        #region Constructors
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
                return _suspensionContext != null;
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
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The start index.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public virtual void InsertItems(IEnumerable<T> collection, int index)
        {
            InsertItems(collection, index, SuspensionMode.None);
        }

        /// <summary>
        /// Inserts the elements of the specified collection at the specified index.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The start index.</param>
        /// <param name="mode">The suspension mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public virtual void InsertItems(IEnumerable<T> collection, int index, SuspensionMode mode)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(mode))
            {
                foreach (var item in collection)
                {
                    Insert(index++, item);
                }
            }
        }

        /// <summary>
        /// Inserts the elements of the specified collection at the specified index.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The start index.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public virtual void InsertItems(IEnumerable collection, int index)
        {
            InsertItems(collection, index, SuspensionMode.None);
        }

        /// <summary>
        /// Inserts the elements of the specified collection at the specified index.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The start index.</param>
        /// <param name="mode">The suspension mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public virtual void InsertItems(IEnumerable collection, int index, SuspensionMode mode)
        {
            Argument.IsNotNull("collection", collection);

            var list = (IList)this;

            using (SuspendChangeNotifications(mode))
            {
                foreach (var item in collection)
                {
                    list.Insert(index++, item);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="E:System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged" /> with 
        /// <see cref="F:System.Collections.Specialized.NotifyCollectionChangedAction.Reset" /> changed action.
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
        /// Adds the specified items to the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void AddItems(IEnumerable<T> collection)
        {
            AddItems(collection, SuspensionMode.None);
        }

        /// <summary>
        /// Adds the specified items to the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="mode">The suspension mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void AddItems(IEnumerable<T> collection, SuspensionMode mode)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(mode))
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
            // Don't create overload, keep as is
            AddItems(collection, SuspensionMode.None);
        }

        /// <summary>
        /// Adds the specified items to the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="mode">The suspension mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void AddItems(IEnumerable collection, SuspensionMode mode)
        {
            Argument.IsNotNull("collection", collection);

            var list = (IList)this;

            using (SuspendChangeNotifications(mode))
            {
                foreach (var item in collection)
                {
                    list.Add(item);
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
            // Don't create overload, keep as is
            RemoveItems(collection, SuspensionMode.None);
        }

        /// <summary>
        /// Removes the specified items from the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="mode">The suspension mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void RemoveItems(IEnumerable<T> collection, SuspensionMode mode)
        {
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(mode))
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
            // Don't create overload, keep as is
            RemoveItems(collection, SuspensionMode.None);
        }

        /// <summary>
        /// Removes the specified items from the collection without causing a change notification for all items.
        /// <para />
        /// This method will raise a change notification at the end.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="mode">The suspension mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public void RemoveItems(IEnumerable collection, SuspensionMode mode)
        {
            Argument.IsNotNull("collection", collection);

            var list = (IList)this;

            using (SuspendChangeNotifications(mode))
            {
                foreach (var item in collection)
                {
                    list.Remove(item);
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
            if (_suspensionContext == null)
            {
                // Create new context
                _suspensionContext = new SuspensionContext<T>(mode);
            }
            else if (_suspensionContext != null && (_suspensionContext.Mode != mode && _suspensionContext.Mode != SuspensionMode.Mixed))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot change mode during another active suspension.");
            }

            return new DisposableToken<FastObservableCollection<T>>(
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

        /// <summary>
        /// Notifies external classes of property changes.
        /// </summary>
        protected void NotifyChanges()
        {
            Action action = () =>
            {
                var eventArgsList = new List<NotifyCollectionChangedEventArgs>();

                var suspensionContext = _suspensionContext;
                if (suspensionContext != null)
                {
                    // Special case: if suspension mode == None, we raise a .Reset (default behavior). We would love to pass in the 
                    // right items (e.g. Reset or MassiveUpdate), but the UI components don't support this, hence the .Reset.
                    //
                    // See https://github.com/Catel/Catel/issues/1066 for details
                    if (suspensionContext.Mode == SuspensionMode.None)
                    {
                        if (suspensionContext.NewItems.Count > 0 || suspensionContext.OldItems.Count > 0)
                        {
                            eventArgsList.Add(CreateEventArgs(NotifyCollectionChangedAction.Reset, null, null));
                        }
                    }
                    else
                    {
                        if (suspensionContext.NewItems.Count != 0)
                        {
                            eventArgsList.Add(CreateEventArgs(NotifyCollectionChangedAction.Add, suspensionContext.NewItems, suspensionContext.NewItemIndices));
                        }

                        if (suspensionContext.OldItems.Count != 0)
                        {
                            eventArgsList.Add(CreateEventArgs(NotifyCollectionChangedAction.Remove, suspensionContext.OldItems, suspensionContext.OldItemIndices));
                        }
                    }
                }
                else
                {
                    eventArgsList.Add(CreateEventArgs(NotifyCollectionChangedAction.Reset, null, null));
                }

                if (eventArgsList.Count > 0)
                {
                    OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

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

        /// <summary>
        /// Raises the <see cref="ObservableCollection{T}.CollectionChanged" /> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var suspensionContext = _suspensionContext;
            if (suspensionContext == null || suspensionContext.Count == 0)
            {
                if (AutomaticallyDispatchChangeNotifications)
                {
                    _dispatcherService.Value.BeginInvokeIfRequired(() => base.OnCollectionChanged(e));
                }
                else
                {
                    base.OnCollectionChanged(e);
                }

                return;
            }

            if (suspensionContext.Count != 0)
            {
                IsDirty = true;
            }
        }

        /// <summary>
        /// Raises the <c>ObservableCollection{T}.PropertyChanged</c> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var suspensionContext = _suspensionContext;
            if (suspensionContext == null || suspensionContext.Count == 0)
            {
                if (AutomaticallyDispatchChangeNotifications)
                {
                    _dispatcherService.Value.BeginInvokeIfRequired(() => base.OnPropertyChanged(e));
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
            var suspensionContext = _suspensionContext;
            if (suspensionContext != null && suspensionContext.Mode != SuspensionMode.None && suspensionContext.Mode != SuspensionMode.Mixed)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Clearing items is only allowed in SuspensionMode.None, current mode is '{suspensionContext.Mode}'");
            }

            if (_suspensionContext != null && (_suspensionContext.Mode == SuspensionMode.None || _suspensionContext.Mode == SuspensionMode.Mixed))
            {
                while (Count > 0)
                {
                    RemoveItem(0);
                }
            }
            else
            {
                // Call base
                base.ClearItems();
            }
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            // Check
            var suspensionContext = _suspensionContext;
            if (suspensionContext != null && suspensionContext.Mode == SuspensionMode.Removing)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
            }

            bool? removed = suspensionContext?.TryRemoveItemFromOldItems(index, item);

            // Call base
            base.InsertItem(index, item);

            if (removed != null && !removed.Value)
            {
                // Remember
                suspensionContext.NewItems.Add(item);
                suspensionContext.NewItemIndices.Add(index);
            }
        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param><param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            // Check
            if (_suspensionContext != null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Mixed))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Moving items is only allowed in SuspensionMode.None, current mode is '{_suspensionContext.Mode}'");
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
            var suspensionContext = _suspensionContext;
            if (suspensionContext != null && suspensionContext.Mode == SuspensionMode.Adding)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Removing items is not allowed in mode SuspensionMode.Adding.");
            }

            // Get item
            T item = this[index];

            bool? removed = suspensionContext?.TryRemoveItemFromNewItems(index, item);

            // Call base
            base.RemoveItem(index);

            if (removed != null && !removed.Value)
            {
                // Remember
                suspensionContext.OldItems.Add(item);
                suspensionContext.OldItemIndices.Add(index);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param><param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            // Check
            if (_suspensionContext != null && _suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Mixed)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Replacing items is only allowed in SuspensionMode.None, current mode is '{_suspensionContext.Mode}'");
            }

            // Call base
            base.SetItem(index, item);
        }
        #endregion Overrides of ObservableCollection

        private NotifyRangedCollectionChangedEventArgs CreateEventArgs(NotifyCollectionChangedAction action, IList changedItems = null, IList<int> changedIndices = null)
        {
            NotifyRangedCollectionChangedEventArgs eventArgs;

            if (changedItems == null && changedIndices == null)
            {
                eventArgs = new NotifyRangedCollectionChangedEventArgs(action);
            }
            else
            {
                eventArgs = new NotifyRangedCollectionChangedEventArgs(action, changedItems, changedIndices);
            }

            return eventArgs;
        }
        #endregion
    }
}