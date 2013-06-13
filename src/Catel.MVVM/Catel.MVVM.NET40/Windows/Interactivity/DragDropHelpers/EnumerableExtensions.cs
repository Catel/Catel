// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity.DragDropHelpers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable lst)
        {
            if (lst == null) return true;
            return !lst.GetEnumerator().MoveNext();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> lst)
        {
            if (lst == null) return true;
            return !lst.Any();
        }
    }
}