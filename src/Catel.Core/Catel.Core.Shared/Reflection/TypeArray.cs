// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeArray.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The type array class.
    /// </summary>
    public static class TypeArray
    {
        /// <summary>
        /// Gets an array of type from five type parameters.
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
        /// Gets an array of type from five type parameters.
        /// </summary>
        /// <param name="type1">The type1</param>
        /// <param name="type2">The type2</param>
        /// <param name="type3">The type3</param>
        /// <param name="type4">The type4</param>
        /// <param name="type5">The type5</param>
        /// <returns>Array of types</returns>
        public static Type[] From(Type type1, Type type2, Type type3, Type type4, Type type5)
        {
            var genericType = typeof(ArrayCache<,,,,>).MakeGenericType(type1, type2, type3, type4, type5);
            var field = genericType.GetFieldEx("Value", BindingFlags.Static | BindingFlags.Public);
            return (Type[])field.GetValue(genericType);
        }


        /// <summary>
        /// Gets an array of type from four type parameters.
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
        /// Gets an array of type from four type parameters.
        /// </summary>
        /// <param name="type1">The type1</param>
        /// <param name="type2">The type2</param>
        /// <param name="type3">The type3</param>
        /// <param name="type4">The type4</param>
        /// <returns>Array of types</returns>
        public static Type[] From(Type type1, Type type2, Type type3, Type type4)
        {
            var genericType = typeof(ArrayCache<,,,>).MakeGenericType(type1, type2, type3, type4);
            var field = genericType.GetFieldEx("Value", BindingFlags.Static | BindingFlags.Public);
            return (Type[])field.GetValue(genericType);
        }


        /// <summary>
        /// Gets an array of type from three type parameters.
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
        /// Gets an array of type from three type parameters.
        /// </summary>
        /// <param name="type1">The type1</param>
        /// <param name="type2">The type2</param>
        /// <param name="type3">The type3</param>
        /// <returns>Array of types</returns>
        public static Type[] From(Type type1, Type type2, Type type3)
        {
            var genericType = typeof(ArrayCache<,,>).MakeGenericType(type1, type2, type3);
            var field = genericType.GetFieldEx("Value", BindingFlags.Static | BindingFlags.Public);
            return (Type[])field.GetValue(genericType);
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
        /// <param name="type1">The type1</param>
        /// <param name="type2">The type2</param>
        /// <returns>Array of types</returns>
        public static Type[] From(Type type1, Type type2)
        {
            var genericType = typeof(ArrayCache<,>).MakeGenericType(type1, type2);
            var field = genericType.GetFieldEx("Value", BindingFlags.Static | BindingFlags.Public);
            return (Type[])field.GetValue(genericType);
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

        /// <summary>
        /// Gets an array of type from two type parameters.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>Array of types</returns>
        public static Type[] From(Type type)
        {
            var genericType = typeof(ArrayCache<>).MakeGenericType(type);
            var field = genericType.GetFieldEx("Value", BindingFlags.Static | BindingFlags.Public);
            return (Type[])field.GetValue(genericType);
        }

        private static class ArrayCache<T1, T2, T3, T4, T5>
        {
            public static readonly Type[] Value = { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
        }

        private static class ArrayCache<T1, T2, T3, T4>
        {
            public static readonly Type[] Value = { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
        }

        private static class ArrayCache<T1, T2, T3>
        {
            public static readonly Type[] Value = { typeof(T1), typeof(T2), typeof(T3) };
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