// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPropertyException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Exception is When an invalid property is added to the <see cref="ModelBase{TModel}"/> class.
    /// </summary>
    public class InvalidPropertyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPropertyException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that caused the exception.</param>
        public InvalidPropertyException(string propertyName)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' is invalid (not serializable?)", string.IsNullOrEmpty(propertyName) ? "null reference property" : propertyName))
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        /// <value>The property name.</value>
        public string PropertyName { get; private set; }
    }
}