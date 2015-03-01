// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
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
        protected abstract string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo);
    }
}