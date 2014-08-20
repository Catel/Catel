// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAutoCompletionService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Collections;

    /// <summary>
    /// Service to implement auto completion features.
    /// </summary>
    public interface IAutoCompletionService
    {
        #region Methods
        /// <summary>
        /// Gets the auto complete values.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="source">The source.</param>
        /// <returns>System.String[].</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        string[] GetAutoCompleteValues(string property, string filter, IEnumerable source);
        #endregion
    }
}