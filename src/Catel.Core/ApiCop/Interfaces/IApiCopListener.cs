// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiCopListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System.Collections.Generic;

    /// <summary>
    /// Grouping options for the <see cref="IApiCopListener"/> implementations.
    /// </summary>
    public enum ApiCopListenerGrouping
    {
        /// <summary>
        /// Group by cop.
        /// </summary>
        Cop,

        /// <summary>
        /// Group by rule.
        /// </summary>
        Rule,

        /// <summary>
        /// Group by tag.
        /// </summary>
        Tag
    }

    /// <summary>
    /// Interface defining ApiCop listeners.
    /// </summary>
    public interface IApiCopListener
    {
        /// <summary>
        /// Gets or sets the grouping for this listener.
        /// </summary>
        /// <value>The grouping.</value>
        ApiCopListenerGrouping Grouping { get; set; }

        /// <summary>
        /// Writes the results of the ApiCop feature.
        /// <para />
        /// Note that this will only contain invalid results. Valid results are not written to the
        /// listeners.
        /// </summary>
        /// <param name="results">The results.</param>
        void WriteResults(IEnumerable<IApiCopResult> results);
    }
}