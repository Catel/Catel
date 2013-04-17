// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;

#if !NET
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Class holding a property value to serialize using the <see cref="ModelBase"/>.
    /// </summary>
#if !NET
    [DataContract]
#else
    [Serializable]
#endif
    public class PropertyValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        public PropertyValue() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <exception cref="ArgumentException">The <c>Key</c> of <paramref name="keyValuePair"/> is <c>null</c> or whitespace.</exception>
        public PropertyValue(KeyValuePair<string, object> keyValuePair)
            : this(keyValuePair.Key, keyValuePair.Value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public PropertyValue(string name, object value)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
#if !NET
        [DataMember]
#endif
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        /// <value>The value of the property.</value>
#if !NET
        [DataMember]
#endif
        public object Value { get; set; }
    }
}