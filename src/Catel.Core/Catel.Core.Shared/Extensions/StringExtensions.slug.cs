// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.slug.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public static partial class StringExtensions
    {
        #region Constants
        /// <summary>
        /// The slug regex.
        /// </summary>
        public static readonly Regex SlugRegex = new Regex(@"[^A-Za-z0-9_.]+");

        /// <summary>
        /// The white space regex.
        /// </summary>
        public static readonly Regex WhiteSpaceRegex = new Regex(@"[\s]+");
        #endregion

        #region Methods
        /// <summary>
        /// Gets the slug of the specific input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="spaceReplacement">The space replacement.</param>
        /// <param name="dotReplacement">The dot replacement.</param>
        /// <returns>System.String.</returns>
        public static string GetSlug(this string input, string spaceReplacement = "", string dotReplacement = "")
        {
            Argument.IsNotNullOrWhitespace("input", input);
            Argument.IsNotNull("spaceReplacement", spaceReplacement);
            Argument.IsNotNull("dotReplacement", dotReplacement);

#if NET
            input = input.RemoveDiacritics();
#endif

            var output = WhiteSpaceRegex.Replace(input, spaceReplacement);
            output = SlugRegex.Replace(output, string.Empty).ToLowerInvariant();

            output = output.Replace(".", dotReplacement);

            return output;
        }

#if NET
        /// <summary>
        /// Removes the diacritics (special characters) from the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string RemoveDiacritics(this string value)
        {
            var normalizedString = value.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
#endif
        #endregion
    }
}