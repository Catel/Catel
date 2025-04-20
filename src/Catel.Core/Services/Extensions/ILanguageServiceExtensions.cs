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
        /// Gets the string with the <see cref="LanguageService.PreferredCulture" />. If the preferred language cannot be
        /// found, this method will use the <see cref="LanguageService.FallbackCulture" /> to retrieve the
        /// string.
        /// <para/>
        /// Once successfully retrieved, the string will be formatted using the <see cref="string.Format(IFormatProvider, string, object[])" /> method.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="args">The arguments to pass to the string formatting method.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        public static string GetRequiredStringAndFormat(this ILanguageService languageService, string resourceName, params object?[] args)
        {
            ArgumentNullException.ThrowIfNull(languageService);

            var result = languageService.GetRequiredString(resourceName);
            return Format(result, args);
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
        /// Gets the string with the <see cref="LanguageService.PreferredCulture" />. If the preferred language cannot be
        /// found, this method will use the <see cref="LanguageService.FallbackCulture" /> to retrieve the
        /// string.
        /// <para/>
        /// Once successfully retrieved, the string will be formatted using the <see cref="string.Format(IFormatProvider, string, object[])" /> method.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <param name="args">The arguments to pass to the string formatting method.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        public static string GetRequiredStringAndFormat(this ILanguageService languageService, string resourceName, CultureInfo cultureInfo, params object?[] args)
        {
            ArgumentNullException.ThrowIfNull(languageService);

            var result = languageService.GetRequiredString(resourceName, cultureInfo);
            return Format(result, args);
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

        /// <summary>
        /// Gets the string with the <see cref="LanguageService.PreferredCulture" />. If the preferred language cannot be
        /// found, this method will use the <see cref="LanguageService.FallbackCulture" /> to retrieve the
        /// string.
        /// <para/>
        /// Once successfully retrieved, the string will be formatted using the <see cref="string.Format(IFormatProvider, string, object[])" /> method.
        /// </summary>
        /// <param name="languageService">The language service.</param>
        /// <param name="languageSource">The language source.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <param name="args">The arguments to pass to the string formatting method.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        public static string GetRequiredStringAndFormat(this ILanguageService languageService, ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo, params object?[] args)
        {
            ArgumentNullException.ThrowIfNull(languageService);

            var result = languageService.GetRequiredString(languageSource, resourceName, cultureInfo);
            return Format(result, args);
        }

        private static string Format(string format, params object?[] args)
        {
            if (args is null || args.Length == 0)
            {
                return format;
            }

            return string.Format(format, args);
        }
    }
}
