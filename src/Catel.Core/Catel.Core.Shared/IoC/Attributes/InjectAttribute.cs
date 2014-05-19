// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Attribute to specify that a specific property must be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InjectAttribute"/> class.
        /// </summary>
        public InjectAttribute(Type type = null, object tag = null)
        {
            Type = type;
            Tag = tag;
        }

        /// <summary>
        /// Gets or sets the type.
        /// <para />
        /// If <c>null</c>, the type must be determined dynamically.
        /// </summary>
        /// <value>The type.</value>
        /// <remarks>
        /// This property is settable so it can be filled when the type is <c>null</c>.
        /// </remarks>
        public Type Type { get; set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }
    }
}