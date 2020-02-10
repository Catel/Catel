// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Catel.Caching;
    using Catel.Collections;
    using Catel.Reflection;

    /// <summary>
    /// The enumerable extensions class.
    /// </summary>
    public static class EnumerableExtensions
    {
        private static readonly MethodInfo CastMethodInfo = typeof(Enumerable).GetMethodEx("Cast", BindingFlags.Static | BindingFlags.Public);

        private static readonly MethodInfo ToArrayMethodInfo = typeof(Enumerable).GetMethodEx("ToArray", BindingFlags.Static | BindingFlags.Public);

        private static readonly MethodInfo ToListMethodInfo = typeof(Enumerable).GetMethodEx("ToList", BindingFlags.Static | BindingFlags.Public);

        private static readonly CacheStorage<Type, MethodInfo> CastGenericMethodInfo = new CacheStorage<Type, MethodInfo>();

        private static readonly CacheStorage<Type, MethodInfo> ToArrayGenericMethodInfoCache = new CacheStorage<Type, MethodInfo>();

        private static readonly CacheStorage<Type, MethodInfo> ToListGenericMethodInfoCache = new CacheStorage<Type, MethodInfo>();

        private static readonly CacheStorage<Type, MethodInfo> AsReadOnlyGenericMethodInfoCache = new CacheStorage<Type, MethodInfo>();

        /// <summary>
        /// Element wise cast an <see cref="Enumerable" /> to <paramref name="type" />.
        /// </summary>
        /// <param name="instance">The enumerable</param>
        /// <param name="type">The type</param>
        /// <returns>The <see cref="IEnumerable" /> with element cast to <paramref name="type" /></returns>
        public static IEnumerable Cast(this IEnumerable instance, Type type)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("type", type);

#pragma warning disable HAA0101 // Array allocation for params parameter
            var methodInfo = CastGenericMethodInfo.GetFromCacheOrFetch(type, () => CastMethodInfo.MakeGenericMethod(type));
#pragma warning restore HAA0101 // Array allocation for params parameter
            return (IEnumerable)methodInfo.Invoke(null, new object[] { instance });
        }

        /// <summary>
        /// Converts an <see cref="Enumerable" /> into an array of <paramref name="type" />
        /// </summary>
        /// <param name="instance">The enumerable</param>
        /// <param name="type">The type</param>
        /// <returns>The array of <paramref name="type" /> as <see cref="IEnumerable" /></returns>
        public static IEnumerable ToSystemArray(this IEnumerable instance, Type type)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("type", type);

#pragma warning disable HAA0101 // Array allocation for params parameter
            var methodInfo = ToArrayGenericMethodInfoCache.GetFromCacheOrFetch(type, () => ToArrayMethodInfo.MakeGenericMethod(type));
#pragma warning restore HAA0101 // Array allocation for params parameter
            return (IEnumerable)methodInfo.Invoke(null, new object[] { instance });
        }

        /// <summary>
        /// Converts an <see cref="Enumerable" /> into a <see cref="IList{T}" />
        /// </summary>
        /// <param name="instance">The enumerable</param>
        /// <param name="type">The type</param>
        /// <returns>The <see cref="IList{T}" /> as <see cref="IEnumerable" /></returns>
        public static IEnumerable ToList(this IEnumerable instance, Type type)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("type", type);

#pragma warning disable HAA0101 // Array allocation for params parameter
            var methodInfo = ToListGenericMethodInfoCache.GetFromCacheOrFetch(type, () => ToListMethodInfo.MakeGenericMethod(type));
#pragma warning restore HAA0101 // Array allocation for params parameter
            return (IEnumerable)methodInfo.Invoke(null, new object[] { instance });
        }

        /// <summary>
        /// Converts an <see cref="Enumerable" /> into a <see cref="IReadOnlyList{T}" /> or <see cref="IReadOnlyCollection{T}" />
        /// </summary>
        /// <param name="instance">The enumerable</param>
        /// <param name="type">The type</param>
        /// <returns>The <see cref="IReadOnlyList{T}" /> or <see cref="IReadOnlyCollection{T}" /> as <see cref="IEnumerable" /></returns>
        public static IEnumerable AsReadOnly(this IEnumerable instance, Type type)
        {
            var list = instance.ToList(type);
            var methodInfo = AsReadOnlyGenericMethodInfoCache.GetFromCacheOrFetch(type, () => list.GetType().GetMethodEx("AsReadOnly"));
            return (IEnumerable)methodInfo.Invoke(list, ArrayShim.Empty<object>());
        }
    }
}
