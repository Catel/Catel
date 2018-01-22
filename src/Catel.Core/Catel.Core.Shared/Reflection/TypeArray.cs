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
        /// <typeparam name="T3">The type 3</typeparam>
        /// <typeparam name="T4">The type 4</typeparam>
        /// <typeparam name="T5">The type 5</typeparam>		
        /// <returns>Array of types</returns>
        public static Type[] From<T1, T2, T3, T4, T5>()
        {
            return ArrayCache<T1, T2, T3, T4, T5>.Value;
        }

        /// <summary>
        /// Gets an array of type from two type parameters.
        /// </summary>
        /// <typeparam name="T1">The type 1</typeparam>
        /// <typeparam name="T2">The type 2</typeparam>
        /// <typeparam name="T3">The type 3</typeparam>
        /// <typeparam name="T4">The type 4</typeparam>	
        /// <returns>Array of types</returns>
        public static Type[] From<T1, T2, T3, T4>()
        {
            return ArrayCache<T1, T2, T3, T4>.Value;
        }

        /// <summary>
        /// Gets an array of type from two type parameters.
        /// </summary>
        /// <typeparam name="T1">The type 1</typeparam>
        /// <typeparam name="T2">The type 2</typeparam>
        /// <typeparam name="T3">The type 3</typeparam>
        /// <returns>Array of types</returns>
        public static Type[] From<T1, T2, T3>()
        {
            return ArrayCache<T1, T2, T3>.Value;
        }

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

        private static class ArrayCache<T1, T2, T3, T4, T5>
        {
            internal static readonly Type[] Value = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
        }

        private static class ArrayCache<T1, T2, T3, T4>
        {
            internal static readonly Type[] Value = { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
        }

        private static class ArrayCache<T1, T2, T3>
        {
            internal static readonly Type[] Value = { typeof(T1), typeof(T2), typeof(T3) };
        }

        private static class ArrayCache<T1, T2>
        {
            internal static readonly Type[] Value = { typeof(T1), typeof(T2) };
        }

        private static class ArrayCache<T1>
        {
            internal static readonly Type[] Value = { typeof(T1) };
        }
    }
}