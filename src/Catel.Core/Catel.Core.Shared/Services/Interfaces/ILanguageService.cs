// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILanguageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Service to implement the retrieval of language services.
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Gets or sets the fallback culture.
        /// </summary>
        /// <value>The fallback culture.</value>
        CultureInfo FallbackCulture { get; set; }

        /// <summary>
        /// Gets or sets the preferred culture.
        /// </summary>
        /// <value>The preferred culture.</value>
        CultureInfo PreferredCulture { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the results should be cached.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if the results should be cached; otherwise, <c>false</c>.</value>
        bool CacheResults { get; set; }

        /// <summary>
        /// Occurs when the <see cref="FallbackCulture"/> or <see cref="PreferredCulture"/> are updated.
        /// </summary>
        event EventHandler<EventArgs> LanguageUpdated;
        
        /// <summary>
        /// Registers the language source.
        /// </summary>
        /// <param name="languageSource">The language source.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="languageSource" /> is <c>null</c> or whitespace.</exception>
        void RegisterLanguageSource(ILanguageSource languageSource);

        /// <summary>
        /// Gets the string with the <see cref="LanguageService.PreferredCulture" />. If the preferred language cannot be
        /// found, this method will use the <see cref="LanguageService.FallbackCulture" /> to retrieve the
        /// string.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        string GetString(string resourceName);

        /// <summary>
        /// Gets the string with the specified culture.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureInfo" /> is <c>null</c>.</exception>
        string GetString(string resourceName, CultureInfo cultureInfo);

        /// <summary>
        /// Preloads the language sources to provide optimal performance.
        /// </summary>
        void PreloadLanguageSources();

        /// <summary>
        /// Clears the language resources.
        /// </summary>
        void ClearLanguageResources();
    }
}