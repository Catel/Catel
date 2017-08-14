// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastBindingList.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Catel.Logging;

    using IoC;
    using Services;

    /// <summary>
    /// Fast implementation of <see cref="BindingList{T}"/> where the change notifications
    /// can be suspended.
    /// </summary>
    /// <typeparam name="T">Type of the elements contained by this collection.</typeparam>
    [Serializable]
    public class FastBindingList<T> : BindingList<T>, ISuspendChangeNotificationsCollection
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly IDispatcherService _dispatcherService;
        #endregion

        #region Fields
        private bool _sorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortProperty;

        /// <summary>
        /// The current suspension context.
        /// </summary>
        [field: NonSerialized]
        private SuspensionContext<T> _suspensionContext;
        #endregion

        #region Constructors
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

        #region Overrides of BindingList
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
        protected override PropertyDescriptor SortPropertyCore
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
        #endregion Overrides of BindingList
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
                    if (IsInsertRequired(item))
                    {
                        Insert(index, item);
                    }
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
                foreach (var item in collection.Cast<T>())
                {
                    if (IsInsertRequired(item))
                    {
                        list.Insert(index, item);
                    }
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
                var lastIdx = Math.Max(0, Count - 1);
                foreach (var item in collection.Reverse())
                {
                    if (IsInsertRequired(item))
                    {
                        Insert(lastIdx, item);
                    }
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
                var lastIdx = Math.Max(0, Count - 1);
                foreach (var item in collection.OfType<T>())
                {
                    if (IsInsertRequired(item))
                    {
                        Insert(lastIdx, item);
                    }
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
                    if (IsRemoveRequired(item))
                    {
                        Remove(item);
                    }
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
                foreach (var item in collection.Cast<T>())
                {
                    if (IsRemoveRequired(item))
                    {
                        list.Remove(item);
                    }
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
            if (_suspensionContext == null)
            {
                // Create new context
                _suspensionContext = new SuspensionContext<T>(mode);
            }
            else if (_suspensionContext != null && _suspensionContext.Mode != mode)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Cannot change mode during another active suspension.");
            }

            return new DisposableToken<FastBindingList<T>>(
                this,
                x =>
                {
                    if (x.Instance._suspensionContext.Count == 0)
                    {
                        x.Instance.RaiseListChangedEvents = false;
                        x.Instance.IsDirty = true;
                    }
                    x.Instance._suspensionContext.Count++;
                },
                x =>
                {
                    if (x.Instance._suspensionContext.Count == 1 && x.Instance._suspensionContext.Mode != SuspensionMode.None)
                    {
                        x.Instance.SynchronizeFromSuspensionContext();
                    }

                    x.Instance._suspensionContext.Count--;
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
        /// Synchronize the list with the suspenstion context.
        /// </summary>
        private void SynchronizeFromSuspensionContext()
        {
            _suspensionContext.Synchronize((idx, value) => base.InsertItem(idx, value), idx => base.RemoveItem(idx));
        }

        /// <summary>
        /// Notifies external classes of property changes.
        /// </summary>
        protected void NotifyChanges()
        {
            Action action = () =>
            {
                // Create event args
                List<ListChangedEventArgs> eventArgsList = new List<ListChangedEventArgs>();

                var suspensionContext = _suspensionContext;
                if (suspensionContext != null)
                {
                    if (suspensionContext.Mode == SuspensionMode.None)
                    {
                        return;
                    }

                    if (suspensionContext.NewItems.Count != 0)
                    {
                        eventArgsList.Add(new NotifyRangedListChangedEventArgs(NotifyRangedListChangedAction.Add, suspensionContext.NewItems, suspensionContext.NewItemIndices));
                    }

                    if (suspensionContext.OldItems.Count != 0)
                    {
                        eventArgsList.Add(new NotifyRangedListChangedEventArgs(NotifyRangedListChangedAction.Remove, suspensionContext.OldItems, suspensionContext.OldItemIndices));
                    }
                }

                // TODO: When reset must be called?
                if (eventArgsList.Count == 0)
                {
                    eventArgsList.Add(new NotifyRangedListChangedEventArgs(NotifyRangedListChangedAction.Reset));
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
                    e = new NotifyListChangedEventArgs(e.ListChangedType,
                        e.NewIndex,
                        e.NewIndex >= 0 ? this[e.NewIndex] : default(T),
                        e.OldIndex,
                        e.OldIndex >= 0 ? this[e.OldIndex] : default(T));
                }
                else
                {
                    e = new NotifyListChangedEventArgs(e.ListChangedType,
                        e.NewIndex,
                        e.NewIndex >= 0 ? this[e.NewIndex] : default(T),
                        e.PropertyDescriptor);
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

        #region Overrides of BindingList
        /// <summary>
        /// Apply sort.
        /// </summary>
        /// <param name="prop">The <see cref="PropertyDescriptor" /> that specifies the property to sort on.</param>
        /// <param name="direction">One of the <see cref="ListSortDirection"/> values.</param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            List<T> list = Items as List<T>;
            if (list == null)
            {
                return;
            }

            list.Sort((lhs, rhs) =>
            {
                object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
                object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);

                int result;
                if (lhsValue == null && rhsValue == null) // both values are null, both equal
                {
                    result = 0;
                }
                else if (lhsValue == null && rhsValue != null) // lhs value is null, rhs not, rhs value is greater
                {
                    result = -1;
                }
                else if (lhsValue != null && rhsValue == null) // rhs value is null, lhs not, lhs value is greater
                {
                    result = 1;
                }
                else if (lhsValue is IComparable) // lhs is IComparable, compare to rhs
                {
                    result = ((IComparable)lhsValue).CompareTo(rhsValue);
                }
                else if (lhsValue.Equals(rhsValue)) // check if both values are equal
                {
                    result = 0;
                }
                else // compare as string
                {
                    result = lhsValue.ToString().CompareTo(rhsValue.ToString());
                }

                if (_sortDirection == ListSortDirection.Descending)
                    result = -result;

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
            if (_suspensionContext != null && _suspensionContext.Mode != SuspensionMode.None)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Clearing items is only allowed in SuspensionMode.None, current mode is '{_suspensionContext.Mode}'.");
            }

            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.None)
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
            if (prop == null)
            {
                return -1;
            }

            List<T> list = Items as List<T>;
            if (list == null)
            {
                return -1;
            }

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];

                if (object.Equals(prop.GetValue(item), key))
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
            if (suspensionContext != null && suspensionContext.Mode == SuspensionMode.Removing)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
            }

            // Call base
            var oldValue = RaiseListChangedEvents;
            RaiseListChangedEvents = false;

            bool insertRequired = IsInsertRequired(item, index);
            try
            {
                if (suspensionContext == null || suspensionContext.Mode == SuspensionMode.None)
                {
                    base.InsertItem(index, item);

                    if (suspensionContext != null && suspensionContext.Mode != SuspensionMode.None)
                    {
                        OnListChanged(new NotifyListChangedEventArgs(ListChangedType.ItemAdded, index, item));
                    }
                }
            }
            finally
            {
                RaiseListChangedEvents = oldValue;
            }

            if (suspensionContext != null && insertRequired)
            {
                // Remember
                suspensionContext.NewItems.Add(item);
                suspensionContext.NewItemIndices.Add(index);
            }
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            // Check
            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Adding)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Removing items is not allowed in mode SuspensionMode.Adding.");
            }

            // Get item
            T item = this[index];

            // Call base
            var oldValue = RaiseListChangedEvents;
            RaiseListChangedEvents = false;

            bool removeRequired = IsRemoveRequired(item, index);
            try
            {
                if (_suspensionContext == null || _suspensionContext.Mode == SuspensionMode.None)
                {
                    base.RemoveItem(index);

                    if (_suspensionContext != null && _suspensionContext.Mode != SuspensionMode.None)
                    {
                        OnListChanged(new NotifyListChangedEventArgs(ListChangedType.ItemDeleted, index, item));
                    }
                }
            }
            finally
            {
                RaiseListChangedEvents = oldValue;
            }

            if (_suspensionContext != null && removeRequired )
            {
                // Remember
                _suspensionContext.OldItems.Add(item);
                _suspensionContext.OldItemIndices.Add(index);
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
            if (_suspensionContext != null && _suspensionContext.Mode != SuspensionMode.None)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Replacing items is only allowed in SuspensionMode.None, current mode is '{_suspensionContext.Mode}'");
            }

            // Get old item
            T oldItem = this[index];

            // Call base
            var oldValue = RaiseListChangedEvents;
            RaiseListChangedEvents = false;
            try
            {
                base.SetItem(index, item);

                if (!NotificationsSuspended)
                {
                    OnListChanged(new NotifyListChangedEventArgs(ListChangedType.ItemChanged, index, item, -1, oldItem));
                }
            }
            finally
            {
                RaiseListChangedEvents = oldValue;
            }
        }
        #endregion Overrides of BindingList
        #endregion

        /// <summary>
        /// Checks if insert is required.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="index">The item index</param>
        /// <returns><c>true</c> if its required; otherwise <c>false</c></returns>
        private bool IsInsertRequired(T item, int index = -1)
        {
            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Mixed)
            {
                return !_suspensionContext.TryRemoveItemFromOldItems(item, index);
            }

            return _suspensionContext != null;
        }

        /// <summary>
        /// Checks if insert is required.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="index">The item index</param>
        /// <returns><c>true</c> if its required; otherwise <c>false</c></returns>
        private bool IsRemoveRequired(T item, int index = -1)
        {
            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Mixed)
            {
                return !_suspensionContext.TryRemoveItemFromNewItems(item, index);
            }

            return _suspensionContext != null;
        }
    }
}

#endif
