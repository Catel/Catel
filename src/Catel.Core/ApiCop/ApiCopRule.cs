// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiCopRule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
        /// <param name="url">The URL.</param>
        protected ApiCopRule(string name, string description, ApiCopRuleLevel level, string url = null)
        {
            Name = name;
            Description = description;
            Level = level;
            Url = url;
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
        /// Gets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; private set; }

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
                AddTag(tag);

                return _propertiesByTag[tag];
            }
        }

        /// <summary>
        /// Adds the tag so it is known in this rule.
        /// </summary>
        /// <param name="tag">The tag.</param>
        protected void AddTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                lock (_lock)
                {
                    if (!_propertiesByTag.ContainsKey(tag))
                    {
                        _propertiesByTag[tag] = new PropertyBag();
                    }
                }
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

        /// <summary>
        /// Gets the result as text.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The result as text.</returns>
        public abstract string GetResultAsText(string tag);
    }
}