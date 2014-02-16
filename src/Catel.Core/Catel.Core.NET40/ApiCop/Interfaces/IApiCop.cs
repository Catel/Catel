// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApiCop.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface to specify writers for the ApiCop functionality.
    /// </summary>
    public interface IApiCop
    {
        /// <summary>
        /// Gets the target type of the ApiCop. This is the type where the ApiCop is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        Type TargetType { get; }

        /// <summary>
        /// Gets the results of this specific ApiCop.
        /// </summary>
        /// <returns>The results of this ApiCop.</returns>
        IEnumerable<IApiCopResult> GetResults();

        /// <summary>
        /// Registers the rule with this ApiCop.
        /// </summary>
        /// <typeparam name="TRule">The type of the rule.</typeparam>
        /// <param name="rule">The rule.</param>
        /// <returns>The rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="rule"/> is <c>null</c>.</exception>
        IApiCop RegisterRule<TRule>(TRule rule)
            where TRule : IApiCopRule;

        /// <summary>
        /// Updates the rule with the specified name. If the rule is found and the ApiCop functionality is enabled,
        /// the specified action will be invoked on the registered rule.
        /// </summary>
        /// <typeparam name="TRule">The type of the rule.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <returns>The rule or <c>null</c> if the rule is not registered first.</returns>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void UpdateRule<TRule>(string name, Action<TRule> action)
            where TRule : IApiCopRule;
    }
}