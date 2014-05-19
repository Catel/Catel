// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiCopResult.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    /// <summary>
    /// Interface defining ApiCop results.
    /// </summary>
    public interface IApiCopResult
    {
        /// <summary>
        /// Gets the cop.
        /// </summary>
        /// <value>The cop.</value>
        IApiCop Cop { get; }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>The rule.</value>
        IApiCopRule Rule { get; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        string Tag { get; }
    }
}