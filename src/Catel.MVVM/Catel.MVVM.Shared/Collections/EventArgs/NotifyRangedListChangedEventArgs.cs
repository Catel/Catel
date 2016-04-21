// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyRangedListChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

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
        /// <param name="listChangedType">Type of change.</param>
        public NotifyRangedListChangedEventArgs(ListChangedType listChangedType)
            : base(listChangedType)
        {
            NewStartingIndex = -1;
            OldStartingIndex = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of change.</param>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="indices">The indices.</param>
        public NotifyRangedListChangedEventArgs(ListChangedType listChangedType, IList changedItems, IList<int> indices)
            : base(listChangedType, -1)
        {
            NewItems = changedItems;
            NewStartingIndex = (indices != null && indices.Count != 0) ? indices[0] : -1;
            OldStartingIndex = -1;

            Indices = indices;
        }

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
