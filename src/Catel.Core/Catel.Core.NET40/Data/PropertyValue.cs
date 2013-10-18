// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Class holding a property value to serialize using the <see cref="ModelBase"/>.
    /// </summary>
#if !NET
    [DataContract]
#else
    [Serializable]
#endif
    public class PropertyValue
#if NET
        : ISerializable
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        public PropertyValue() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <c>Key</c> of <paramref name="keyValuePair"/> is <c>null</c> or whitespace.</exception>
        public PropertyValue(PropertyData propertyData, KeyValuePair<string, object> keyValuePair)
            : this(propertyData, keyValuePair.Key, keyValuePair.Value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        /// <param name="propertyData">The property data.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public PropertyValue(PropertyData propertyData, string name, object value)
        {
            Argument.IsNotNull("propertyData", propertyData);
            Argument.IsNotNullOrWhitespace("name", name);

            PropertyData = propertyData;
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

        /// <summary>
        /// Gets the property data.
        /// </summary>
        /// <value>The property data.</value>
        [XmlIgnore]
        public PropertyData PropertyData { get; internal set; }

        /// <summary>
        /// Gets or sets the graph identifier.
        /// </summary>
        /// <value>The graph identifier.</value>
        [XmlIgnore]
        public int GraphId { get; set; }

        /// <summary>
        /// Gets or sets the graph reference identifier.
        /// </summary>
        /// <value>The graph reference identifier.</value>
        [XmlIgnore]
        public int GraphRefId { get; set; }

#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValue"/> class.
        /// </summary>
        public PropertyValue(SerializationInfo info, StreamingContext context)
        {
            Argument.IsNotNull("info", info);

            Name = info.GetString("Name");
            Value = info.GetValue("Value", typeof(object));

            try
            {
                GraphId = (int)info.GetValue("GraphId", typeof(int));
                GraphRefId = (int)info.GetValue("GraphRefId", typeof(int));
            }
            catch (Exception)
            {
                // Required for backwards compatibility
            }
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Argument.IsNotNull("info", info);

            info.AddValue("Name", Name);
            info.AddValue("Value", Value);
            info.AddValue("GraphId", GraphId);
            info.AddValue("GraphRefId", GraphRefId);
        }
#endif
    }
}