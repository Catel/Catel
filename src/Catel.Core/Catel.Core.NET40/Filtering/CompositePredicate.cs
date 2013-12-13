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
    public class CompositePredicate<T>
    {
        #region Fields
        private readonly List<Predicate<T>> _filters = new List<Predicate<T>>();
        private Predicate<T> _matchesAll = x => true;
        private Predicate<T> _matchesAny = x => true;
        private Predicate<T> _matchesNone = x => false;
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

            _matchesAll = x => _filters.All(predicate => predicate(x));
            _matchesAny = x => _filters.Any(predicate => predicate(x));
            _matchesNone = x => !MatchesAny(x);

            _filters.Add(filter);
        }

        /// <summary>
        /// Checks whether the specified target matches all of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target matches all of the filters, <c>false</c> otherwise.</returns>
        public bool MatchesAll(T target)
        {
            return _matchesAll(target);
        }

        /// <summary>
        /// Checks whether the specified target matches any of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target matches any of the filters, <c>false</c> otherwise.</returns>
        public bool MatchesAny(T target)
        {
            return _matchesAny(target);
        }

        /// <summary>
        /// Checks whether the specified target matches none of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target matches none of the filters, <c>false</c> otherwise.</returns>
        public bool MatchesNone(T target)
        {
            return _matchesNone(target);
        }

        /// <summary>
        /// Checks whether the specified target does not match any of the registered predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the target does not match any of the filters, <c>false</c> otherwise.</returns>
        public bool DoesNotMatchAny(T target)
        {
            return _filters.Count == 0 || !MatchesAny(target);
        }
        #endregion

        /// <summary>
        /// +s the specified invokes.
        /// </summary>
        /// <param name="invokes">The invokes.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public static CompositePredicate<T> operator +(CompositePredicate<T> invokes, Predicate<T> filter)
        {
            invokes.Add(filter);
            return invokes;
        }
    }
}