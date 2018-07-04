// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCop.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ApiCop writer class.
    /// </summary>
    public class ApiCop : IApiCop
    {
        private readonly object _lock = new object();
        private readonly Dictionary<string, IApiCopRule> _rules = new Dictionary<string, IApiCopRule>();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCop"/> class.
        /// </summary>
        /// <param name="targetType">The type for which this ApiCop is intented.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> is <c>null</c>.</exception>
        public ApiCop(Type targetType)
        {
            Argument.IsNotNull("targetType", targetType);

            TargetType = targetType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target type of the ApiCop. This is the type where the ApiCop is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }
        #endregion

        /// <summary>
        /// Registers the rule with this ApiCop.
        /// </summary>
        /// <typeparam name="TRule">The type of the rule.</typeparam>
        /// <param name="rule">The rule.</param>
        /// <returns>The rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="rule"/> is <c>null</c>.</exception>
        public IApiCop RegisterRule<TRule>(TRule rule)
            where TRule : IApiCopRule
        {
            if (!ApiCopManager.IsEnabled)
            {
                return this;
            }

            Argument.IsNotNull("rule", rule);

            lock (_lock)
            {
                if (!_rules.ContainsKey(rule.Name))
                {
                    _rules.Add(rule.Name, rule);
                }

                return this;
            }
        }

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
        public void UpdateRule<TRule>(string name, Action<TRule> action)
            where TRule : IApiCopRule
        {
            if (!ApiCopManager.IsEnabled)
            {
                return;
            }

            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("action", action);

            lock (_lock)
            {
                if (_rules.TryGetValue(name, out var rule))
                {
                    var typedRule = (TRule)rule;
                    action(typedRule);
                }
            }
        }

        /// <summary>
        /// Gets the results of this specific ApiCop.
        /// </summary>
        /// <returns>The results of this ApiCop.</returns>
        public IEnumerable<IApiCopResult> GetResults()
        {
            var results = new List<IApiCopResult>();

            lock (_lock)
            {
                foreach (var ruleKeyValuePair in _rules)
                {
                    var rule = ruleKeyValuePair.Value;
                    var tags = rule.GetTags();

                    foreach (var tag in tags)
                    {
                        if (!rule.IsValid(this, tag))
                        {
                            results.Add(new ApiCopResult(this, rule, tag));
                        }
                    }
                }
            }

            return results;
        }
    }
}
