// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncludeInSerializationAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Attribute to define that a specific member must be included in the serialization by the serialization engine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class IncludeInSerializationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeInSerializationAttribute"/> class.
        /// </summary>
        public IncludeInSerializationAttribute()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeInSerializationAttribute"/> class.
        /// </summary>
        /// <param name="name">Name of the member.</param>
        public IncludeInSerializationAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the mapped.
        /// </summary>
        /// <value>The name of the mapped.</value>
        public string Name { get; set; }
    }
}