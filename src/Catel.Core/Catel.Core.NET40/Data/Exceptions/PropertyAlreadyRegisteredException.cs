// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyAlreadyRegisteredException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Exception when a property is added to the <see cref="ModelBase{TModel}"/> class that is
    /// already registered by the object.
    /// </summary>
    public class PropertyAlreadyRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyAlreadyRegisteredException"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that caused the exception.</param>
        /// <param name="propertyType">Type of the object that is trying to register the property.</param>
        public PropertyAlreadyRegisteredException(string propertyName, Type propertyType)
            : base(string.Format(CultureInfo.InvariantCulture, "Property '{0}' is already registered on type '{1}'", propertyName, propertyType))
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