namespace Catel.Services
{
    using System;
    using System.Globalization;

    public static class ILanguageServiceExtensions
    {
        /// <summary>
        /// Gets the string with the <see cref="LanguageService.PreferredCulture" />. If the preferred language cannot be
        /// found, this method will use the <see cref="LanguageService.FallbackCulture" /> to retrieve the
        /// string.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        public static string GetRequiredString(this ILanguageService languageService, string resourceName)
        {
            ArgumentNullException.ThrowIfNull(languageService);

            var result = languageService.GetString(resourceName);
            if (result is null)
            {
                throw new CatelException($"Cannot find language resource string '{resourceName}'");
            }

            return result;
        }

        /// <summary>
        /// Gets the string with the specified culture.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureInfo" /> is <c>null</c>.</exception>
        public static string GetRequiredString(this ILanguageService languageService, string resourceName, CultureInfo cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(languageService);

            var result = languageService.GetString(resourceName, cultureInfo);
            if (result is null)
            {
                throw new CatelException($"Cannot find language resource string '{resourceName}'");
            }

            return result;
        }

        /// <summary>
        /// Gets the string with the specified language source and culture.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="languageSource">The language source.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="languageSource" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureInfo" /> is <c>null</c>.</exception>
        public static string GetRequiredString(this ILanguageService languageService, ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
        {
            ArgumentNullException.ThrowIfNull(languageService);

            var result = languageService.GetString(languageSource, resourceName, cultureInfo);
            if (result is null)
            {
                throw new CatelException($"Cannot find language resource string '{resourceName}'");
            }

            return result;
        }
    }
}
