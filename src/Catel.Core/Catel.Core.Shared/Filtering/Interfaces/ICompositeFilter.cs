// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositeFilter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// Composite filter.
    /// </summary>
    /// <typeparam name="T">Type of the filter.</typeparam>
    public interface ICompositeFilter<T> 
        where T : class
    {
        #region Properties
        /// <summary>
        /// Gets the includes.
        /// </summary>
        /// <value>The includes.</value>
        CompositePredicate<T> Includes { get; set; }

        /// <summary>
        /// Gets or sets the excludes.
        /// </summary>
        /// <value>The excludes.</value>
        CompositePredicate<T> Excludes { get; set; }
        #endregion

        /// <summary>
        /// Checks whether the target matches any of the <see cref="Includes"/> and does
        /// not match any of the <see cref="Excludes"/>.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the match is a successful hit, <c>false</c> otherwise.</returns>
        bool Matches(T target);

        /// <summary>
        /// Object implementation of the <see cref="CompositeFilter{T}.Matches"/> method so it can be used for non-generic predicates.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the match is a successful hit, <c>false</c> otherwise.</returns>
        bool MatchesObject(object target);
    }
}