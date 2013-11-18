// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Logging;

    /// <summary>
    /// Extensions for the <see cref="ICollection"/> and <see cref="Collection{T}"/> classes.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Determines whether the item can be moved up in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to check.</param>
        /// <returns><c>true</c> if the item can be moved up in the list; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        public static bool CanMoveItemUp(this IList list, object item)
        {
            Argument.IsNotNull("list", list);

            if (item == null)
            {
                return false;
            }

            if (list.Count <= 1)
            {
                return false;
            }

            if (list.IndexOf(item) <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves the specified item up in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to move up.</param>
        /// <returns><c>true</c> if the item has successfully been moved up; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="item"/> is <c>null</c>.</exception>
        public static bool MoveItemUp(this IList list, object item)
        {
            Argument.IsNotNull("list", list);
            Argument.IsNotNull("item", item);

            var currentIndex = list.IndexOf(item);
            if (currentIndex == -1)
            {
                Log.Warning("Object not found in list, cannot move up");
                return false;
            }

            return MoveItemUpByIndex(list, currentIndex);
        }

        /// <summary>
        /// Determines whether the item can be moved down in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to check.</param>
        /// <returns><c>true</c> if the item can be moved down in the list; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        public static bool CanMoveItemDown(this IList list, object item)
        {
            Argument.IsNotNull("list", list);

            if (item == null)
            {
                return false;
            }

            if (list.Count <= 1)
            {
                return false;
            }

            var index = list.IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            if (index == list.Count - 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves the item at the specified index up in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index of the item to move up.</param>
        /// <returns><c>true</c> if the item has successfully been moved up; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is smaller than 0 or larger than the list count.</exception>
        public static bool MoveItemUpByIndex(this IList list, int index)
        {
            Argument.IsNotNull("list", list);
            Argument.IsNotOutOfRange("index", index, 0, list.Count - 1);

            if (list.Count < index - 1)
            {
                Log.Error("Number of items in list is {0}, cannot move index {1} up", list.Count, index);
                return false;
            }

            if (index == 0)
            {
                Log.Debug("Index of item to move up is 0, no move up required");
                return true;
            }

            var item = list[index];
            list.RemoveAt(index);
            list.Insert(index - 1, item);

            return true;
        }

        /// <summary>
        /// Moves the specified item down in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="item">The item to move down.</param>
        /// <returns><c>true</c> if the item has successfully been moved down; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="item"/> is <c>null</c>.</exception>
        public static bool MoveItemDown(this IList list, object item)
        {
            Argument.IsNotNull("list", list);
            Argument.IsNotNull("item", item);

            var currentIndex = list.IndexOf(item);
            if (currentIndex == -1)
            {
                Log.Warning("Object not found in list, cannot move down");
                return false;
            }

            return MoveItemDownByIndex(list, currentIndex);
        }

        /// <summary>
        /// Moves the item at the specified index down in the list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index of the item to move down.</param>
        /// <returns><c>true</c> if the item has successfully been moved down; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is smaller than 0 or larger than the list count.</exception>
        public static bool MoveItemDownByIndex(this IList list, int index)
        {
            Argument.IsNotNull("list", list);
            Argument.IsNotOutOfRange("index", index, 0, list.Count - 1);

            if (list.Count < index - 1)
            {
                Log.Error("Number of items in list is {0}, cannot move index {1} down", list.Count, index);
                return false;
            }

            if (index == list.Count - 1)
            {
                Log.Debug("Index of item to move down equals the count, no move down required");
                return true;
            }

            var item = list[index];
            list.RemoveAt(index);
            list.Insert(index + 1, item);

            return true;
        }

        /// <summary>
        /// Add an range of items to the specified <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of items within the observable collection.</typeparam>
        /// <param name="collection">The <see cref="ObservableCollection{T}"/>.</param>
        /// <param name="range">An range of items.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="range"/> is <c>null</c>.</exception>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
        {
            Argument.IsNotNull("collection", collection);
            Argument.IsNotNull("range", range);

            foreach (T curItem in range)
            {
                collection.Add(curItem);
            }
        }

        /// <summary>
        /// Replaces the whole range of the specified <paramref name="collection"/>. This is done by internally
        /// calling <see cref="Collection{T}.Clear"/> and finally <see cref="AddRange{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of items within the observable collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="range">The range of items to add to the observable collection.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="range"/> is <c>null</c>.</exception>
        public static void ReplaceRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
        {
            Argument.IsNotNull("collection", collection);
            Argument.IsNotNull("range", range);

            collection.Clear();

            AddRange(collection, range);
        }

        /// <summary>
        /// Removes the first entry from the list.
        /// <para />
        /// When there are no items in the list, this method will silently exit.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        public static void RemoveFirst(this IList list)
        {
            Argument.IsNotNull("list", list);

            if (list.Count == 0)
            {
                return;
            }

            list.RemoveAt(0);
        }

        /// <summary>
        /// Removes the last entry from the list.
        /// <para />
        /// When there are no items in the list, this method will silently exit.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="list"/> is <c>null</c>.</exception>
        public static void RemoveLast(this IList list)
        {
            Argument.IsNotNull("list", list);

            if (list.Count == 0)
            {
                return;
            }

            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// the for each extension on <see cref="IEnumerable{TItem}"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<TItem>(this IEnumerable<TItem> collection, Action<TItem> action)
        {
            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                action(item);
            }
        }

#if NETFX_CORE
        /// <summary>
        /// Converts the dictionary to a readonly collection.
        /// </summary>
        /// <typeparam name="T">Type of the items inside the collection.</typeparam>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>The readonly version of the collection.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="collection"/> is <c>null</c>.</exception>
        public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> collection)
        {
            Argument.IsNotNull("collection", collection);

            return new ReadOnlyCollection<T>(collection);
        }
#endif
    }
}