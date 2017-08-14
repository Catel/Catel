// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Context class the hold all relevant data while notifications are suspended.
    /// </summary>
    /// <typeparam name="T">Type of the elements contained by the suspending collection.</typeparam>
    public class SuspensionContext<T>
    {
        #region Fields
        private readonly List<int> _newItemIndices = new List<int>();

        private readonly List<T> _newItems = new List<T>();

        private readonly List<int> _oldItemIndices = new List<int>();

        private readonly List<T> _oldItems = new List<T>();

        private int _suspensionCount;

        private readonly SuspensionMode _suspensionMode;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionContext{T}" /> class.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        public SuspensionContext(SuspensionMode mode)
        {
            _suspensionMode = mode;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the suspension count.
        /// </summary>
        public int Count
        {
            get
            {
                return _suspensionCount;
            }

            set
            {
                if (value != _suspensionCount)
                {
                    if (value < 0)
                    {
                        _suspensionCount = 0;
                    }
                    else
                    {
                        _suspensionCount = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the suspension mode.
        /// </summary>
        public SuspensionMode Mode
        {
            get
            {
                return _suspensionMode;
            }
        }

        /// <summary>
        /// Gets the indices od the added items while change notifications were suspended.
        /// </summary>
        public List<int> NewItemIndices
        {
            get
            {
                return _newItemIndices;
            }
        }

        /// <summary>
        /// Gets the added items while change notifications were suspended.
        /// </summary>
        public List<T> NewItems
        {
            get
            {
                return _newItems;
            }
        }

        /// <summary>
        /// Gets the indices od the removed items while change notifications were suspended.
        /// </summary>
        public List<int> OldItemIndices
        {
            get
            {
                return _oldItemIndices;
            }
        }

        /// <summary>
        /// Gets the removed items while change notifications were suspended.
        /// </summary>
        public List<T> OldItems
        {
            get
            {
                return _oldItems;
            }
        }
        #endregion

        #region Methods

        #endregion

        /// <summary>
        /// Tries to remove the item from old items
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index"></param>
        /// <returns><c>true</c> if removed, otherwise <c>false</c>.</returns>
        public bool TryRemoveItemFromOldItems(T item, int index = -1)
        {
            return TryRemoveItems(item, index, _oldItems, _oldItemIndices);
        }

        /// <summary>
        /// Tries to remove the item from new items
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The item index</param>
        /// <returns><c>true</c> if removed, otherwise <c>false</c>.</returns>
        public bool TryRemoveItemFromNewItems(T item, int index = -1)
        {
            return TryRemoveItems(item, index, _newItems, _newItemIndices);
        }

        /// <summary>
        /// Synchronize
        /// </summary>
        /// <param name="insertSyncAction">The insert synchronization action</param>
        /// <param name="removeSyncAction">The remove synchronization action</param>
        public void Synchronize(Action<int, T> insertSyncAction, Action<int> removeSyncAction)
        {
            SynchronizeInserts(insertSyncAction);
            SynchronizeRemoves(removeSyncAction);
        }

        /// <summary>
        /// Tries to remove the item from the given <paramref name="items"/>
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The index</param>
        /// <param name="items">The items</param>
        /// <param name="itemIndexes">The item indexes</param>
        /// <returns><c>true</c> if removed, otherwise <c>false</c>.</returns>
        private static bool TryRemoveItems(T item, int index, List<T> items, List<int> itemIndexes)
        {
            if (itemIndexes.Count == 0)
            {
                return false;
            }

            if (index != -1)
            {
                // TODO: Improve the performace of this operations.

                var itemIdx = itemIndexes.LastIndexOf(index);
                if (itemIdx == -1 || !Equals(item, items[itemIdx]))
                {
                    return false;
                }

                items.RemoveAt(itemIdx);
                itemIndexes.RemoveAt(itemIdx);

                return true;
            }

            var newIdx = items.LastIndexOf(item);
            if (newIdx > -1)
            {
                items.RemoveAt(newIdx);
                itemIndexes.RemoveAt(newIdx);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Synchronize from old items
        /// </summary>
        /// <param name="insertSyncAction">The remove sync action</param>
        private void SynchronizeInserts(Action<int, T> insertSyncAction)
        {
            if (insertSyncAction != null && NewItems.Count > 0)
            {
                for (int i = 0; i < NewItems.Count; i++)
                {
                    insertSyncAction(NewItemIndices[i], NewItems[i]);
                }
            }
        }

        /// <summary>
        /// Synchronize from old items
        /// </summary>
        /// <param name="removeSyncAction">The remove sync action</param>
        private void SynchronizeRemoves(Action<int> removeSyncAction)
        {
            if (removeSyncAction != null && OldItems.Count > 0)
            {
                SortedDictionary<int, T> sortedDictionary = new SortedDictionary<int, T>(Comparer<int>.Create((x, y) => y.CompareTo(x)));
                for (int i = 0; i < OldItems.Count; i++)
                {
                    sortedDictionary.Add(OldItemIndices[i], OldItems[i]);
                }

                int idx = 0;
                foreach (KeyValuePair<int, T> pair in sortedDictionary)
                {
                    OldItemIndices[idx] = pair.Key;
                    OldItems[idx++] = pair.Value;

                    removeSyncAction(pair.Key);
                }
            }
        }
    }
}