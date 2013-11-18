// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositePredicate.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompositePredicate<T>
    {
        #region Fields
        private readonly List<Predicate<T>> _filters = new List<Predicate<T>>();
        private Predicate<T> _matchesAll = x => true;
        private Predicate<T> _matchesAny = x => true;
        private Predicate<T> _matchesNone = x => false;
        #endregion

        #region Methods
        private void Add(Predicate<T> filter)
        {
            _matchesAll = x => _filters.All(predicate => predicate(x));
            _matchesAny = x => _filters.Any(predicate => predicate(x));
            _matchesNone = x => !MatchesAny(x);

            _filters.Add(filter);
        }

        /// <summary>
        /// Matcheses all.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public bool MatchesAll(T target)
        {
            return _matchesAll(target);
        }

        /// <summary>
        /// Matcheses any.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public bool MatchesAny(T target)
        {
            return _matchesAny(target);
        }

        /// <summary>
        /// Matcheses the none.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public bool MatchesNone(T target)
        {
            return _matchesNone(target);
        }

        /// <summary>
        /// Doeses the not matche any.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public bool DoesNotMatcheAny(T target)
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