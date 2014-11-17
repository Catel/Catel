// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNotNullableException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Exception when a property value is set to null but when the type does not support
    /// null values.
    /// </summary>
    public class PropertyNotNullableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNotNullableException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that caused the exception.</param>
        /// <param name="propertyType">Type of the object that is trying to register the property.</param>
        public PropertyNotNullableException(string propertyName, Type propertyType)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' on type '{1}' does not support null-values", propertyName, propertyType))
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        /// <value>The property name.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the property type for which the property is already registered.
        /// </summary>
        /// <value>The property type for which the property is already registered.</value>
        public Type PropertyType { get; private set; }
    }
}