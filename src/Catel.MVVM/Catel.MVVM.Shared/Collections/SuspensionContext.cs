// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System.Collections.Generic;

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
        /// <param name="index">
        /// The item index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// <c>true</c> if removed, otherwise <c>false</c>.
        /// </returns>
        public bool TryRemoveItemFromOldItems(int index, T item)
        {
            if (Mode == SuspensionMode.None || Mode == SuspensionMode.Mixed)
            {
                var oldIdx = OldItems.LastIndexOf(item);
                if (oldIdx > -1 && OldItemIndices[oldIdx] == index)
                {
                    OldItems.RemoveAt(oldIdx);
                    OldItemIndices.RemoveAt(oldIdx);

                    for (var i = 0; i < OldItemIndices.Count; i++)
                    {
                        if (OldItemIndices[i] >= index)
                        {
                            OldItemIndices[i]++;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to remove the item from new items
        /// </summary>
        /// <param name="index">
        /// The item index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// <c>true</c> if removed, otherwise <c>false</c>.
        /// </returns>
        public bool? TryRemoveItemFromNewItems(int index, T item)
        {
            if (Mode == SuspensionMode.None || Mode == SuspensionMode.Mixed)
            {
                var newIdx = NewItems.LastIndexOf(item);
                if (newIdx > -1 && NewItemIndices[newIdx] == index)
                {
                    NewItems.RemoveAt(newIdx);
                    NewItemIndices.RemoveAt(newIdx);
                    for (var i = 0; i < NewItemIndices.Count; i++)
                    {
                        if (NewItemIndices[i] >= index)
                        {
                            NewItemIndices[i]--;
                        }
                    }

                    return true;
                }
            }

            return false;
        }
    }
}