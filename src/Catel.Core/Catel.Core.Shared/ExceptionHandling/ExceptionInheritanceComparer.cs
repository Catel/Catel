// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionInheritanceComparer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !SL5

namespace Catel.ExceptionHandling
{
    using System;
    using System.Collections.Generic;
    using Reflection;

    internal class ExceptionInheritanceComparer : IComparer<Type>
    {
        /// <summary>
        /// Compares two exception type, and returns a value indicating whether one of the types is smaller, equal or larger than the other type.
        /// </summary>
        /// <returns>
        /// A signed integer that of the relative values <paramref name="x" /> and <paramref name="y" /> indicates how in the following table veranschaulicht.Wert Meaning Less than 0<paramref name="x" /> is smaller than <paramref name="y" />.Zero<paramref name="x" /> is equal to <paramref name="y" />.greater than 0<paramref name="x" /> is bigger than <paramref name="y" />.
        /// </returns>
        /// <param name="x">The first exception type to be compared.</param>
        /// <param name="y">The second exception type to be compared.</param>
        /// <exception cref="ArgumentNullException">The <paramref ref="x"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref ref="y"/> is <c>null</c>.</exception>
        public int Compare(Type x, Type y)
        {
            Argument.IsNotNull("x", x);
            Argument.IsNotNull("y", y);

            if (x == y)
            {
                return 0;
            }

            if (x.IsAssignableFromEx(y))
            {
                return 1;
            }
          
            return -1;
        }
    }
}

#endif