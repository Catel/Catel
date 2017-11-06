// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// Context class the hold all relevant data while notifications are suspended.
    /// </summary>
    /// <typeparam name="T">Type of the elements contained by the suspending collection.</typeparam>
    internal class SuspensionContext<T>
    {
        #region Fields
        /// <summary>
        /// The suspension count.
        /// </summary>
        private int _suspensionCount;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SuspensionContext{T}" /> class.
        /// </summary>
        /// <param name="mode">The suspension mode.</param>
        public SuspensionContext(SuspensionMode mode)
        {
            Mode = mode;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets events generators registry.
        /// </summary>
        public static Lazy<Dictionary<SuspensionMode, Func<SuspensionContext<T>, ICollection<NotifyRangedCollectionChangedEventArgs>>>> EventsGeneratorsRegistry { get; } = new Lazy<Dictionary<SuspensionMode, Func<SuspensionContext<T>, ICollection<NotifyRangedCollectionChangedEventArgs>>>>(InitializeRegistry);

        /// <summary>
        /// Gets the indices of the changed items while change notifications.
        /// </summary>
        public List<int> ChangedItemIndices { get; } = new List<int>();

        /// <summary>
        /// Gets the changed items while change notifications.
        /// </summary>
        public List<T> ChangedItems { get; } = new List<T>();

        /// <summary>
        /// Gets or sets the suspension count.
        /// </summary>
        public int Count
        {
            get => _suspensionCount;

            set
            {
                if (value != _suspensionCount)
                {
                    _suspensionCount = value < 0 ? 0 : value;
                }
            }
        }

        /// <summary>
        /// Gets the actions while change notifications were suspended in Mixed mode.
        /// </summary>
        public List<NotifyCollectionChangedAction> MixedActions { get; } = new List<NotifyCollectionChangedAction>();

        /// <summary>
        /// Gets the suspension mode.
        /// </summary>
        public SuspensionMode Mode { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Initialize the Events generators registry
        /// </summary>
        /// <returns></returns>
        private static Dictionary<SuspensionMode, Func<SuspensionContext<T>, ICollection<NotifyRangedCollectionChangedEventArgs>>> InitializeRegistry()
        {
            return new Dictionary<SuspensionMode, Func<SuspensionContext<T>, ICollection<NotifyRangedCollectionChangedEventArgs>>>
                       {
                           { SuspensionMode.None, context => context.CreateNoneEvents() },
                           { SuspensionMode.Adding, context => context.CreateAddingEvents() },
                           { SuspensionMode.Removing, context => context.CreateRemovingEvents() },
                           { SuspensionMode.Mixed, context => context.CreateMixedEvents() },
                           { SuspensionMode.MixedBash, context => context.CreateMixedBashEvents() },
                           { SuspensionMode.MixedConsolidate, context => context.CreateMixedConsolidateEvents() },
                           { SuspensionMode.Silent, context => context.CreateSilentEvents() }
                       };
        }
        #endregion Methods
    }
}