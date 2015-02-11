﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiCopRule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface defining ApiCop rules.
    /// </summary>
    public interface IApiCopRule
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        string Url { get; }

        /// <summary>
        /// Gets the level of impact this rule has.
        /// </summary>
        /// <value>The level.</value>
        ApiCopRuleLevel Level { get; }

        /// <summary>
        /// Determines whether the specified ApiCop rule is valid.
        /// </summary>
        /// <param name="apiCop">The ApiCop.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the specified ApiCop is valid; otherwise, <c>false</c>.</returns>
        bool IsValid(IApiCop apiCop, string tag);

        /// <summary>
        /// Gets all the tags used by this rule.
        /// </summary>
        /// <returns>The list of tags.</returns>
        IEnumerable<string> GetTags();

        /// <summary>
        /// Gets the result as text.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The result as text.</returns>
        string GetResultAsText(string tag);
    }
}