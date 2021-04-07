// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyRangedListChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The ranged notify list changed event args.
    /// </summary>
    public class NotifyRangedListChangedEventArgs : NotifyListChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The real action that was performed on the <see cref="FastBindingList{T}"/>.</param>
        public NotifyRangedListChangedEventArgs(NotifyRangedListChangedAction action)
            : base(ListChangedType.Reset, -1)
        {
            Action = action;

            NewStartingIndex = -1;
            OldStartingIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The real action that was performed on the <see cref="FastBindingList{T}"/>.</param>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="indices">The indices.</param>
        public NotifyRangedListChangedEventArgs(NotifyRangedListChangedAction action, IList changedItems, IList<int> indices)
            : base(ListChangedType.Reset, -1)
        {
            Action = action;

            var startingIndex = indices is not null && indices.Count != 0 ? indices[0] : -1;
            if (action == NotifyRangedListChangedAction.Add)
            {
                NewItems = changedItems;
                NewStartingIndex = startingIndex;
                OldStartingIndex = -1;
            }
            else
            {
                OldItems = changedItems;
                OldStartingIndex = startingIndex;
                NewStartingIndex = -1;
            }

            Indices = indices;
        }

        /// <summary>
        /// Gets the real action that was performed on the <see cref="FastBindingList{T}"/>.
        /// </summary>
        public NotifyRangedListChangedAction Action { get; private set; }

        /// <summary>
        /// Gets the new items.
        /// </summary>
        public IList NewItems { get; private set; }

        /// <summary>
        /// Gets the new starting index.
        /// </summary>
        public int NewStartingIndex { get; private set; }

        /// <summary>
        /// Gets the old items.
        /// </summary>
        public IList OldItems { get; private set; }

        /// <summary>
        /// Gets the old starting index.
        /// </summary>
        public int OldStartingIndex { get; private set; }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        public IList<int> Indices { get; private set; }
    }
}

#endif
