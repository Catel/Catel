// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILanguageSource.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    /// <summary>
    /// Interface defining a language source.
    /// </summary>
    public interface ILanguageSource
    {
        /// <summary>
        /// Gets the source for the current language source.
        /// </summary>
        /// <returns>The source string.</returns>
        string GetSource();
    }
}