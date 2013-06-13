// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CannotSetPropertyValueException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;

    /// <summary>
    /// Exception in case a property value cannot be set.
    /// </summary>
    public class CannotSetPropertyValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotSetPropertyValueException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public CannotSetPropertyValueException(string propertyName)
            : base(string.Format("Cannot set the value of property '{0}'", propertyName))
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; private set; }
    }
}