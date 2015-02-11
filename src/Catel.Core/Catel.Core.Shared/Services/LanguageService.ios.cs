﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageService.ios.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if IOS

namespace Catel.Services
{
    using System.Globalization;

    public partial class LanguageService
    {
        /// <summary>
        /// Preloads the language sources to provide optimal performance.
        /// </summary>
        /// <param name="languageSource">The language source.</param>    
        protected override void PreloadLanguageSource(ILanguageSource languageSource)
        {
            // Not required
        }
    
        /// <summary>
        /// Gets the string from the specified resource file with the current culture.
        /// </summary>
        /// <param name="languageSource">The language source.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the string cannot be found.</returns>
        protected override string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
        {
            var source = languageSource.GetSource();
            
            throw new MustBeImplementedException();

            //return null;
        }   
    }
}

#endif