// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeArray.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Reflection
{
    using System;

    /// <summary>
    /// The type array class.
    /// </summary>
    public static class TypeArray
    {
        /// <summary>
        /// Gets an array of type from two type parameters.
        /// </summary>
        /// <typeparam name="T1">The type 1</typeparam>
        /// <typeparam name="T2">The type 2</typeparam>
        /// <returns>Array of types</returns>
        public static Type[] From<T1, T2>()
        {
            return ArrayCache<T1, T2>.Value;
        }

        /// <summary>
        /// Gets an array of type from two type parameters.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>Array of types</returns>
        public static Type[] From<T>()
        {
            return ArrayCache<T>.Value;
        }

        private static class ArrayCache<T1, T2>
        {
            public static readonly Type[] Value = { typeof(T1), typeof(T2) };
        }

        private static class ArrayCache<T1>
        {
            public static readonly Type[] Value = { typeof(T1) };
        }
    }
}