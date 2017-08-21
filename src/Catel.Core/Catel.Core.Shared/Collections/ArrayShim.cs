// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayShim.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET45 || XAMARIN || PCL
#define ARRAY_EMPTY_MISSING
#endif

namespace Catel.Collections
{
    using System;

    /// <summary>
    /// Shim class for Array to provide support on all platforms.
    /// </summary>
    public static class ArrayShim
    {
#if ARRAY_EMPTY_MISSING
        internal static class EmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
#endif

        /// <summary>
        /// Returns an empty array.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array.</typeparam>
        /// <returns>Returns an empty <see cref="Array"/>.</returns>
        public static T[] Empty<T>()
        {
#if ARRAY_EMPTY_MISSING
            return EmptyArray<T>.Value;
#else
            return Array.Empty<T>();
#endif
        }
    }
}