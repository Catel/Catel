// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyRangedCollectionChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Catel.Collections.Extensions;

    /// <summary>
    /// The ranged notify collection changed event args.
    /// </summary>
    public class NotifyRangedCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        public NotifyRangedCollectionChangedEventArgs()
            : base(NotifyCollectionChangedAction.Reset)
        {
            SuspensionMode = SuspensionMode.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="mode">The suspension mode.</param>
        public NotifyRangedCollectionChangedEventArgs(IList changedItems, IList<int> indices, SuspensionMode mode)
            : base(ModeToAction(mode), changedItems, (indices != null && indices.Count != 0) ? indices[0] : -1)
        {
            Argument.IsNotNull(nameof(changedItems), changedItems);
            Argument.IsNotNull(nameof(indices), indices);
            // ReSharper disable once PossibleNullReferenceException
            Argument.IsNotOutOfRange(nameof(indices), indices.Count, changedItems.Count, changedItems.Count);

            ChangedItems = changedItems;
            Indices = indices;
            SuspensionMode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="mode">The suspension mode.</param>
        /// <param name="action">The action.</param>
        public NotifyRangedCollectionChangedEventArgs(IList changedItems, IList<int> indices, SuspensionMode mode, NotifyCollectionChangedAction action)
            : base(EnsureModeAndAction(mode, action), changedItems, (indices != null && indices.Count != 0) ? indices[0] : -1)
        {
            Argument.IsNotNull(nameof(changedItems), changedItems);
            Argument.IsNotNull(nameof(indices), indices);
            // ReSharper disable once PossibleNullReferenceException
            Argument.IsNotOutOfRange(nameof(indices), indices.Count, changedItems.Count, changedItems.Count);

            ChangedItems = changedItems;
            Indices = indices;
            SuspensionMode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyRangedCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="mixedActions">The mixed actions.</param>
        public NotifyRangedCollectionChangedEventArgs(IList changedItems, IList<int> indices, IList<NotifyCollectionChangedAction> mixedActions)
            : base(NotifyCollectionChangedAction.Reset)
        {
            Argument.IsNotNull(nameof(changedItems), changedItems);
            Argument.IsNotNull(nameof(indices), indices);
            Argument.IsNotNull(nameof(mixedActions), mixedActions);

            SuspensionMode = SuspensionMode.Mixed;
            ChangedItems = changedItems;
            Indices = indices;
            MixedActions = mixedActions;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the indices.
        /// </summary>
        public IList<int> Indices { get; }

        /// <summary>
        /// Gets the changed items.
        /// </summary>
        public IList ChangedItems { get; }

        /// <summary>
        /// Gets the mixed actions.
        /// </summary>
        public IList<NotifyCollectionChangedAction> MixedActions { get; }

        /// <summary>
        /// Gets the suspension mode.
        /// </summary>
        public SuspensionMode SuspensionMode { get; }
        #endregion

        #region Methods
        /// <summary>
        /// The ensure mode and action.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="NotifyCollectionChangedAction"/>.</returns>
        private static NotifyCollectionChangedAction EnsureModeAndAction(SuspensionMode mode, NotifyCollectionChangedAction action)
        {
            if (mode == SuspensionMode.Mixed || !mode.IsMixedMode())
            {
                throw new ArgumentException($"Wrong mode '{mode}' for constructor.");
            }

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove: return action;
                default: throw new ArgumentException($"Wrong action '{action}' for constructor.");
            }
        }

        /// <summary>
        /// Transforms the <see cref="SuspensionMode"/> into its equivalent <see cref="NotifyCollectionChangedAction"/>.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        /// <returns>The equivalent <see cref="NotifyCollectionChangedAction"/>.</returns>
        private static NotifyCollectionChangedAction ModeToAction(SuspensionMode mode)
        {
            switch (mode)
            {
                case SuspensionMode.Adding: return NotifyCollectionChangedAction.Add;
                case SuspensionMode.Removing: return NotifyCollectionChangedAction.Remove;
                default: throw new ArgumentException($"Wrong mode '{mode}' for constructor.");
            }
        }
        #endregion
    }
}