// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNotFoundException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;

    /// <summary>
    /// Exception for in case a property is not found.
    /// </summary>
    public class PropertyNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNotFoundException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyNotFoundException(string propertyName)
            : base(string.Format("Property '{0}' is not found", propertyName))
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