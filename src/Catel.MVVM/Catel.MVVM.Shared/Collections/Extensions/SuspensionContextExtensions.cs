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
    using System.Linq;

    /// <summary>
    /// The suspension context extensions.
    /// </summary>
    internal static class SuspensionContextExtensions
    {
        #region Methods
        /// <summary>
        /// The create event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateEventArgsList<T>(this SuspensionContext<T> suspensionContext)
        {
            // No suspension context is the same as None mode
            var mode = suspensionContext?.Mode ?? SuspensionMode.None;

            // Fast return for no items in not None modes
            // ReSharper disable once PossibleNullReferenceException
            if (mode != SuspensionMode.None && suspensionContext.ChangedItems.Count == 0)
            {
                return ArrayShim.Empty<NotifyRangedCollectionChangedEventArgs>();
            }

            // Determine creation depending on mode
            ICollection<NotifyRangedCollectionChangedEventArgs> eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
            switch (mode)
            {
                case SuspensionMode.None:
                    {
                        eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs());
                        break;
                    }

                case SuspensionMode.Adding:
                case SuspensionMode.Removing:
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs(suspensionContext.ChangedItems, suspensionContext.ChangedItemIndices, mode));
                        break;
                    }

                case SuspensionMode.Mixed:
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs(suspensionContext.ChangedItems, suspensionContext.ChangedItemIndices, suspensionContext.MixedActions));
                        break;
                    }

                case SuspensionMode.MixedBash:
                    {
                        eventArgsList = suspensionContext.CreateMixedBashEventArgsList();
                        break;
                    }

                case SuspensionMode.MixedConsolidate:
                    {
                        eventArgsList = suspensionContext.CreateMixedConsolidateEventArgsList();
                        break;
                    }

                default:
                    {
                        // ReSharper disable once LocalizableElement
                        throw new ArgumentOutOfRangeException(nameof(mode), $"The suspension mode '{mode}' is unhandled.");
                    }
            }

            return eventArgsList;
        }

        /// <summary>
        /// The create mixed bash event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateMixedBashEventArgsList<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.MixedBash);

            return suspensionContext.CreateBashEventArgsList(SuspensionMode.MixedBash);
        }

        /// <summary>
        /// The create mixed consolidate event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="ICollection{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        public static ICollection<NotifyRangedCollectionChangedEventArgs> CreateMixedConsolidateEventArgsList<T>(this SuspensionContext<T> suspensionContext)
        {
            Argument.IsNotNull(nameof(suspensionContext), suspensionContext);
            Argument.IsValid(nameof(suspensionContext.Mode), suspensionContext.Mode, mode => mode == SuspensionMode.MixedConsolidate);

            var events = suspensionContext.CreateBashEventArgsList(SuspensionMode.MixedConsolidate);

            bool consolidated;
            do
            {
                consolidated = false;
                for (int i = events.Count - 1; i >= 1; i--)
                {
                    var currentEvent = events[i];
                    var previousEvent = events[i - 1];
                    if (currentEvent.Action != previousEvent.Action)
                    {
                        for (int j = currentEvent.Indices.Count - 1; j >= 0; j--)
                        {
                            var index = currentEvent.Indices[j];
                            var item = currentEvent.ChangedItems[j];

                            var indexOnPreviousEvent = previousEvent.Indices.IndexOf(index);
                            if (indexOnPreviousEvent != -1 && Equals(previousEvent.ChangedItems[indexOnPreviousEvent], item))
                            {
                                previousEvent.Indices.RemoveAt(indexOnPreviousEvent);
                                previousEvent.ChangedItems.RemoveAt(indexOnPreviousEvent);
                                previousEvent = events[i - 1] = new NotifyRangedCollectionChangedEventArgs(previousEvent.ChangedItems, previousEvent.Indices, SuspensionMode.MixedConsolidate, previousEvent.Action);

                                currentEvent.Indices.RemoveAt(j);
                                currentEvent.ChangedItems.RemoveAt(j);

                                consolidated = true;
                            }
                        }

                        if (consolidated)
                        {
                            if (currentEvent.Indices.Count == 0)
                            {
                                events.RemoveAt(i);
                            }
                            else
                            {
                                events[i] = new NotifyRangedCollectionChangedEventArgs(currentEvent.ChangedItems, currentEvent.Indices, SuspensionMode.MixedConsolidate, currentEvent.Action);
                            }
                        }
                    }
                    else
                    {
                        var changedIndexes = previousEvent.Indices;
                        changedIndexes.AddRange(currentEvent.Indices);

                        var changedItems = previousEvent.ChangedItems.Cast<T>().ToList();
                        changedItems.AddRange(currentEvent.ChangedItems.Cast<T>());

                        events[i - 1] = new NotifyRangedCollectionChangedEventArgs(changedItems, changedIndexes, SuspensionMode.MixedConsolidate, previousEvent.Action);
                        events.RemoveAt(i);

                        consolidated = true;
                    }
                }
            }
            while (consolidated);

            return events;
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
        /// The create bash event args list.
        /// </summary>
        /// <param name="suspensionContext">The suspension context.</param>
        /// <param name="suspensionMode">The suspension mode.</param>
        /// <typeparam name="T">The type of collection item.</typeparam>
        /// <returns>The <see cref="IList{NotifyRangedCollectionChangedEventArgs}"/>.</returns>
        private static IList<NotifyRangedCollectionChangedEventArgs> CreateBashEventArgsList<T>(this SuspensionContext<T> suspensionContext, SuspensionMode suspensionMode)
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
            if (changedItems.Count != 0)
            {
                // ReSharper disable once PossibleInvalidOperationException
                eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs(changedItems, changedItemIndices, suspensionMode, previousAction.Value));
            }

            return eventArgsList;
        }
        #endregion Methods
    }
}