namespace Catel.Collections
{
    using System;
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
        /// <remarks>This is only for use of <see cref="Catel.Collections.SuspensionMode.None"/>.</remarks>
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
        /// <remarks>This only for use of <see cref="Catel.Collections.SuspensionMode.Adding"/> and <see cref="Catel.Collections.SuspensionMode.Removing"/>.</remarks>
        public NotifyRangedCollectionChangedEventArgs(IList changedItems, IList<int> indices, SuspensionMode mode)
            : base(ModeToAction(mode), changedItems, indices.Count != 0 ? indices[0] : -1)
        {
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
        /// <remarks>This is only for use of <see cref="Catel.Collections.SuspensionMode.MixedBash"/>.</remarks>
        public NotifyRangedCollectionChangedEventArgs(IList changedItems, IList<int> indices, SuspensionMode mode, NotifyCollectionChangedAction action)
            : base(EnsureModeAndAction(mode, action), changedItems, indices.Count != 0 ? indices[0] : -1)
        {
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
        /// <remarks>This is only for use of <see cref="Catel.Collections.SuspensionMode.Mixed"/>.</remarks>
        public NotifyRangedCollectionChangedEventArgs(IList changedItems, IList<int> indices, IList<NotifyCollectionChangedAction> mixedActions)
            : base(NotifyCollectionChangedAction.Reset)
        {
            SuspensionMode = SuspensionMode.Mixed;
            ChangedItems = changedItems;
            Indices = indices;
            MixedActions = mixedActions;
        }

        /// <summary>
        /// Gets the indices.
        /// </summary>
        public IList<int>? Indices { get; }

        /// <summary>
        /// Gets the changed items.
        /// </summary>
        public IList? ChangedItems { get; }

        /// <summary>
        /// Gets the mixed actions.
        /// </summary>
        public IList<NotifyCollectionChangedAction>? MixedActions { get; }

        /// <summary>
        /// Gets the suspension mode.
        /// </summary>
        public SuspensionMode SuspensionMode { get; }

        /// <summary>
        /// The ensure mode and action.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="NotifyCollectionChangedAction"/>.</returns>
        private static NotifyCollectionChangedAction EnsureModeAndAction(SuspensionMode mode, NotifyCollectionChangedAction action)
        {
            // Check for mixed modes except for Mixed, others fail
            if (mode == SuspensionMode.Mixed || !mode.IsMixedMode())
            {
                throw new ArgumentException($"Wrong mode '{Enum<SuspensionMode>.ToString(mode)}' for constructor.");
            }

            // Check for action Add or Remove, others fail
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove: 
                    return action;
                
                default: 
                    throw new ArgumentException($"Wrong action '{Enum<NotifyCollectionChangedAction>.ToString(action)}' for constructor.");
            }
        }

        /// <summary>
        /// Transforms the <see cref="SuspensionMode"/> into its equivalent <see cref="NotifyCollectionChangedAction"/>.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        /// <returns>The equivalent <see cref="NotifyCollectionChangedAction"/>.</returns>
        private static NotifyCollectionChangedAction ModeToAction(SuspensionMode mode)
        {
            // Only transform modes Adding and Removing, others fail
            switch (mode)
            {
                case SuspensionMode.Adding: 
                    return NotifyCollectionChangedAction.Add;

                case SuspensionMode.Removing: 
                    return NotifyCollectionChangedAction.Remove;

                default: 
                    throw new ArgumentException($"Wrong mode '{Enum<SuspensionMode>.ToString(mode)}' for constructor.");
            }
        }
    }
}
