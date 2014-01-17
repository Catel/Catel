// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// <summary>
//   Collection helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System.Collections;

    /// <summary>
    /// Collection helper class.
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// Checks whether a collection is the same as another collection.
        /// </summary>
        /// <param name="listA">The list A.</param>
        /// <param name="listB">The list B.</param>
        /// <returns>
        /// True if the two collections contain all the same items in the same order.
        /// </returns>
        public static bool IsEqualTo(IEnumerable listA, IEnumerable listB)
        {
            if (listA == listB)
            {
                return true;
            }

            if ((listA == null) || (listB == null))
            {
                return false;
            }

            if (ReferenceEquals(listA, listB))
            {
                return true;
            }

            IEnumerator enumeratorA = listA.GetEnumerator();
            IEnumerator enumeratorB = listB.GetEnumerator();

            bool enumAHasValue = enumeratorA.MoveNext();
            bool enumBHasValue = enumeratorB.MoveNext();

            while (enumAHasValue && enumBHasValue)
            {
                object currentA = enumeratorA.Current;
                object currentB = enumeratorB.Current;

                if (currentA == currentB)
                {
                    enumAHasValue = enumeratorA.MoveNext();
                    enumBHasValue = enumeratorB.MoveNext();

                    continue;
                }

                if ((currentA == null) || (currentB == null))
                {
                    return false;
                }

                if (!currentA.Equals(currentB))
                {
                    return false;
                }

                enumAHasValue = enumeratorA.MoveNext();
                enumBHasValue = enumeratorB.MoveNext();
            }

            // If we get here, and both enumerables don't have any value left, they are equal
            return !(enumAHasValue || enumBHasValue);
        }
    }
}