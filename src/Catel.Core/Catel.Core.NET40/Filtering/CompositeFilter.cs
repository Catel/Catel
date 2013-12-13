// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeFilter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    /// <summary>
    /// Composite filter.
    /// </summary>
    /// <typeparam name="T">Type of the filter.</typeparam>
    public class CompositeFilter<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeFilter{T}"/> class.
        /// </summary>
        public CompositeFilter()
        {
            Excludes = new CompositePredicate<T>();
            Includes = new CompositePredicate<T>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the includes.
        /// </summary>
        /// <value>The includes.</value>
        public CompositePredicate<T> Includes { get; internal set; }

        /// <summary>
        /// Gets or sets the excludes.
        /// </summary>
        /// <value>The excludes.</value>
        public CompositePredicate<T> Excludes { get; internal set; }
        #endregion

        #region Methods
        /// <summary>
        /// Checks whether the target matches any of the <see cref="Includes"/> and does
        /// not match any of the <see cref="Excludes"/>.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the match is a successful hit, <c>false</c> otherwise.</returns>
        public bool Matches(T target)
        {
            return Includes.MatchesAny(target) && Excludes.DoesNotMatchAny(target);
        }
        #endregion
    }
}