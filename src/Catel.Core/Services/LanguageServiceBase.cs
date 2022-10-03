namespace Catel.Services
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Abstract class to allow partial abstract methods.
    /// </summary>
    public abstract class LanguageServiceBase
    {
        /// <summary>
        /// Preloads the language sources to provide optimal performance.
        /// </summary>
        /// <param name="languageSource">The language source.</param>
        protected abstract void PreloadLanguageSource(ILanguageSource languageSource);

        /// <summary>
        /// Gets the string from the specified resource file with the current culture.
        /// </summary>
        /// <param name="languageSource">The language source.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the string cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="languageSource" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureInfo" /> is <c>null</c>.</exception>
        public abstract string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo);
    }
}
