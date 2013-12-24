// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositePredicate.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Composite predicate.
    /// </summary>
    /// <typeparam name="T">The predicates.</typeparam>
    public class CompositePredicate<T> where T : class 
    {
        #region Fields
        private readonly List<Predicate<T>> _filters = new List<Predicate<T>>();
        private Predicate<T> _matchesAll = candidate => true;
        private Predicate<T> _matchesAny = candidate => true;
        private Predicate<T> _matchesNone = candidate => false;
        #endregion

        #region Methods
        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="filter"/> is <c>null</c>.</exception>
        private void Add(Predicate<T> filter)
        {
            Argument.IsNotNull("filter", filter);

            _matchesAll = candidate => _filters.All(predicate => predicate(candidate));
            _matchesAny = candidate => _filters.Any(predicate => predicate(candidate));
            _matchesNone = candidate => !MatchesAny(candidate);

            _filters.Add(filter);
        }

        /// <summary>
        /// Checks whether the specified target matches all of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target matches all of the filters, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public bool MatchesAll(T target)
        {
            Argument.IsNotNull("target", target);

            return _matchesAll(target);
        }

        /// <summary>
        /// Checks whether the specified target matches any of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target matches any of the filters, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public bool MatchesAny(T target)
        {
            Argument.IsNotNull("target", target);

            return _matchesAny(target);
        }

        /// <summary>
        /// Checks whether the specified target matches none of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target matches none of the filters, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public bool MatchesNone(T target)
        {
            Argument.IsNotNull("target", target);

            return _matchesNone(target);
        }

        /// <summary>
        /// Checks whether the specified target does not match any of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target does not match any of the filters, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public bool DoesNotMatchAny(T target)
        {
            Argument.IsNotNull("target", target);

            return !_filters.Any() || !MatchesAny(target);
        }

        /// <summary>
        /// +s the specified invokes.
        /// </summary>
        /// <param name="invokes">The invokes.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="invokes"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="filter"/> is <c>null</c>.</exception>
        public static CompositePredicate<T> operator +(CompositePredicate<T> invokes, Predicate<T> filter)
        {
            Argument.IsNotNull("invokes", invokes);
            Argument.IsNotNull("filter", filter);

            invokes.Add(filter);
            return invokes;
        }
        #endregion
    }
}