// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// The string extensions.
    /// </summary>
    public static partial class StringExtensions
    {
        #region Methods
        /// <summary>
        /// Executes a string comparison that is case insensitive.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="valueToCheck">The value to check.</param>
        /// <returns><c>true</c> if the strings are equal, <c>false</c> otherwise.</returns>
        public static bool EqualsIgnoreCase(this string str, string valueToCheck)
        {
            return string.Equals(str, valueToCheck, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified string contains the value to check.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="valueToCheck">The value to check.</param>
        /// <returns><c>true</c> if the string contains the value to check, <c>false</c> otherwise.</returns>
        public static bool ContainsIgnoreCase(this string str, string valueToCheck)
        {
            var index = IndexOfIgnoreCase(str, valueToCheck);
            return index >= 0;
        }

        /// <summary>
        /// Determines whether the string starts with the value to check.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="valueToCheck">The value to check.</param>
        /// <returns><c>true</c> if the string starts with the value to check, <c>false</c> otherwise.</returns>
        public static bool StartsWithIgnoreCase(this string str, string valueToCheck)
        {
            var startsWith = str.StartsWith(valueToCheck, StringComparison.OrdinalIgnoreCase);
            return startsWith;
        }

        /// <summary>
        /// Determines whether the string ends with the value to check.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="valueToCheck">The value to check.</param>
        /// <returns><c>true</c> if the string ends with the value to check, <c>false</c> otherwise.</returns>
        public static bool EndsWithIgnoreCase(this string str, string valueToCheck)
        {
            var endsWith = str.EndsWith(valueToCheck, StringComparison.OrdinalIgnoreCase);
            return endsWith;
        }

        /// <summary>
        /// Determines the index of the value to check inside the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="valueToCheck">The value to check.</param>
        /// <returns>The index or <c>-1</c> if not found.</returns>
        public static int IndexOfIgnoreCase(this string str, string valueToCheck)
        {
            var index = str.IndexOf(valueToCheck, StringComparison.OrdinalIgnoreCase);
            return index;
        }

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