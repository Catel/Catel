// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoCompletionService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Data;
    using Catel.Reflection;

    /// <summary>
    /// Service to implement auto completion features.
    /// </summary>
    public class AutoCompletionService : IAutoCompletionService
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletionService"/> class.
        /// </summary>
        public AutoCompletionService()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the auto complete values.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="source">The source.</param>
        /// <returns>System.String[].</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        public virtual string[] GetAutoCompleteValues(string property, string filter, IEnumerable source)
        {
            Argument.IsNotNull("source", source);

            var propertyValues = new List<string>();

            if (string.IsNullOrWhiteSpace(property))
            {
                try
                {
                    // Filter items directly
                    propertyValues.AddRange(from x in source.OfType<string>()
                                            select x);
                }
                catch (Exception)
                {
                    // Swallow
                }
            }
            else
            {
                propertyValues.AddRange(from x in source.Cast<object>()
                                        select GetPropertyValue(x, property));
            }

            propertyValues = propertyValues.Where(x => !string.Equals(x, "null")).Distinct().ToList();

            var filteredValues = propertyValues;

            if (!string.IsNullOrEmpty(filter))
            {
                filteredValues = filteredValues.Where(x => x.Contains(filter)).ToList();
            }

            var orderedPropertyValues = filteredValues.GroupBy(x => x).Select(g => new
            {
                Value = g.Key,
                Count = g.Select(x => x).Distinct().Count()
            }).OrderBy(x => x.Count).Select(x => x.Value).Take(10);

            return orderedPropertyValues.OrderBy(x => x).ToArray();
        }

        private static string GetPropertyValue(object obj, string propertyName)
        {
            object value = null;

            var modelBase = obj as ModelBase;
            if (modelBase != null)
            {
                if (modelBase.IsPropertyRegistered(propertyName))
                {
                    value = modelBase.GetValueFast(propertyName);
                }
            }
            else
            {
                value = PropertyHelper.GetPropertyValue(obj, propertyName);
            }

            return ObjectToStringHelper.ToString(value);
        }
        #endregion
    }
}