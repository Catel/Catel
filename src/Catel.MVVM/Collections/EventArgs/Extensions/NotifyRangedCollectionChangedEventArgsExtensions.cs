// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifyRangedCollectionChangedEventArgsExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="NotifyRangedCollectionChangedEventArgs" /> extensions methods.
    /// </summary>
    internal static class NotifyRangedCollectionChangedEventArgsExtensions
    {
        /// <summary>
        /// Consolidate changed items and indexes from <paramref name="other" /> into <paramref name="instance" /> by append.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <param name="other">The other instance.</param>
        public static bool ConsolidateItemsByAppend(this NotifyRangedCollectionChangedEventArgs instance, NotifyRangedCollectionChangedEventArgs other)
        {
            if (other.Indices.Count == 0)
            {
                return false;
            }

            instance.Indices.AddRange(other.Indices);
            foreach (var item in other.ChangedItems)
            {
                instance.ChangedItems.Add(item);
            }

            other.Indices.Clear();
            other.ChangedItems.Clear();

            return true;
        }

        /// <summary>
        /// Consolidates changed items and indexes from <paramref name="other" /> into <paramref name="instance" />.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <param name="other">The other instance.</param>
        /// <returns><c>True</c> if consolidation was executed; otherwise <c>False</c></returns>
        public static bool ConsolidateItems(this NotifyRangedCollectionChangedEventArgs instance, NotifyRangedCollectionChangedEventArgs other)
        {
            if (instance.Indices.Count == 0 || other.Indices.Count == 0)
            {
                return false;
            }

            if (instance.Indices.Count > other.Indices.Count)
            {
                var backup = instance;
                instance = other;
                other = backup;
            }

            var consolidated = false;
            for (var i = instance.Indices.Count - 1; i >= 0; i--)
            {
                var idx = other.Indices.IndexOf(instance.Indices[i]);
                while (idx != -1 && !Equals(other.ChangedItems[idx], instance.ChangedItems[i]))
                {
                    idx = other.Indices.IndexOf(instance.Indices[i], idx + 1);
                }

                if (idx != -1)
                {
                    other.Indices.RemoveAt(idx);
                    other.ChangedItems.RemoveAt(idx);

                    instance.Indices.RemoveAt(i);
                    instance.ChangedItems.RemoveAt(i);

                    consolidated = true;
                }
            }

            return consolidated;
        }
    }
}