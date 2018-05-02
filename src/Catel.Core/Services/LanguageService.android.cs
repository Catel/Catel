// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.Services
{
    using System;
    using System.Globalization;
    using global::Android.Content.Res;

    public partial class LanguageService
    {
        /// <summary>
        /// Preloads the language sources to provide optimal performance.
        /// </summary>
        /// <param name="languageSource">The language source.</param>    
        protected override void PreloadLanguageSource(ILanguageSource languageSource)
        {
            // TODO: cache parsed string xmls?
        }
    
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
        public override string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
        {
            Argument.IsNotNull("languageSource", languageSource);
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);
            Argument.IsNotNull("cultureInfo", cultureInfo);

            //var source = languageSource.GetSource();

            //var stringsXml = string.Format("res/values-{0}/strings.xml", cultureInfo.TwoLetterISOLanguageName.ToLower());
            //var xpath = string.Format("/resources/string[@name='{0}']", resourceName);

            //var stringsXmlDocument = XDocument.Load(stringsXml);

            //var element = stringsXmlDocument.XPathSelectElement(xpath);
            //return element.Value;

            var context = Android.ContextHelper.CurrentContext;
            var packageName = context.PackageName;
            
            var id = context.Resources.GetIdentifier(resourceName, "string", packageName);
            //var id = Resources.System.GetIdentifier(resourceName, "string", packageName);
            return context.GetString(id);
        }
    }
}

#endif