// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyRangedCollectionChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !SL5

namespace Catel.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// The ranged notify collection changed event args.
    /// </summary>
    public class NotifyRangedCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public NotifyRangedCollectionChangedEventArgs(NotifyCollectionChangedAction action)
            : base(action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="indices">The indices.</param>
        public NotifyRangedCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, IList<int> indices)
            : base(action, changedItems, (indices != null && indices.Count != 0) ? indices[0] : -1)
        {
            Argument.IsNotNull("indices", indices);
            // ReSharper disable once PossibleNullReferenceException
            Argument.IsNotOutOfRange("indices", indices.Count, changedItems.Count, changedItems.Count);

            Indices = indices;
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        public IList<int> Indices { get; private set; }
    }
}

#endif