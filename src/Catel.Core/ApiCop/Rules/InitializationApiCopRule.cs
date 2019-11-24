// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitializationApiCopRule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop.Rules
{
    using System;

    /// <summary>
    /// The initialization mode.
    /// </summary>
    public enum InitializationMode
    {
        /// <summary>
        /// The lazy.
        /// </summary>
        Lazy,

        /// <summary>
        /// The eager.
        /// </summary>
        Eager
    }

    /// <summary>
    /// Rule to show that classes should be initialized at startup.
    /// </summary>
    public class InitializationApiCopRule : ApiCopRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationApiCopRule" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="level">The level.</param>
        /// <param name="recommendedInitializationMode">The recommended initialization mode.</param>
        /// <param name="url">The URL.</param>
        public InitializationApiCopRule(string name, string description, ApiCopRuleLevel level, 
            InitializationMode recommendedInitializationMode, string url = null) 
            : base(name, description, level, url)
        {
            RecommendedInitializationMode = recommendedInitializationMode;
        }

        /// <summary>
        /// Gets the recommended initialization mode.
        /// </summary>
        /// <value>The recommended initialization mode.</value>
        public InitializationMode RecommendedInitializationMode { get; private set; }

        /// <summary>
        /// Determines whether the specified ApiCop rule is valid.
        /// </summary>
        /// <param name="apiCop">The ApiCop.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the specified ApiCop is valid; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool IsValid(IApiCop apiCop, string tag)
        {
            var propertyBag = GetPropertyBagForTag(tag);
            var actualInitializationMode = propertyBag.GetValue("ActualInitializationMode", InitializationMode.Lazy);

            return actualInitializationMode == RecommendedInitializationMode;
        }

        /// <summary>
        /// Gets the result as text.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The result as text.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string GetResultAsText(string tag)
        {
            string recommendation = string.Empty;

            switch (RecommendedInitializationMode)
            {
                case InitializationMode.Lazy:
                    recommendation = "It is recommended to not eager initialize this feature";
                    break;

                case InitializationMode.Eager:
                    recommendation = "It is recommended to not to lazy initialize this feature, initialize at startup instead";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(RecommendedInitializationMode));
            }

            return string.Format("[{0}] {1}", tag, recommendation);
        }

        /// <summary>
        /// Sets the initialization model. If the value is already set, it won't be updated so it can be used without
        /// checking for previous states.
        /// </summary>
        /// <param name="initializationMode">The initialization mode.</param>
        /// <param name="tag">The tag.</param>
        public void SetInitializationMode(InitializationMode initializationMode, string tag)
        {
            var propertyBag = GetPropertyBagForTag(tag);
            if (propertyBag.IsPropertyAvailable("ActualInitializationMode"))
            {
                return;
            }

            propertyBag.SetValue("ActualInitializationMode", initializationMode);
        }
    }
} 
