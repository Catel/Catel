namespace Catel
{
    using System;

    /// <summary>
    /// The string extensions.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Splits the string by camel case, e.g. 'HiThere' will result in 'Hi there'.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string SplitCamelCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var finalString = string.Empty;

            for (int i = 0; i < value.Length; i++)
            {
                if (i != 0)
                {
                    if (char.IsUpper(value[i]))
                    {
                        finalString += " " + char.ToLower(value[i]);
                        continue;
                    }
                }

                finalString += value[i];
            }

            return finalString;
        }

        /// <summary>
        /// Executes a string comparison that is case insensitive.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="valueToCheck">The value to check.</param>
        /// <returns><c>true</c> if the strings are equal, <c>false</c> otherwise.</returns>
        public static bool EqualsIgnoreCase(this string str, string valueToCheck)
        {
            if (str is null)
            {
                return false;
            }

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
            if (str is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(valueToCheck))
            {
                return false;
            }

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
            if (str is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(valueToCheck))
            {
                return false;
            }

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
            if (str is null)
            {
                return -1;
            }

            if (string.IsNullOrEmpty(valueToCheck))
            {
                return -1;
            }

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

        /// <summary>
        /// Determines whether the string equals any of the values.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="values">The values to check for.</param>
        /// <returns><c>true</c> if the string equals any of the values, <c>false</c> otherwise.</returns>
        public static bool EqualsAny(this string str, params string[] values)
        {
            if (str is null)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (str.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the string equals with any of the values.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="values">The values to check for.</param>
        /// <returns><c>true</c> if the string equals any of the values, <c>false</c> otherwise.</returns>
        public static bool EqualsAnyIgnoreCase(this string str, params string[] values)
        {
            if (str is null)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (str.EqualsIgnoreCase(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the string starts with any of the values.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="values">The values to check for.</param>
        /// <returns><c>true</c> if the string starts with any of the values, <c>false</c> otherwise.</returns>
        public static bool StartsWithAny(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (var value in values)
            {
                if (str.StartsWith(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the string starts with any of the values.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="values">The values to check for.</param>
        /// <returns><c>true</c> if the string starts with any of the values, <c>false</c> otherwise.</returns>
        public static bool StartsWithAnyIgnoreCase(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (var value in values)
            {
                if (str.StartsWithIgnoreCase(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the string ends with any of the values.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="values">The values to check for.</param>
        /// <returns><c>true</c> if the string ends with any of the values, <c>false</c> otherwise.</returns>
        public static bool EndsWithAny(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (var value in values)
            {
                if (str.EndsWith(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the string ends with any of the values.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="values">The values to check for.</param>
        /// <returns><c>true</c> if the string ends with any of the values, <c>false</c> otherwise.</returns>
        public static bool EndsWithAnyIgnoreCase(this string str, params string[] values)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (var value in values)
            {
                if (str.EndsWithIgnoreCase(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
