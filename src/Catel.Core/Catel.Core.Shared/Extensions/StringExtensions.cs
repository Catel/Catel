// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    /// <summary>
    /// The string extensions.
    /// </summary>
    public static partial class StringExtensions
    {
        #region Methods
        /// <summary>
        /// Prepares a string value as search filter by trimming it and making it lower-case.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The search filter.</returns>
        public static string PrepareAsSearchFilter(this string filter)
        {
            var filterText = filter;
            if (string.IsNullOrWhiteSpace(filterText))
            {
                filterText = string.Empty;
            }

            filterText = filterText.ToLower().Trim();
            return filterText;
        }
        #endregion
    }
}