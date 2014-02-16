// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCopRule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ApiCop
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Data;

    /// <summary>
    /// Class containing an ApiCop rule.
    /// </summary>
    public abstract class ApiCopRule : IApiCopRule
    {
        private readonly object _lock = new object();

        private readonly Dictionary<string, PropertyBag> _propertiesByTag = new Dictionary<string, PropertyBag>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCopRule" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="level">The level.</param>
        protected ApiCopRule(string name, string description, ApiCopRuleLevel level)
        {
            Name = name;
            Description = description;
            Level = level;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private  set; }

        /// <summary>
        /// Gets the level of impact this rule has.
        /// </summary>
        /// <value>The level.</value>
        public ApiCopRuleLevel Level { get; private set; }

        /// <summary>
        /// Gets the property bag for the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The property bag.</returns>
        protected PropertyBag GetPropertyBagForTag(string tag)
        {
            lock (_lock)
            {
                if (!_propertiesByTag.ContainsKey(tag))
                {
                    _propertiesByTag[tag] = new PropertyBag();
                }

                return _propertiesByTag[tag];
            }
        }

        /// <summary>
        /// Gets all the tags used by this rule.
        /// </summary>
        /// <returns>The list of tags.</returns>
        public IEnumerable<string> GetTags()
        {
            lock (_lock)
            {
                return _propertiesByTag.Keys.ToArray();
            }
        }

        /// <summary>
        /// Determines whether the specified ApiCop rule is valid.
        /// </summary>
        /// <param name="apiCop">The ApiCop.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the specified ApiCop is valid; otherwise, <c>false</c>.</returns>
        public abstract bool IsValid(IApiCop apiCop, string tag);
    }
}