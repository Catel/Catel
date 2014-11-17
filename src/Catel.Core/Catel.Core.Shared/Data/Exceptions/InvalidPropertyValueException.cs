// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidPropertyValueException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Exception when an the new value of a property of the <see cref="ModelBase"/> class is invalid.
    /// </summary>
    public class InvalidPropertyValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPropertyValueException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that caused the exception.</param>
        /// <param name="expectedType">Expected type for the property.</param>
        /// <param name="actualType">Actual object value type.</param>
        public InvalidPropertyValueException(string propertyName, Type expectedType, Type actualType)
            : base(string.Format(CultureInfo.InvariantCulture, "Expected a value of type '{0} instead of '{1}' for property '{2}'", expectedType, actualType, propertyName))
        {
            PropertyName = propertyName;
            ExpectedType = expectedType;
            ActualType = actualType;
        }

        /// <summary>
        /// Gets or sets the property name.
        /// </summary>
        /// <value>The property name.</value>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets or sets the expected type.
        /// </summary>
        /// <value>The expected type.</value>
        public Type ExpectedType { get; private set; }

        /// <summary>
        /// Gets or sets the actual type.
        /// </summary>
        /// <value>The actual type.</value>
        public Type ActualType { get; private set; }
    }
}