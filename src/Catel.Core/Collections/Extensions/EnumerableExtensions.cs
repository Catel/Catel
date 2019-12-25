﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/> implementations.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Creates an <see cref="ObservableDictionary{TKey,TValue}"/> from an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create an <see cref="ObservableDictionary{TKey,TValue}"/> from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="ObservableDictionary{TKey,TValue}"/> that contains values of type <typeparamref name="TElement"/> selected from the input sequence.</returns>
        public static ObservableDictionary<TKey, TElement> ToObservableDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            Argument.IsNotNull(nameof(source), source);
            Argument.IsNotNull(nameof(keySelector), keySelector);
            Argument.IsNotNull(nameof(elementSelector), elementSelector);

            ObservableDictionary<TKey, TElement> d = new ObservableDictionary<TKey, TElement>(comparer);

            foreach (TSource element in source)
            {
                d.Add(keySelector(element), elementSelector(element));
            }

            return d;
        }

        /// <summary>
        /// Creates an <see cref="ObservableDictionary{TKey,TValue}"/> from an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create an <see cref="ObservableDictionary{TKey,TValue}"/> from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>An <see cref="ObservableDictionary{TKey,TValue}"/> that contains values of type <typeparamref name="TElement"/> selected from the input sequence.</returns>
        public static ObservableDictionary<TKey, TElement> ToObservableDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return ToObservableDictionary(source, keySelector, elementSelector, null);
        }

        /// <summary>
        /// Creates an <see cref="ObservableDictionary{TKey,TValue}"/> from an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create an <see cref="ObservableDictionary{TKey,TValue}"/> from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
        /// <returns>An <see cref="ObservableDictionary{TKey,TValue}"/> that contains keys and values.</returns>
        public static ObservableDictionary<TKey, TSource> ToObservableDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return ToObservableDictionary(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
        }

        /// <summary>
        /// Creates an <see cref="ObservableDictionary{TKey,TValue}"/> from an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to create an <see cref="ObservableDictionary{TKey,TValue}"/> from.</param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <returns>An <see cref="ObservableDictionary{TKey,TValue}"/> that contains keys and values.</returns>
        public static ObservableDictionary<TKey, TSource> ToObservableDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ToObservableDictionary(source, keySelector, IdentityFunction<TSource>.Instance, null);
        }

        internal class IdentityFunction<TElement>
        {
            protected IdentityFunction()
            {
            }

            public static Func<TElement, TElement> Instance
            {
                get { return x => x; }
            }
        }
    }
}
