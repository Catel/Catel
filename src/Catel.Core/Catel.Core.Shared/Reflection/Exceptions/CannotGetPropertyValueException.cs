// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CannotGetPropertyValueException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;

    /// <summary>
    /// Exception in case a property value cannot be get.
    /// </summary>
    public class CannotGetPropertyValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotGetPropertyValueException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public CannotGetPropertyValueException(string propertyName)
            : base(string.Format("Cannot get the value of property '{0}'", propertyName))
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