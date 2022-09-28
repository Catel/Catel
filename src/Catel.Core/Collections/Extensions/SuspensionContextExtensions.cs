// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionContextExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// The suspension context extensions.
    /// </summary>
    public static class SuspensionContextExtensions
    {
        #region Methods
        /// <summary>
        /// The create event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            // No suspension context is the same as None mode
            var mode = suspensionContext?.Mode ?? SuspensionMode.None;

            // Fast return for no items in not None modes
            if (mode != SuspensionMode.None && suspensionContext?.ChangedItems.Count == 0)
            {
                return Array.Empty<NotifyRangedCollectionChangedEventArgs>();
            }

            if (!SuspensionContext<T>.EventsGeneratorsRegistry.Value.TryGetValue(mode, out var createEvents))
            {
                // ReSharper disable once LocalizableElement
                throw new ArgumentOutOfRangeException(nameof(mode), $"The suspension mode '{Enum<SuspensionMode>.ToString(mode)}' is unhandled.");
            }

            return createEvents(suspensionContext);
        }

        /// <summary>
        /// Creates the none event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateNoneEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsValid(nameof(suspensionContext), suspensionContext, context => context is null || context.Mode == SuspensionMode.None);

            return new List<NotifyRangedCollectionChangedEventArgs> { new NotifyRangedCollectionChangedEventArgs() };
        }

        /// <summary>
        /// Creates the adding event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateAddingEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.Adding);

            return new List<NotifyRangedCollectionChangedEventArgs> { new NotifyRangedCollectionChangedEventArgs(suspensionContext.ChangedItems, suspensionContext.ChangedItemIndices, suspensionContext.Mode) };
        }

        /// <summary>
        /// Creates the removing event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateRemovingEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.Removing);

            return new List<NotifyRangedCollectionChangedEventArgs> { new NotifyRangedCollectionChangedEventArgs(suspensionContext.ChangedItems, suspensionContext.ChangedItemIndices, suspensionContext.Mode) };
        }

        /// <summary>
        /// The create mixed event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateMixedEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.Mixed);

            return new List<NotifyRangedCollectionChangedEventArgs> { new NotifyRangedCollectionChangedEventArgs(suspensionContext.ChangedItems, suspensionContext.ChangedItemIndices, suspensionContext.MixedActions) };
        }

        /// <summary>
        /// The create mixed bash event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateMixedBashEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.MixedBash);

            return suspensionContext.CreateBashEvents(SuspensionMode.MixedBash);
        }

        /// <summary>
        /// The create mixed consolidate event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateMixedConsolidateEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.MixedConsolidate);

            var events = suspensionContext.CreateBashEvents(suspensionContext.Mode);

            bool restartRequired;
            do
            {
                restartRequired = false;

                for (var i = events.Count - 1; i >= 1; i--)
                {
                    var currentEvent = events[i];
                    if (currentEvent.Indices.Count > 0)
                    {
                        var previousEvent = events[i - 1];
                        restartRequired = currentEvent.Action == previousEvent.Action ? previousEvent.ConsolidateItemsByAppend(currentEvent) : previousEvent.ConsolidateItems(currentEvent);
                    }

                    if (currentEvent.Indices.Count == 0)
                    {
                        events.RemoveAt(i);
                        restartRequired = i < events.Count;
                    }
                }
            }
            while (restartRequired);

            return events;
        }

        /// <summary>
        /// Creates the silent event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateSilentEvents<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.Silent);

            return Array.Empty<NotifyRangedCollectionChangedEventArgs>();
        }

        /// <summary>
        /// The is mixed mode.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns><c>True</c> if <see cref="SuspensionMode"/> is one of the mixed modes; otherwise, <c>false</c>.</returns>
        public static bool IsMixedMode<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);

            return suspensionContext.Mode.IsMixedMode();
        }

        /// <summary>
        /// Creates the bash event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <param name="suspensionMode">The suspension mode.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="IList{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        private static IList<NotifyRangedCollectionChangedEventArgs> CreateBashEvents<T>(this SuspensionContext<T> suspensionContext, SuspensionMode suspensionMode)
        {
            var i = 0;
            var changedItems = new List<T>();
            var changedItemIndices = new List<int>();
            var previousAction = (NotifyCollectionChangedAction?)null;
            var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
            foreach (var action in suspensionContext.MixedActions)
            {
                // If action changed, create event args for remembered items
                if (previousAction.HasValue && action != previousAction.Value)
                {
                    // Create and add event args
                    eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs(changedItems, changedItemIndices, suspensionMode, previousAction.Value));

                    // Reset lists
                    changedItems = new List<T>();
                    changedItemIndices = new List<int>();
                }

                // Remember item and index
                changedItems.Add(suspensionContext.ChangedItems[i]);
                changedItemIndices.Add(suspensionContext.ChangedItemIndices[i]);

                // Update to current action
                previousAction = action;

                i++;
            }

            // Create event args for last item(s)
            if (changedItems.Count != 0 && previousAction is not null)
            {
                eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs(changedItems, changedItemIndices, suspensionMode, previousAction.Value));
            }

            return eventArgsList;
        }
        #endregion Methods
    }
}
