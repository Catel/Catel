// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCopUnusedFeatureRule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop.Rules
{
    using System.Collections.Generic;

    /// <summary>
    /// Rule to find out unused feature counts.
    /// </summary>
    public class UnusedFeatureApiCopRule : ApiCopRule
    {
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnusedFeatureApiCopRule" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="level">The level.</param>
        public UnusedFeatureApiCopRule(string name, string description, ApiCopRuleLevel level)
            : base(name, description, level)
        {
        }

        /// <summary>
        /// Increases the count and determines whether the feature is being used at the moment.
        /// </summary>
        /// <param name="isUsed">if set to <c>true</c>, the feature is being used.</param>
        /// <param name="tag">The tag.</param>
        public void IncreaseCount(bool isUsed, string tag)
        {
            Argument.IsNotNullOrWhitespace("tag", tag);

            lock (_lock)
            {
                var propertyBag = GetPropertyBagForTag(tag);

                if (!propertyBag.IsPropertyAvailable("TotalCount"))
                {
                    propertyBag.SetPropertyValue("TotalCount", 0);
                    propertyBag.SetPropertyValue("UsedCount", 0);
                    propertyBag.SetPropertyValue("UnusedCount", 0);
                }

                propertyBag.UpdatePropertyValue<int>("TotalCount", x => x + 1);

                if (isUsed)
                {
                    propertyBag.UpdatePropertyValue<int>("UsedCount", x => x + 1);
                }
                else
                {
                    propertyBag.UpdatePropertyValue<int>("UnusedCount", x => x + 1);
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
            Argument.IsNotNullOrWhitespace("tag", tag);

            lock (_lock)
            {
                var propertyBag = GetPropertyBagForTag(tag);
                if (propertyBag.GetAllProperties().Count == 0)
                {
                    return true;
                }

                var totalCount = propertyBag.GetPropertyValue<int>("TotalCount");
                //var usedCount = propertyBag.GetPropertyValue<int>("UsedCount");
                var unusedCount = propertyBag.GetPropertyValue<int>("UnusedCount");

                return totalCount == unusedCount;
            }
        }

        /// <summary>
        /// Gets the result as text.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The result as text.</returns>
        public override string GetResultAsText(string tag)
        {
            var propertyBag = GetPropertyBagForTag(tag);
            if (propertyBag.GetAllProperties().Count == 0)
            {
                return null;
            }

            return string.Format("[{0}] Feature used '{1}' of '{2}' times", tag,
                propertyBag.GetPropertyValue<int>("UsedCount"),
                propertyBag.GetPropertyValue<int>("TotalCount"));
        }
    }
}