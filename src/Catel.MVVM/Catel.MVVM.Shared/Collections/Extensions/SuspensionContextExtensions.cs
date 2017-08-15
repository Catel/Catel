// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionContextExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{ 
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// The suspension context extensions.
    /// </summary>
    internal static class SuspensionContextExtensions
    {
        #region Methods
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
                    eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs(changedItems, changedItemIndices, SuspensionMode.MixedBash, previousAction.Value));

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
                eventArgsList.Add(new NotifyRangedCollectionChangedEventArgs(changedItems, changedItemIndices, SuspensionMode.MixedBash, previousAction.Value));
            }

            return eventArgsList;
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
        #endregion Methods
    }
}