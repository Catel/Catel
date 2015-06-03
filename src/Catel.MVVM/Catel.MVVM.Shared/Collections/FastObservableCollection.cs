// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
    public class FastObservableCollection<T> : ObservableCollection<T>
    {
        #region Constants
        private static readonly IDispatcherService _dispatcherService;
        #endregion

        #region Fields
        private bool _suspendChangeNotifications;
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

            using (SuspendChangeNotifications())
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

            using (SuspendChangeNotifications())
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

            using (SuspendChangeNotifications())
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

            using (SuspendChangeNotifications())
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

            using (SuspendChangeNotifications())
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

            using (SuspendChangeNotifications())
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
            return new DisposableToken<FastObservableCollection<T>>(this,
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
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            };

            if (AutomaticallyDispatchChangeNotifications)
            {
                _dispatcherService.InvokeIfRequired(action);
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
                    _dispatcherService.InvokeIfRequired(() => base.OnCollectionChanged(e));
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
        #endregion
    }
}