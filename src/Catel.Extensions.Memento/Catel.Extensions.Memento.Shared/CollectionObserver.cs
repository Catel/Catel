// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionObserver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System.Collections;
    using System.Collections.Specialized;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// This class provides a simple <see cref="INotifyCollectionChanged"/> observer that will add undo/redo support to a 
    /// collection class automatically by monitoring the collection changed events.
    /// </summary>
    public class CollectionObserver : ObserverBase
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The collection.
        /// </summary>
        private INotifyCollectionChanged _collection;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionObserver"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="mementoService">The memento service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public CollectionObserver(INotifyCollectionChanged collection, object tag = null, IMementoService mementoService = null)
            : base(tag, mementoService)
        {
            Argument.IsNotNull("collection", collection);

            _collection = collection;
            _collection.CollectionChanged += OnCollectionChanged;
        }
        #endregion

        #region Methods
        /// <summary>
        /// This is invoked when the collection changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method must be public because the <see cref="IWeakEventListener"/> is used.
        /// </remarks>
        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Log.Debug("Automatically tracking change '{0}' of collection", e.Action);

            var undoList = new List<CollectionChangeUndo>();
            var collection = (IList)sender;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    undoList.AddRange(e.NewItems.Cast<object>().Select((item, i) => new CollectionChangeUndo(collection, CollectionChangeType.Add, -1, e.NewStartingIndex + i, null, item, Tag)));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    undoList.AddRange(e.OldItems.Cast<object>().Select((item, i) => new CollectionChangeUndo(collection, CollectionChangeType.Remove, e.OldStartingIndex + i, -1, item, null, Tag)));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    undoList.Add(new CollectionChangeUndo(collection, CollectionChangeType.Replace, e.OldStartingIndex, e.NewStartingIndex, e.OldItems[0], e.NewItems[0], Tag));
                    break;

#if NET
                case NotifyCollectionChangedAction.Move:
                    undoList.Add(new CollectionChangeUndo(collection, CollectionChangeType.Move, e.OldStartingIndex, e.NewStartingIndex, e.NewItems[0], e.NewItems[0], Tag));
                    break;
#endif
            }

            foreach (var operation in undoList)
            {
                MementoService.Add(operation);
            }

            Log.Debug("Automatically tracked change '{0}' of collection", e.Action);
        }

        /// <summary>
        /// Clears all the values and unsubscribes any existing change notifications.
        /// </summary>
        public override void CancelSubscription()
        {
            Log.Debug("Canceling collection change subscription");

            if (_collection != null)
            {
                _collection.CollectionChanged -= OnCollectionChanged;
                _collection = null;
            }

            Log.Debug("Canceled collection change subscription");
        }
        #endregion
    }
}
