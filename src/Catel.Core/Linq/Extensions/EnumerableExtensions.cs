namespace Catel.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Catel.Caching;
    using Catel.Reflection;

    /// <summary>
    /// The enumerable extensions class.
    /// </summary>
    public static class EnumerableExtensions
    {
        private static readonly MethodInfo CastMethodInfo = typeof(Enumerable).GetMethodEx("Cast", BindingFlags.Static | BindingFlags.Public)!;
        private static readonly MethodInfo ToArrayMethodInfo = typeof(Enumerable).GetMethodEx("ToArray", BindingFlags.Static | BindingFlags.Public)!;
        private static readonly MethodInfo ToListMethodInfo = typeof(Enumerable).GetMethodEx("ToList", BindingFlags.Static | BindingFlags.Public)!;

        private static readonly CacheStorage<Type, MethodInfo> CastGenericMethodInfo = new CacheStorage<Type, MethodInfo>();
        private static readonly CacheStorage<Type, MethodInfo> ToArrayGenericMethodInfoCache = new CacheStorage<Type, MethodInfo>();
        private static readonly CacheStorage<Type, MethodInfo> ToListGenericMethodInfoCache = new CacheStorage<Type, MethodInfo>();
        private static readonly CacheStorage<Type, MethodInfo> AsReadOnlyGenericMethodInfoCache = new CacheStorage<Type, MethodInfo>();

        static EnumerableExtensions()
        {
        }

        /// <summary>
        /// Element wise cast an <see cref="Enumerable" /> to <paramref name="type" />.
        /// </summary>
        /// <param name="instance">The enumerable</param>
        /// <param name="type">The type</param>
        /// <returns>The <see cref="IEnumerable" /> with element cast to <paramref name="type" /></returns>
        public static IEnumerable Cast(this IEnumerable instance, Type type)
        {
            var methodInfo = CastGenericMethodInfo.GetFromCacheOrFetch(type, () => CastMethodInfo!.MakeGenericMethod(type));
            if (methodInfo is null)
            {
                throw new CatelException("Cannot create generic Cast method");
            }

            var result = methodInfo.Invoke(null, new object[] { instance }) as IEnumerable;
            if (result is null)
            {
                throw new CatelException("Could not create IEnumerable using the Cast method");
            }

            return result;
        }

        /// <summary>
        /// Converts an <see cref="Enumerable" /> into an array of <paramref name="type" />
        /// </summary>
        /// <param name="instance">The enumerable</param>
        /// <param name="type">The type</param>
        /// <returns>The array of <paramref name="type" /> as <see cref="IEnumerable" /></returns>
        public static IEnumerable ToSystemArray(this IEnumerable instance, Type type)
        {
            var methodInfo = ToArrayGenericMethodInfoCache.GetFromCacheOrFetch(type, () => ToArrayMethodInfo!.MakeGenericMethod(type));
            if (methodInfo is null)
            {
                throw new CatelException("Cannot create generic ToArray method");
            }

            var result = methodInfo.Invoke(null, new object[] { instance }) as IEnumerable;
            if (result is null)
            {
                throw new CatelException("Could not create IEnumerable using the ToArray method");
            }

            return result;
        }

        /// <summary>
        /// Converts an <see cref="Enumerable" /> into a <see cref="IList{T}" />
        /// </summary>
        /// <param name="instance">The enumerable</param>
        /// <param name="type">The type</param>
        /// <returns>The <see cref="IList{T}" /> as <see cref="IEnumerable" /></returns>
        public static IEnumerable ToList(this IEnumerable instance, Type type)
        {
            var methodInfo = ToListGenericMethodInfoCache.GetFromCacheOrFetch(type, () => ToListMethodInfo!.MakeGenericMethod(type));
            if (methodInfo is null)
            {
                throw new CatelException("Cannot create generic ToList method");
            }

            var result = methodInfo.Invoke(null, new object[] { instance }) as IEnumerable;
            if (result is null)
            {
                throw new CatelException("Could not create IEnumerable using the ToList method");
            }

            return result;
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
            var methodInfo = AsReadOnlyGenericMethodInfoCache.GetFromCacheOrFetch(type, () => list.GetType().GetMethodEx("AsReadOnly")!);
            if (methodInfo is null)
            {
                throw new CatelException("Cannot create generic AsReadOnly method");
            }

            var result = methodInfo.Invoke(list, Array.Empty<object>()) as IEnumerable;
            if (result is null)
            {
                throw new CatelException("Could not create IEnumerable using the AsReadOnly method");
            }

            return result;
        }
    }
}
