// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using Catel.IoC;
    using Catel.Services;

    /// <summary>
    /// Resource helper class to read resource files.
    /// </summary>
    public static class ResourceHelper
    {
        private static readonly ILanguageService _languageService = ServiceLocator.Default.ResolveType<ILanguageService>();

        /// <summary>
        /// Gets the string from the specified resource file.
        /// </summary>
        /// <param name="callingType">Type of the calling.</param>
        /// <param name="resourceFile">The resource file.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <exception cref="ArgumentException">The <paramref name="resourceFile"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceName"/> is <c>null</c> or whitespace.</exception>
        public static string GetString(Type callingType, string resourceFile, string resourceName)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            return GetString(resourceName);
        }

        /// <summary>
        /// Gets the string from the specified resource file.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c> or whitespace.</exception>
        public static string GetString(string resourceName)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            return _languageService.GetString(resourceName);
        }
    }
}