// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TooManyDependenciesApiCopRule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop.Rules
{
    using System;
    using System.Collections.Generic;
    using Catel.Reflection;

    /// <summary>
    /// Rule to prevent too many dependencies on a class.
    /// </summary>
    public class TooManyDependenciesApiCopRule : ApiCopRule
    {
        #region Constants
        /// <summary>
        /// The maximum dependencies
        /// </summary>
        private const int MaxDependencies = 8;
        #endregion

        #region Fields
        private readonly Dictionary<string, int> _dependenciesPerType = new Dictionary<string, int>();
        private readonly object _lockObject = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCopRule" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="level">The level.</param>
        /// <param name="url">The URL.</param>
        public TooManyDependenciesApiCopRule(string name, string description, ApiCopRuleLevel level, string url = null)
            : base(name, description, level, url)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the number of dependencies injected for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="numberOfDependencies">The number of dependencies.</param>
        public void SetNumberOfDependenciesInjected(Type type, int numberOfDependencies)
        {
            if (type is null)
            {
                return;
            }

            lock (_lockObject)
            {
                var typeName = type.GetSafeFullName(true);

                AddTag(typeName);

                var update = false;

                if (_dependenciesPerType.TryGetValue(typeName, out var existingNumberOfDependencies))
                {
                    if (existingNumberOfDependencies < numberOfDependencies)
                    {
                        update = true;
                    }
                }
                else
                {
                    update = true;
                }

                if (update)
                {
                    _dependenciesPerType[typeName] = numberOfDependencies;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified ApiCop rule is valid.
        /// </summary>
        /// <param name="apiCop">The ApiCop.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the specified ApiCop is valid; otherwise, <c>false</c>.</returns>
        public override bool IsValid(IApiCop apiCop, string tag)
        {
            if (!_dependenciesPerType.TryGetValue(tag, out var maxDependencies))
            {
                maxDependencies = 0;
            }

            return maxDependencies <= MaxDependencies;
        }

        /// <summary>
        /// Gets the result as text.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The result as text.</returns>
        public override string GetResultAsText(string tag)
        {
            if (!_dependenciesPerType.TryGetValue(tag, out var maxDependencies))
            {
                maxDependencies = 0;
            }

            return string.Format("[{0}] Type has '{1}' dependencies injected, consider splitting the class into multiple classes",
                tag, maxDependencies);
        }
        #endregion
    }
}
