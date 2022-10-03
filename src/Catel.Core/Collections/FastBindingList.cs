namespace Catel.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    using IoC;
    using Logging;
    using Services;

    /// <summary>
    /// Fast implementation of <see cref="BindingList{T}"/> where the change notifications
    /// can be suspended.
    /// </summary>
    /// <typeparam name="T">Type of the elements contained by this collection.</typeparam>
    [Serializable]
    public class FastBindingList<T> : BindingList<T>, ISuspendChangeNotificationsCollection
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly IDispatcherService _dispatcherService;

        private bool _sorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor? _sortProperty;

        /// <summary>
        /// The current suspension context.
        /// </summary>
        private ExtendedSuspensionContext<T>? _suspensionContext;

        /// <summary>
        /// Initializes static members of the <see cref="FastBindingList{T}"/> class.
        /// </summary>
        static FastBindingList()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            _dispatcherService = dependencyResolver.Resolve<IDispatcherService>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBindingList{T}" /> class.
        /// </summary>
        public FastBindingList()
        {
            AutomaticallyDispatchChangeNotifications = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBindingList{T}" /> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public FastBindingList(IEnumerable<T> collection)
            : this()
        {
            AddItems(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastBindingList{T}" /> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public FastBindingList(IEnumerable collection)
            : this()
        {
            AddItems(collection);
        }

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
        public bool NotificationsSuspended
        {
            get
            {
                return _suspensionContext is not null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether events should automatically be dispatched to the UI thread.
        /// </summary>
        /// <value><c>true</c> if events should automatically be dispatched to the UI thread; otherwise, <c>false</c>.</value>
        public bool AutomaticallyDispatchChangeNotifications { get; set; }

        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        /// <value><c>true</c> if the list is sorted, otherwise, <c>false</c>.</value>
        protected override bool IsSortedCore
        {
            get
            {
                return _sorted;
            }
        }

        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        /// <value>One of the <see cref="ListSortDirection"/> values. The default is <see cref="ListSortDirection.Ascending"/>.</value>
        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return _sortDirection;
            }
        }

        /// <summary>
        /// Gets the property descriptor that is used for sorting the list.
        /// </summary>
        /// <value>The <see cref="PropertyDescriptor"/> used for sorting the list.</value>
        protected override PropertyDescriptor? SortPropertyCore
        {
            get
            {
                return _sortProperty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list supports searching.
        /// </summary>
        /// <value><c>true</c> if the list supports searching; otherwise, <c>false</c>.</value>
        protected override bool SupportsSearchingCore
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        /// <value><c>true</c> if the list supports sorting; otherwise, <c>false</c>.</value>
        protected override bool SupportsSortingCore
        {
            get
            {
                return true;
            }
        }

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

            var list = (IList)this;

            using (SuspendChangeNotifications(SuspensionMode.Adding))
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
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(_suspensionContext?.Mode ?? SuspensionMode.Adding))
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

            var list = (IList)this;

            using (SuspendChangeNotifications(_suspensionContext?.Mode ?? SuspensionMode.Adding))
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
            Argument.IsNotNull("collection", collection);

            using (SuspendChangeNotifications(_suspensionContext?.Mode ?? SuspensionMode.Removing))
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

            var list = (IList)this;

            using (SuspendChangeNotifications(_suspensionContext?.Mode ?? SuspensionMode.Removing))
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
        /// var fastCollection = new FastBindingList<int>();
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
        /// var fastCollection = new FastBindingList<int>();
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
                _suspensionContext = new ExtendedSuspensionContext<T>(mode);
            }
            else if (_suspensionContext.Mode != mode)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot change mode during another active suspension.");
            }

            return new DisposableToken<FastBindingList<T>>(
                this,
                x =>
                {
                    if (x.Instance._suspensionContext!.Count == 0)
                    {
                        x.Instance.RaiseListChangedEvents = false;
                        x.Instance.IsDirty = true;
                    }
                    x.Instance._suspensionContext.Count++;
                },
                x =>
                {
                    x.Instance._suspensionContext!.Count--;
                    if (x.Instance._suspensionContext.Count == 0)
                    {
                        x.Instance.RaiseListChangedEvents = true;
                        x.Instance.IsDirty = false;
                        x.Instance.NotifyChanges();

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
                // Create event args
                var eventArgsList = new List<ListChangedEventArgs>();

                var suspensionContext = _suspensionContext;
                if (suspensionContext is not null)
                {
                    if (suspensionContext.NewItems.Count != 0)
                    {
                        eventArgsList.Add(new NotifyRangedListChangedEventArgs(NotifyRangedListChangedAction.Add, suspensionContext.NewItems, suspensionContext.NewItemIndices));
                    }

                    if (suspensionContext.OldItems.Count != 0)
                    {
                        eventArgsList.Add(new NotifyRangedListChangedEventArgs(NotifyRangedListChangedAction.Remove, suspensionContext.OldItems, suspensionContext.OldItemIndices));
                    }
                }
                else
                {
                    eventArgsList.Add(new NotifyListChangedEventArgs(ListChangedType.Reset));
                }

                // Fire events
                foreach (var eventArgs in eventArgsList)
                {
                    OnListChanged(eventArgs);
                }
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
        /// Raises the <see cref="BindingList{T}.ListChanged" /> event, but also makes sure the event is dispatched to the UI thread.
        /// </summary>
        /// <param name="e">The <see cref="ListChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            // Convert args if needed
            if (!(e is NotifyListChangedEventArgs))
            {
                if (e.NewIndex >= 0 && e.OldIndex >= 0)
                {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                    e = new NotifyListChangedEventArgs(e.ListChangedType,
                        e.NewIndex,
                        e.NewIndex >= 0 ? this[e.NewIndex] : default,
                        e.OldIndex,
                        e.OldIndex >= 0 ? this[e.OldIndex] : default);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                }
                else
                {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                    e = new NotifyListChangedEventArgs(e.ListChangedType,
                        e.NewIndex,
                        e.NewIndex >= 0 ? this[e.NewIndex] : default,
                        e.PropertyDescriptor);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                }
            }

            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.BeginInvokeIfRequired(() => base.OnListChanged(e));
            }
            else
            {
                base.OnListChanged(e);
            }
        }

        /// <summary>
        /// Apply sort.
        /// </summary>
        /// <param name="prop">The <see cref="PropertyDescriptor" /> that specifies the property to sort on.</param>
        /// <param name="direction">One of the <see cref="ListSortDirection"/> values.</param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            var list = Items as List<T>;
            if (list is null)
            {
                return;
            }

            list.Sort((lhs, rhs) =>
            {
                var result = 0;

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                var lhsValue = lhs is null ? null : _sortProperty.GetValue(lhs);
                var rhsValue = rhs is null ? null : _sortProperty.GetValue(rhs);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                if (lhsValue is null && rhsValue is null) // both values are null, both equal
                {
                    result = 0;
                }
                else if (lhsValue is null) // lhs value is null, rhs not, rhs value is greater
                {
                    result = -1;
                }
                else if (rhsValue is null) // rhs value is null, lhs not, lhs value is greater
                {
                    result = 1;
                }
                else if (lhsValue is IComparable comparable) // lhs is IComparable, compare to rhs
                {
                    result = comparable.CompareTo(rhsValue);
                }
                else if (lhsValue.Equals(rhsValue)) // check if both values are equal
                {
                    result = 0;
                }
                else // compare as string
                {
                    result = (lhsValue.ToString() ?? string.Empty).CompareTo(rhsValue.ToString());
                }

                if (_sortDirection == ListSortDirection.Descending)
                {
                    result = -result;
                }

                return result;
            });

            _sorted = true;

            OnListChanged(new NotifyListChangedEventArgs(ListChangedType.Reset));
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            // Check
            if (_suspensionContext is not null && _suspensionContext.Mode != SuspensionMode.None)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Clearing items is only allowed in SuspensionMode.None, current mode is '{Enum<SuspensionMode>.ToString(_suspensionContext.Mode)}'.");
            }

            if (_suspensionContext is not null && _suspensionContext.Mode == SuspensionMode.None)
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
        /// Search for the index of item that has the specified property descriptor with the specified value.
        /// </summary>
        /// <param name="prop">The <see cref="PropertyDescriptor" /> that specifies the property to search on.</param>
        /// <param name="key">The value of property to match.</param>
        protected override int FindCore(PropertyDescriptor prop, object key)
        {
            if (prop is null)
            {
                return -1;
            }

            var list = Items as List<T>;
            if (list is null)
            {
                return -1;
            }

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                if (object.Equals(prop.GetValue(item), key))
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            // Check
            var suspensionContext = _suspensionContext;
            if (suspensionContext is not null && suspensionContext.Mode == SuspensionMode.Removing)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
            }

            // Call base
            var oldValue = RaiseListChangedEvents;
            RaiseListChangedEvents = false;

            bool? removed;
            try
            {
                removed = suspensionContext?.TryRemoveItemFromOldItems(index, item);

                base.InsertItem(index, item);

                if (suspensionContext is null)
                {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                    OnListChanged(new NotifyListChangedEventArgs(ListChangedType.ItemAdded, index, item));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                }
            }
            finally
            {
                RaiseListChangedEvents = oldValue;
            }

            if (removed is not null && !removed.Value)
            {
                // Remember
                suspensionContext?.NewItems.Add(item);
                suspensionContext?.NewItemIndices.Add(index);
            }
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            // Check
            if (_suspensionContext is not null && _suspensionContext.Mode == SuspensionMode.Adding)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Removing items is not allowed in mode SuspensionMode.Adding.");
            }

            // Get item
            var item = this[index];

            // Call base
            var oldValue = RaiseListChangedEvents;
            RaiseListChangedEvents = false;

            bool? removed;
            try
            {
                removed = _suspensionContext?.TryRemoveItemFromNewItems(index, item);
                base.RemoveItem(index);

                if (!NotificationsSuspended)
                {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                    OnListChanged(new NotifyListChangedEventArgs(ListChangedType.ItemDeleted, index, item));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                }
            }
            finally
            {
                RaiseListChangedEvents = oldValue;
            }

            if (removed is not null && !removed.Value)
            {
                // Remember
                _suspensionContext?.OldItems.Add(item);
                _suspensionContext?.OldItemIndices.Add(index);
            }
        }

        /// <summary>
        /// Removes any applied sort.
        /// </summary>
        protected override void RemoveSortCore()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            // Check
            if (_suspensionContext is not null && _suspensionContext.Mode != SuspensionMode.None)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Replacing items is only allowed in SuspensionMode.None, current mode is '{Enum<SuspensionMode>.ToString(_suspensionContext.Mode)}'");
            }

            // Get old item
            var oldItem = this[index];

            // Call base
            var oldValue = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            try
            {
                base.SetItem(index, item);

                if (!NotificationsSuspended)
                {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
                    OnListChanged(new NotifyListChangedEventArgs(ListChangedType.ItemChanged, index, item, -1, oldItem));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
                }
            }
            finally
            {
                RaiseListChangedEvents = oldValue;
            }
        }
    }
}
