// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="IEnumerable{T}"/> extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region Methods
        /// <summary>
        /// Fors the each.
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
        #endregion
    }
}