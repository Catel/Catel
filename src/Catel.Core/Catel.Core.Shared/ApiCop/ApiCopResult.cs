// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCopResult.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;

    /// <summary>
    /// ApiCop result class.
    /// </summary>
    public class ApiCopResult : IApiCopResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCopResult"/> class.
        /// </summary>
        /// <param name="cop">The API cop.</param>
        /// <param name="rule">The rule.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="cop"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="tag"/> is <c>null</c> or whitespace.</exception>
        public ApiCopResult(IApiCop cop, IApiCopRule rule, string tag)
        {
            Argument.IsNotNull("cop", cop);
            Argument.IsNotNull("rule", rule);
            Argument.IsNotNullOrWhitespace("tag", tag);

            Cop = cop;
            Rule = rule;
            Tag = tag;
        }

        /// <summary>
        /// Gets the cop.
        /// </summary>
        /// <value>The cop.</value>
        public IApiCop Cop { get; private set; }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>The rule.</value>
        public IApiCopRule Rule { get; private set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; private set; }
    }
}