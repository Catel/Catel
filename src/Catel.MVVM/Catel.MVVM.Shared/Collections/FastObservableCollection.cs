﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Linq;
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
                foreach (var item in collection.Reverse())
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
                foreach (var item in collection.OfType<T>().Reverse())
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
                var lastIdx = Math.Max(0, Count - 1);
                foreach (var item in collection.OfType<T>().Reverse())
                {
                    if (IsInsertRequired(item))
                    {
                        list.Insert(lastIdx, item);
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
                foreach (var item in collection.OfType<T>())
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
                    
                    if (x.Instance._suspensionContext.Count == 1 && x.Instance._suspensionContext.Mode != SuspensionMode.None)
                    {
                        x.Instance.SynchronizeFromSuspensionContext();
                    }

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
                var eventArgsList = new List<NotifyCollectionChangedEventArgs>();

                if (_suspensionContext != null && _suspensionContext.Mode != SuspensionMode.None)
                {
                    if (_suspensionContext.NewItems.Count != 0)
                    {
                        eventArgsList.Add(CreateEventArgs(NotifyCollectionChangedAction.Add, _suspensionContext.NewItems, _suspensionContext.NewItemIndices));
                    }

                    if (_suspensionContext.OldItems.Count != 0)
                    {
                        eventArgsList.Add(CreateEventArgs(NotifyCollectionChangedAction.Remove, _suspensionContext.OldItems, _suspensionContext.OldItemIndices));
                    }
                }

                // TODO: When reset must be called?
                if (eventArgsList.Count == 0)
                {
                    eventArgsList.Add(CreateEventArgs(NotifyCollectionChangedAction.Reset));
                }

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                foreach (var eventArgs in eventArgsList)
                {
                    OnCollectionChanged(eventArgs);
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
            if (_suspensionContext == null || _suspensionContext.Count == 0)
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

            if (_suspensionContext.Count != 0)
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
            if (_suspensionContext == null || _suspensionContext.Count == 0)
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
            if (_suspensionContext != null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Mixed))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Clearing items is only allowed in SuspensionMode.None or SuspensionMode.Mixed, current mode is '{_suspensionContext.Mode}'");
            }

            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Mixed)
            {
                for (int i = 0; i < Count; i++)
                {
                    RemoveItem(i);
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
            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Removing)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Adding items is not allowed in mode SuspensionMode.Removing.");
            }

            var insertRequired = IsInsertRequired(item, index);

            if (_suspensionContext == null || _suspensionContext.Mode != SuspensionMode.Mixed && _suspensionContext.Mode != SuspensionMode.Adding)
            {
                // Call base
                base.InsertItem(index, item);
            }

            if (insertRequired)
            {
                _suspensionContext.NewItems.Add(item);
                _suspensionContext.NewItemIndices.Add(index);
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
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Moving items is only allowed in SuspensionMode.None or SuspensionMode.Mixed, current mode is '{_suspensionContext.Mode}'");
            }

            if (_suspensionContext != null && _suspensionContext.Mode == SuspensionMode.Mixed)
            {
                RemoveItem(oldIndex);
                InsertItem(newIndex, this[oldIndex]);
            }
            else
            {
                base.MoveItem(oldIndex, newIndex);
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
            var item = this[index];

            var removeRequired = IsRemoveRequired(item, index);

            if (_suspensionContext == null || _suspensionContext.Mode != SuspensionMode.Mixed && _suspensionContext.Mode != SuspensionMode.Mixed && _suspensionContext.Mode != SuspensionMode.Removing)
            {
                // Call base
                base.RemoveItem(index);
            }

            if (removeRequired)
            {
                _suspensionContext.OldItems.Add(item);
                _suspensionContext.OldItemIndices.Add(index);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param><param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, T item)
        {
            // Check
            if (_suspensionContext != null && (_suspensionContext.Mode != SuspensionMode.None && _suspensionContext.Mode != SuspensionMode.Mixed))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Replacing items is only allowed in SuspensionMode.None or SuspensionMode.Mixed, current mode is '{_suspensionContext.Mode}'");
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
        #endregion
    }
}